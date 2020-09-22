using UnityEngine;
using System;
using System.Collections;
using RFLib;

/// <summary>
/// Missile controller - controls missile movement for player and ememy fired missiles
/// </summary>
public class MissileController : MonoBehaviour 
{
    [Tooltip("Spawn delay for a missile tracer")]
    [SerializeField]
	float TracerSpawnDelay	= 0.05f;


	[Tooltip("The y speed of the fired missile. Enemies fire down, so make sure it is negative")]
	[SerializeField]
    float yvel = 4f; 					// Player missiles fire upwards, invader missiles fire down - default to a player missile
    

	GameObject firedBy;			// Maintian a reference to the firer; used by other logic

    float dropTime = 0.0f;		// time to wait before dropping a "tracer". tracers are just the game objects used to create a missile tail

	// Use this for initialization
	void Start () 
    {
    //    gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update () 
    {
        Vector3 pos = transform.position;
		pos.y += (yvel * Time.deltaTime);
        transform.position = pos;

		// Spawn a trace. Traces have a limited lifetime; they don't actully move--just shrink down and despawn
		// However, there can be quite a few on screen at once, so they are spawned/despawed with an object pool
        if(dropTime <= 0.0f)
        {
			GameObject trace = RFObjectPool.Spawn("MissileTrail", pos, Quaternion.identity);
			dropTime = TracerSpawnDelay;
        }
        dropTime -= Time.deltaTime;


		if (pos.y >  GameGlobals.MISSILE_CONSTRAINT_Y || pos.y < -GameGlobals.MISSILE_CONSTRAINT_Y)
        {
            killMissile();
        }
	}

	/// <summary>
	/// Players fired missile
	///  - For the player, just use all the defaults
	/// </summary>
	/// <param name="playerObject">Player object.</param>
    public void PlayerFired(GameObject playerObject)
    {

        firedBy = playerObject;
    }

	/// <summary>
	/// Enemy Fired the Missile
	/// </summary>
	/// <param name="enemy">Enemy.</param>
    public void EnemyFired(GameObject enemy)
    {
		// make sure yvel is negative!
		if (yvel > 0) yvel = -yvel;

		firedBy = enemy;

        gameObject.SetActive(true);
		Vector3 pos = enemy.transform.position;

		// Place the missile just slightly below the enemy
        pos.y -= 0.1f;
        transform.position = pos;


    }

    
    // Using OnTriggerEnter2D - missile rigid body set to kinematic; we're not using the phyics engine collision
    // mechanics.  We simply want to destroy something (enemy, player, shield)
    void OnTriggerEnter2D(Collider2D collidedWith)
    {

        // When Y VEl is > 0, the player fired the one and only allowable player missle.
        // We do not care if an enemy's missile touches another enemy..
        if(collidedWith.tag == "Enemy" && yvel > 0)
        {
            // Destroy enemy, inactivate player missile            
			EnemyController ic = collidedWith.GetComponent<EnemyController>();
            ic.missileHit();
            killMissile();
        }
        // Boom! Player hit. Ouch. 
        else if(collidedWith.tag == "Player" && yvel < 0)
        {
            PlayerController pc = collidedWith.GetComponent<PlayerController>();
            pc.missileHit();
            killMissile();
        }
        else if (collidedWith.tag == "Shield")
        {
        
            collidedWith.gameObject.SetActive(false);
            killMissile();
        }

    }

    private void killMissile()
    {

        // The missile was already "killed" either by way of despawning or by Destroy -- don't kill it again 
        if (!gameObject.activeInHierarchy) return;

        bool dodestroy = true;
        if(firedBy != null)
        {
            // If the firedBy game object is not an enemy (it is a player), then notify
            // the gameobject that this particular missile is no loner "alive". Basically, allowing a
            // "reset" for the player missile, as only 1 player missile can exist at a time.
            if(firedBy.tag != "Enemy")
                firedBy.SendMessage("firedMissileDead", SendMessageOptions.DontRequireReceiver     );

            // Missile was fired by an "enemy" --- so, instead of destroying the missile, despawn it
            else
            {
                RFObjectPool.Despawn(this.gameObject);
                dodestroy = false;
            }

            firedBy = null;
            

        }
        if (dodestroy)
        {
            gameObject.SetActive(false);
            Destroy(this.gameObject);
        }

    }
}
