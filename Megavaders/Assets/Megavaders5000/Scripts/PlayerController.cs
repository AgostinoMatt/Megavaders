using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

	public SceneGamePlayManager gameLogicManager;
    // We only allow a single missle shot. Instantiate it, set it as inactive, keep a reference to it, and so on.
    public MissileController missile;

    public GameObject MissilePrefab;

    public Slider slider;
    public float sliderValue;

    [SerializeField]
    GameObject ExplosionFX = null;

    public bool autoMode = true; // Used by the main screen to update the unit via coode

    [SerializeField]
    float PlayerMoveSpeed = 2.5f;

	// Use this for initialization
	void Start () 
	{

	}
	void OnEnable()
	{
		if(autoMode)
		{
		
			Vector3 pos = transform.position;
			pos.x = 0;
			transform.position = pos;
		}
	}
	
	// Update is called once per frame
	void Update () 
    {
        sliderValue = slider.value;
        Vector3 temp = transform.position;
        temp.x = sliderValue;
        transform.localPosition = temp;

        fireMissile();

        /*if (!autoMode)
		{
	        // Accept player input
	       if (gameLogicManager == null || gameLogicManager.isPlaying())
	        {
				if(Input.GetAxis(InputHelper.FIREBUTTON) == 1)
	            {
	                fireMissile();

	            }


				float horizMove = Input.GetAxis(InputHelper.HORIZONTAL);
	            if( horizMove != 0)
	            {
	                Vector3 pos = transform.position;
	                pos.x += Mathf.Abs(PlayerMoveSpeed) * horizMove * Time.deltaTime;
					pos.x = Mathf.Clamp (pos.x, -GameGlobals.SCREEN_CONSTRAINT_X, GameGlobals.SCREEN_CONSTRAINT_X);
	                transform.position = pos;

	            }
	        }
		}*/
	}

	void FixedUpdate()
	{
		// Auto Update the player unit
		if(autoMode)
		{
			Vector3 pos = transform.position;
			if (pos.x > 4.5 || pos.x < -4.5)
                PlayerMoveSpeed = -PlayerMoveSpeed;
			pos.x += PlayerMoveSpeed * 0.025f;
			
			transform.position = pos;
			
			if(Random.Range(0,100) < 10)
			{
				fireMissile();
			}
		}
	}


    // Restart -resets position
    public void restart()
    {
        if(missile != null)
        {
            Destroy(missile.gameObject);
            missile = null;
        }
        Vector3 pos = transform.position;
        pos.x = 0;
        transform.position = pos;


    }

    private void fireMissile()
    {

        if (missile == null)
        {
            GameObject go = Instantiate(MissilePrefab) as GameObject;

            Vector3 pos = transform.localPosition;

            pos.y += 0.1f;
            go.transform.localPosition = pos;

            missile = go.GetComponent<MissileController>();
            missile.PlayerFired(this.gameObject);
//            go.particleSystem.Play();
        }
    }


    public void missileHit()
    {
        GameObject go = Instantiate(ExplosionFX) as GameObject;
        go.transform.position = transform.position;
        gameLogicManager.DoGameOver();
    }

    public void firedMissileDead()
    {
        this.missile = null;
    }
}
