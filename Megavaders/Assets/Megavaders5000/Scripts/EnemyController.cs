using UnityEngine;
using System.Collections;

[RequireComponent( typeof(SpriteRenderer) )]
public class EnemyController : MonoBehaviour 
{

	public Sprite SpriteFrame1;
	public Sprite SpriteFrame2;

	public static float MoveVel       = 0.1f;
	public static float MoveDirection = 1.0f;

	// When this enemy is killed by the player, increase the player's score!
	public int ScoreValue = 10;

    [SerializeField]
    GameObject ExplosionPrefab = null;

	public SceneGamePlayManager gameLogicManager = null; // If added from the logic manager, this is set for some callback action

	public bool autoPlay = false;	// Autoplay happens in the intro screen

	// fill this with whatver is in SpriteFrame1 and SpriteFrame2 to create a simple
	// flip-flip animation between the two frames
	Sprite[] spriteFrames;
	int spriteFrameIndex = 0;

	SpriteRenderer spriteRenderer;

	float countdown = 0.5f;

	void Awake()
	{
		spriteFrames = new Sprite[2]{ SpriteFrame1, SpriteFrame2}	;

		if(SpriteFrame1 == null)
		{
			Debug.Log("Empty Enemy SpriteFrame1. Set the sprite in the editor!");
		}
		if(SpriteFrame2 == null)
		{
			Debug.Log("Empty Enemy SpriteFrame2. Set the sprite in the editor!");
		}
	}

	// Use this for initialization
	void Start () 
	{
		// SpriteRenderer required
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update () 
	{
        // "AutoPlay" is set in the intro scene; when enabled, change the sprite
        // displyed every 'countdown' seconds (0.5f)
		if(autoPlay)
		{
			countdown -= Time.deltaTime;
			if(countdown < 0)
			{
				UpdateSprite();
				countdown = 0.5f;
			}
		}


	}

	// Move invader down a row...
	public float dropDownRow(bool switchDir=false)
	{
		transform.Translate(0.0f, -0.25f, 0.0f);
		// Return the Y position so we know if we've hit the bottom. Game Over if so
		return transform.position.y;

	}

    public void Kill()
    {
        GameObject go = Instantiate(ExplosionPrefab) as GameObject;
        go.transform.position = transform.position;

        Destroy(gameObject);
    }

    // Hit by a missile.  
    public void missileHit()
	{
		if (gameLogicManager != null)
            gameLogicManager.EnemyHit(this);
	}


	// UpdateSprite visual. If also move is true, move the sprite a bit based on move direction
	// Return the sprites X position 
	// this is called by the game logic manager 
	public float UpdateSprite(bool alsoMove=false)
	{
		// Set the sprite 
		spriteFrameIndex += 1;
		if (spriteFrameIndex > 1) spriteFrameIndex = 0;

		
		spriteRenderer.sprite = spriteFrames[spriteFrameIndex];


		// if it moves, then move it!
		if(alsoMove)
		{
			transform.Translate(MoveVel * MoveDirection, 0, 0);
		}
		// Return the x position to the caller ( GameLogicManager) 
		return transform.position.x;
	}

	// Trigger when colliding with a player or a shield part.
	void OnTriggerEnter2D(Collider2D collidedWith)
	{
		// hit a shield, turn of collided parts
		if(collidedWith.tag == "Shield")
		{
			collidedWith.gameObject.SetActive(false);
		}
		else if(collidedWith.tag == "Player")
		{
			gameLogicManager.DoGameOver();
		}

	}

}