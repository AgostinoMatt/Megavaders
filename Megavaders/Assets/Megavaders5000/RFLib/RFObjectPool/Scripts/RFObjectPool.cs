using UnityEngine;
using System.Collections.Generic;


// OBJECT POOLING for GAMEOBJECTS
// Inspired by:
// https://gist.github.com/quill18/5a7cfffae68892621267


// TODO: If the SAME GAME OBJECT (for example, a particle effect) is used in multiple places, but parented differently, then what?
//       - Perhaps disregard parenting management?

namespace RFLib
{
	public static class RFObjectPool 
	{
		const int INITIAL_POOL_SIZE = 5;


		static Dictionary<GameObject, RFGameObjectPool> objectPools;	// Dictionary of object pools (to game object)
		static Dictionary<string, RFGameObjectPool> objectPoolsByID;	// Dictionary of object pools (to pool id)

		/// <summary>
		/// Initialize an object pool based on game object
		/// </summary>
		/// <param name="prefab">Game Prefab to instance/pool</param>
		/// <param name="poolID">A Name/ID for the pool, useful for for spawning based on desired game object OR object pool</param>
		/// <param name="startPoolSize">Initial Pool size</param>
		/// <param name="spawnTarget">Target transform to parent spawned item to; may be null</param>
		public static void Init(GameObject prefab, string poolID = null, int startPoolSize = INITIAL_POOL_SIZE, Transform spawnTarget=null)
		{
			if( prefab == null )
			{
				Debug.LogError( " NULL Prefab passed to init! You must pass a valid game object." );

			}

			if( objectPools == null )
			{
				objectPools = new Dictionary<GameObject, RFGameObjectPool>();
			}
			if( objectPoolsByID == null )
			{
				objectPoolsByID = new Dictionary<string, RFGameObjectPool>();
			}


			// If the gameobject type we want to pool is not null and does not
			// have a pool entry, create a new pool of startPoolSize
			if( prefab != null && objectPools.ContainsKey( prefab ) == false )
			{
				RFGameObjectPool op = new RFGameObjectPool( prefab, startPoolSize, poolID,spawnTarget );	
				objectPools[ prefab ] = op;
				objectPoolsByID[ op.GameObjectPoolID ] = op;
			}
		}
		/// <summary>
		/// Initialize and preload an object pool.  Pool id will be named/ID'd as the gameobject's hashasble value
		/// </summary>
		/// <param name="prefab">Prefab to prelaod</param>
		/// <param name="amount">Amount to initially instance</param>
		/// <param name="parent">Target transform to spawn to</param>
		public static void Preload(GameObject prefab, int amount, Transform parent)
		{
			Preload( prefab, null, amount, parent );
		}
		/// <summary>
		/// Initialize and preload an object pool.  If POOL ID is null, the pull id will be named/ID'd as the gameobject's hashasble value
		/// </summary>
		/// <param name="prefab">Prefab to prelaod</param>
		/// <param name="amount">Amount to initially instance</param>
		/// <param name="parent">Target transform to spawn to</param>
		public static void Preload(GameObject prefab, string poolID=null, int amount=INITIAL_POOL_SIZE, Transform parent=null)
		{
			Init( prefab,poolID, amount, parent );
			GameObject[] gobs = new GameObject[amount];
			for(int cnt = 0; cnt < amount; cnt ++)
			{
				GameObject spawned = Spawn(prefab, Vector3.zero, Quaternion.identity);
				gobs[ cnt ] = spawned;

			}

			for( int cnt = 0; cnt < amount; cnt++ )
			{
				Despawn( gobs[ cnt ] );
			}
		}

		/// <summary>
		/// Spawn an object from the pool. Pools MUST BE INITIALIZED FIRST!
		/// </summary>
		/// <param name="prefab">Gameobject to spawn from a pool</param>
		/// <param name="pos">Position to spawn</param>
		/// <param name="rot">Rotation to spawn</param>
		public static GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
		{
			GameObject gob = objectPools[ prefab ].Spawn( pos, rot );
			return gob;
		}

		/// <summary>
		/// Spawn an object from the pool. Pools MUST BE INITIALIZED FIRST!
		/// </summary>
		/// <param name="poolID">Pool ID To Spawn From</param>
		/// <param name="pos">Position to spawn</param>
		/// <param name="rot">Rotation to spawn</param>
		public static GameObject Spawn(string poolID, Vector3 pos, Quaternion rot)
		{
			GameObject gob = objectPoolsByID[ poolID ].Spawn( pos, rot );
			return gob;

		}


		/// <summary>
		/// Despawn the specified gameObject, placing it back into it's respective pool
		/// </summary>
		/// <param name="gameObject">Game object to despawn</param>
		public static void Despawn(GameObject gameObject)
		{
			RFGameObjectPoolMember rpm = gameObject.GetComponent<RFGameObjectPoolMember>();
			if( rpm != null )
				rpm.objectPool.Despawn( gameObject );
			else
			{
				GameObject.Destroy(gameObject);
			}
			
		}

		/// <summary>
		/// Clear the object pools; if a scene is destroyed, it needs to clean out the object pools
		/// or there will be issues with transform parenting
		/// </summary>
		public static void ClearAll()
		{
			foreach (RFGameObjectPool op in objectPools.Values) 
			{
				op.Clear();

			}
			objectPools.Clear();
			objectPoolsByID.Clear ();
		}
	

	}
}