using UnityEngine;
using System.Collections;

/*  Singleton Classes
	
	inspired by / copied from: http://wiki.unity3d.com/index.php/Singleton

	Example Use:
	public class SomeClass : SingletonMonoBehavior<SomeClass>
	{
		protected SomeClass() {} // Protected Constructor -- force always singleton use
	}

*/


namespace RFLib
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		private static T _instance;
	 
		private static object _lock = new object();
	 
		public static T Instance
		{
			get
			{
	 
				lock(_lock)
				{
					if (_instance == null)
					{
						_instance = (T) FindObjectOfType(typeof(T));
	 
						if ( FindObjectsOfType(typeof(T)).Length > 1 )
						{
							Debug.LogError("[SingletonMonoBehaviour] Something went really wrong " +
								" - there should never be more than 1 singleton!" +
								" Reopening the scene might fix it.");
							return _instance;
						}
	 
						if (_instance == null)
						{
							Debug.Log("[SingletonMonoBehaviour] An instance of " + typeof(T) + "is needed in the scene!");
						} 
					}
	 
					return _instance;
				}
			}
		}
	}
}