using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class MegavadersManager : MonoBehaviour 
{
	[MenuItem ("Megavaders 5000/Clear Scores")]
	public static void ClearHighScoresFile()
	{
			File.Delete(HighScoreDataManager.GetGameScoreFile());
	}



}


/* See if inputs are setup, if not, warn the user
 * 
 */ 
[InitializeOnLoad]
public class ReviewInputManager
{
    // Just verify that the Input Manager has the necessary axis set up
	static ReviewInputManager()
	{

		List<string> requiredAxes = new List<string>(){"Fire1", "Horizontal", "Vertical" /*, "Exit" */};

		SerializedObject sObject = new SerializedObject( AssetDatabase.LoadAllAssetsAtPath( "ProjectSettings/InputManager.asset")[0]);

		SerializedProperty axes = sObject.FindProperty("m_Axes");

		axes.Next(true);
		axes.Next(true);
		while(axes.Next(false))
		{
			SerializedProperty axis = axes.Copy();
			axis.Next(true);
			if( requiredAxes.Contains(axis.stringValue))
				requiredAxes.Remove(axis.stringValue);
		}

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (requiredAxes.Count != 0)
		{
			string requiredAxesNames = string.Join(", ", requiredAxes.ToArray());
            Debug.LogWarning("The following Input Axes should be configured in the Input Manager: " + requiredAxesNames);
        }
#endif
    }
}