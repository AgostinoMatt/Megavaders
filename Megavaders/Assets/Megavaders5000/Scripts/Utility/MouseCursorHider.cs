using UnityEngine;
using System.Collections;

/* Hide the mouse cursor when playing
 * 
 * When playing from the editor, we want to SHOW the cursor when
 * it runs beyond the constraints of the game window
 */ 

public class MouseCursorHider : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
#if UNITY_EDITOR
		if(Input.mousePresent)
		{
			Vector3 mousePos = Input.mousePosition;
			Cursor.visible = !( mousePos.x > 0 && mousePos.y > 0 && mousePos.x < Screen.width && mousePos.y < Screen.height   );
		}
#endif
	
	}

	void OnDestroy()
	{
		Cursor.visible = true;
	}
}
