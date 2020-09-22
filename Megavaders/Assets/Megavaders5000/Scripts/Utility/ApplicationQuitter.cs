using UnityEngine;
using System;
#if UNITY_EDITOR || UNITY_EDITOR_64
using UnityEditor;
#endif

/*  Quit Script -- simple input handling script to exit the program
 * 
 *  Requires QuitAxisName value to be setup in the input manager
 *  
 *  If AllowEscapeToQuit is set to True, then allow the Keyboard ESCAPE key to also function
 *  as an application exit trigger.
 * 
 */ 

public class ApplicationQuitter : MonoBehaviour 
{
	public bool AllowEscapeToQuit = false;
	public string QuitAxisName = "Exit";  // Static name of the axis we want to check input

	bool axisIsDefined = false;				// Set to true on start, if the axis is defined.



	// Use this for initialization
	void Start () 
	{
#pragma warning disable 0168   
        try
		{
			Input.GetAxis(QuitAxisName);	
			axisIsDefined = true;
		}
		catch(ArgumentException e)
		{
			
		}
#pragma warning restore 0168

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!axisIsDefined && !AllowEscapeToQuit)
		{
			Debug.LogWarning(string.Format("ApplicationQuitter Warning: The {0} input axis is not defined and AllowEscapeToExit is also false. \nPlayers will not be able to exit the application using ApplicationQuitter", QuitAxisName));
		}
#endif

    }
	
	// Update is called once per frame
	void Update () 
	{
		if( (axisIsDefined && Input.GetAxis(QuitAxisName) != 0) || (AllowEscapeToQuit && Input.GetKeyDown(KeyCode.Escape)) )
		{
			DoQuit();
		}
	}


	// Do Quit -- 
	public void DoQuit()
	{
		// If we turned the cursor off, make sure it's back on!
		if(Cursor.visible == false)	Cursor.visible = true;

#if UNITY_EDITOR || UNITY_EDITOR_64
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif


	}
}
