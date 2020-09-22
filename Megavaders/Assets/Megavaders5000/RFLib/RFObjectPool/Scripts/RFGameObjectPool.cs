using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RFLib
{

	/// <summary>
	/// Monobehavior for pooled memebers
	/// </summary>
	public class RFGameObjectPoolMember : MonoBehaviour
	{
		public RFGameObjectPool objectPool = null;
		public void Despawn()
		{
			if( objectPool != null ) objectPool.Despawn( gameObject );
		}

	}

	/// <summary>
	/// Game Object Pool
	/// </summary>
	public class RFGameObjectPool 
	{
		public string GameObjectPoolID;				// String name of hte pool; useful in collections of pools


		GameObject gameObjectPrefab;			// The base prefab to manage
		Stack<GameObject> inactiveObjects;		// Collection of Inactive Game Objects (ready to spawn)
		Transform spawnTarget;					// Transform - target to place spawned objects

		public RFGameObjectPool(GameObject sourcePrefab, int startQty, string poolID=null, Transform inSpawnTarget = null)
		{
			gameObjectPrefab = sourcePrefab;
			spawnTarget = inSpawnTarget;

			if( string.IsNullOrEmpty( poolID ) )
				GameObjectPoolID = gameObjectPrefab.GetHashCode().ToString();
			else
				GameObjectPoolID = poolID;

			inactiveObjects = new Stack<GameObject>(startQty);
		}

		/// <summary>
		/// Clear out the object pool
		/// </summary>
		public void Clear()
		{
			gameObjectPrefab = null;
			inactiveObjects.Clear ();
			spawnTarget = null;
		}



		/// <summary>
		/// Spawn a gameobject at POSITION with ROTATION
		/// </summary>
		/// <param name="pos">Position.</param>
		/// <param name="rot">Rot.</param>
		public GameObject Spawn(Vector3 pos, Quaternion rot)
		{
			GameObject returnObject = null;
			if( inactiveObjects.Count > 0 )
			{
				returnObject = inactiveObjects.Pop();
				if( returnObject == null )
					return Spawn( pos, rot );

			}
			else
			{
				returnObject = (GameObject)GameObject.Instantiate( gameObjectPrefab );
				returnObject.AddComponent<RFGameObjectPoolMember>().objectPool = this;
			}

			returnObject.transform.position = pos;
			returnObject.transform.rotation = rot;

			if( spawnTarget != null )
				returnObject.transform.SetParent( spawnTarget );

			returnObject.SetActive( true );


			return returnObject;

		}

		/// <summary>
		/// Despawn the specified gameObject.
		/// </summary>
		/// <param name="gameObject">Game object to despawn</param>
		public void Despawn(GameObject gameObject)
		{
			// Make sure the despawned object belongs in THIS pool.
			RFGameObjectPoolMember rpm = gameObject.GetComponent<RFGameObjectPoolMember>();
			if( rpm != null )
			{
				if( rpm.objectPool == this )
				{
					gameObject.SetActive( false );
					inactiveObjects.Push( gameObject );
				}
				else
				{
					// The game object is a member to a different pool.  Programmer error in despawn request?
					Debug.Log("Despawning a gameobject to the wrong OBJECT POOL! Redirecting.");
					rpm.objectPool.Despawn( gameObject );
				}
			}
			else
			{
				GameObject.Destroy(gameObject);
			}

		}


	}
}