using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using RFLib;

public enum GameStates
{
	START,
	PLAYING,
	GAME_OVER,
	GETTING_SCORE
};


public class SceneGamePlayManager : MonoBehaviour 
{
    // |
    // | ITEMS HOOKED UP IN THE UNITY EDITOR
    // |

    // References to the Game's various views
    public GameObject PlayerExplosionParticles;

    // The container used for parenting spawned Enemy Missiles.
    public Transform EnemyMissileContainer;

    // Player Object Controller Reference
    public PlayerController Player;
    public Transform[] PlayerShields;

    // Missile for Enemy Invaders to fire
    public GameObject EnemyMissilePrefab;

    // UI Elements to update: Score Text and Wave Text
    public Text ScoreText;
    public Text WaveText;

    // |
    // | END OF ITEMS HOOKED UP IN THE UNITY EDITOR
    // |


    // Manage game states which basically set various modes, show screens, etc
    GameStates gameState = GameStates.START;
    
    private int currentScore = 0;
    private int currentWave = 1;

    // Maintain a list of the invaders. We could simply reference the enemy game objects in the EnemyContainer,
    // but that would then mean performing several getcomponent calls
	private List<EnemyController> enemiesList;

    // Enemy update Timer
    private float enemyUpdateTimer = 0.0f;
    private float enemyUpdateTimerMax = 0.5f;       // Maximum amount of time to wait between enemy update. 
                                                    // Enemy Updates happen faster as waves increase and less enemies exist
    // Invader missile drop controls
    private float lastFireTime  = 0.0f;             // when was the last missile fired?    

    [SerializeField]
    [Tooltip("The minimimum number of seconds before another enemy fire check is made.")]
    float EnemyFireDelay    = 1.0f;             // Fire delay, we dont want a bunch of missiles flying out


    [SerializeField]
    [Tooltip("Number between 1 and 100 representing the chance an enemy fire check is made.")]
    [Range(1,100)]
    int EnemyFireChance     = 10;              


    MarchingSoundManager marchingSoundManager = null;
    EnemyCreator enemyCreator = null;                   // The Creator Component for enemies

    // Used for testing; see Update function;
    Coroutine killingAll = null;

    void Awake()
	{
		marchingSoundManager = GetComponent<MarchingSoundManager>();
        enemyCreator = GetComponent<EnemyCreator>();
    }

	// Use this for initialization
	void Start () 
    {
		enemiesList = new List<EnemyController>();
		gameState = GameStates.START;
		StartGame();
	}


	// Update is called once per frame
	void Update () 
    {
        if(gameState == GameStates.PLAYING)
        {
            // Nothing to do here currently...
            enemyUpdateTimer += Time.deltaTime;
            if (enemyUpdateTimer >= enemyUpdateTimerMax)
            {
                enemyUpdateTimer = 0.0f;
                moveEnemies();

            }

            // Uncomment the below section to provide a way to blow up all enemies; good
            // for testing. See DestroAllEnemies below; adjust as needed.
            /*
            if(Input.GetKeyDown(KeyCode.B))
            {
                if(killingAll == null)
                    killingAll = StartCoroutine(DestroyAllEnemies());
            }
            */
        }
	}

    public bool isPlaying()
    {
        return gameState == GameStates.PLAYING;
    }

    // Start the game; update the game start to PLAYING,
    public void StartGame()
    {
        Player.gameLogicManager = this;

        // Set all Shields to active:
        int shieldCnt = PlayerShields.Length;
        for (int cnt = 0; cnt < shieldCnt; cnt ++ )
        {
            Transform playerShield = PlayerShields[cnt];
            for (int childNdx = 0; childNdx < playerShield.childCount; childNdx++)
                playerShield.GetChild(childNdx).gameObject.SetActive(true);

        }

        Player.restart();

        currentScore = 0;
        currentWave = 1;
        GameGlobals.SetSessionData(currentWave, currentScore);

        updateScore();
        updateWave();
        drawEnemies();

        this.Player.autoMode = false;
        gameState = GameStates.PLAYING;

    }

    private void updateScore(int amount=0)
    {
        currentScore += amount;
        ScoreText.text = "SCORE\n" + currentScore.ToString();
    }
    private void updateWave(int amount = 0)
    {
        currentWave += amount;
        WaveText.text = "WAVE\n" + currentWave.ToString();
    }

    // Draw a grid of our enemy invaders
    // 
    private void drawEnemies()
    {
        enemiesList = enemyCreator.CreateEnemies();
        foreach (EnemyController enemyController in enemiesList)
        {
            enemyController.gameLogicManager = this;
        }

        // Setup Enemy Invader Speeds..
        float cwave = (float)currentWave;
        enemyUpdateTimerMax = 0.35f - (0.01f * (cwave-1));
		EnemyController.MoveDirection = 1.0f;
		EnemyController.MoveVel = 0.1f + (0.01f * (cwave - 1)); // Start out each moving just a bit more

    }

	public void EnemyHit(EnemyController enemy)
    {
        enemy.Kill();

		updateScore( enemy.ScoreValue );
		enemiesList.Remove(enemy);

        // Increase the update speed
        enemyUpdateTimerMax -= 0.005f;

        // Slightly increase move distance
		EnemyController.MoveVel += 0.005f;


		// If there are no enemy invaders left, start the next wave
		int enemyCount = enemiesList.Count;
        if (enemyCount <= 0)
        {
            if (killingAll != null)
            {
                StopCoroutine(killingAll);
                killingAll = null;
            }
            updateWave(1);
            drawEnemies();
        }

    }

    public void DoGameOver()
    {

		gameState = GameStates.GAME_OVER;
        Player.gameObject.SetActive(false);
        GameGlobals.SetSessionData(currentWave, currentScore);
		Invoke("GameOverAfterExplosion", 0.85f);
        
    }

	public void GameOverAfterExplosion()
	{
		SceneManager.LoadScene("GameOver");
	}


    // March the invaders across the screen. if one hits an extent, move them all down and switch the movement direction
    // Update them here since they are sort of herky-jerky and all must move along X before a Y change takes place
    private void moveEnemies()
    {
		int maxCount = enemiesList.Count;
        bool moveDown = false;
        bool hitBottom = false;
        bool isPlaying = (gameState == GameStates.PLAYING);

		if(gameState == GameStates.PLAYING && marchingSoundManager != null) marchingSoundManager.PlayMarchingBeat();

        int randoFireCount = 0;

        for (int cnt = 0; cnt < maxCount; cnt++)
        {
			EnemyController enemyController = enemiesList[cnt];
			float xloc = enemyController.UpdateSprite(isPlaying);
            if (isPlaying)
            {
                float nextFireTime = lastFireTime + this.EnemyFireDelay;
                float currTime = Time.time;

                int fireChance = UnityEngine.Random.Range(0, 100);
                // Randomly shoot a missile if currtime has passed the limit
                if (fireChance < this.EnemyFireChance && currTime > nextFireTime)
                {
                    randoFireCount += 1;
                    enemyFireMissile(enemyController);
                    lastFireTime = currTime;
                }


                // X boundaries - if a the sprite has moved beyond the x constraints in either direction,
                // then signal a move down (moves all enemies down a row, then switches their direction)
                if ((xloc <= -GameGlobals.SCREEN_CONSTRAINT_X || xloc >= GameGlobals.SCREEN_CONSTRAINT_X ) )
                    moveDown = true;

            }

        }

        // Select a random enemy to fire from
        if (enemiesList.Count > 0)
        {
            for (int firecnt = 0; firecnt < randoFireCount; firecnt++)
            {
                int cnt = UnityEngine.Random.Range(0, enemiesList.Count);
                EnemyController enemyController = enemiesList[cnt];
                enemyFireMissile(enemyController);
            }
        }

        // MoveDown signalled, move all enemies down one row, and reverse the marching direction
        if (moveDown)
        {
			EnemyController.MoveDirection = -EnemyController.MoveDirection;
            for (int cnt = 0; cnt < maxCount; cnt++)
            {
				EnemyController enemyController = enemiesList[cnt];
                float yloc = enemyController.dropDownRow();
                // If any Enemy has reached the "bottom", then it's game over.
                if (yloc <= GameGlobals.ENEMY_LOWEST_POINT ) hitBottom = true;
            }
        }

        if (hitBottom)
            DoGameOver();
    }


    /// <summary>
    /// Called when an enemy should fire. If there are already MAX_ENEMY_MISSILES, then no firing will
    /// take place.  Checks the ACTIVE CHILD count, since pooling is used to spawn/despawn (activeate/deactive)
    /// enemy missiles.
    /// </summary>
    /// <param name="enemy"></param>
	private bool enemyFireMissile(EnemyController enemy)
    {
        int activeCount = 0;
        bool fired = false;
        for(int cnt = 0; cnt < EnemyMissileContainer.childCount; cnt ++ )
        {
            if (EnemyMissileContainer.GetChild(cnt).gameObject.activeInHierarchy == true) activeCount++;
            
        }

		// Fire a missile from the invader if the maximum number of active missiles don't already exist.
        if (activeCount < GameGlobals.MAX_ENEMY_MISSILES)
        {
            GameObject missile = RFObjectPool.Spawn(EnemyMissilePrefab, Vector3.zero, Quaternion.identity);
            if (missile != null)
            {
                MissileController mc = missile.GetComponent<MissileController>();
                mc.EnemyFired(enemy.gameObject);
                fired = true;
            }
        }
        return fired;
    }


    // Destroy them all!!!
    IEnumerator DestroyAllEnemies()
    {
        while (enemiesList.Count > 0)
        {
            EnemyController enemy = enemiesList[0];
            EnemyHit(enemy);
            yield return new WaitForSeconds(0.1f);
        }

    }
}
