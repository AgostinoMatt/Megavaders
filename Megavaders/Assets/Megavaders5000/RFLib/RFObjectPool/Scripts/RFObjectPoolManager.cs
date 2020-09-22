using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace RFLib
{
	/// <summary>
	/// RF object pool manager - Provide in-editor functionalityy to create and manage RFObjectPool
	/// Preloads RFObjectPool with necessary data on startup.
	/// </summary>
	public class RFObjectPoolManager : SingletonMonoBehaviour<RFObjectPoolManager> 
	{
		[Serializable]
		class RFObjectPoolInitData
		{
			public string ObjectPoolID 			= null;
			public int StartCount				= 5;
			public GameObject SourcePrefab 		= null;
			public Transform TargetSpawnParent 	= null;
		}

#pragma warning disable 0649
        [SerializeField]
		List<RFObjectPoolInitData> InitData;
#pragma warning restore 0649

        bool isReady = false;

		public bool IsReady
		{
			get { return isReady; }
		}

		// Use this for initialization
		void Start () 
		{
			if( InitData != null )
			{
				for( int cnt = 0; cnt < InitData.Count; cnt++ )
				{
					RFObjectPoolInitData opid = InitData[ cnt ];
					// Only allow "properly" filled out objects to be spawned
					if( opid != null && opid.SourcePrefab != null )
					{
                        RFObjectPool.Preload( opid.SourcePrefab, opid.ObjectPoolID, opid.StartCount, opid.TargetSpawnParent );
					}
                }
			}

			isReady = true;
		}

		// Update is called once per frame
		void Update () { }

		void OnDestroy()
		{
			RFObjectPool.ClearAll ();
		}
	}
}

