using UnityEngine;
using System.Collections;

/*  Helper class to identify which input axis definitions to use.  Sometimes
 *  USB joysticks cause identity issues on the PC. 
 * 
 *  We get around the identity issues by defining a couple joystick/keyboard
 *  options in the input manager; then when the player actually presses a fire
 *  button, we determine which one it is, and set the global input axis identifiers
 *  appropriately.
 *  
 *  View current input settings in the editor: Edit->Project Settings->Input
 */ 
public class InputHelper : MonoBehaviour 
{
	public static InputHelper _instance;
	public static string whichInput = "";


	public static string FIREBUTTON = "Fire1";
	public static string HORIZONTAL = "Horizontal";
	public static string VERTICAL = "Vertical";


	public string WhichInput; // Display in Editor

	void Awake()
	{
		if(_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}


	// Update is called once per frame
	void Update () 
	{
		WhichInput = whichInput.ToString();
		// Determine some input settings depending on what input item may be
		// usable.  
		// The value ultimately gets set once a player presses the appropriate fire button.
		if(whichInput == "")
		{
			// These should set up in Unity by default
			if (Input.GetButtonUp("Fire1"))
			{
				FIREBUTTON = "Fire1";
				HORIZONTAL = "Horizontal";
				VERTICAL = "Vertical";

				whichInput = "1";
			}
			else if (Input.GetButtonUp("Fire2"))
			{
				FIREBUTTON = "Fire2";
				HORIZONTAL = "Horizontal2";
				VERTICAL = "Vertical2";

				try
				{
					Input.GetAxis(HORIZONTAL);
				}
				catch
				{
					Debug.LogWarning("Horizontal2 Axis not set up. Reverting to Horizontal Axis 1");
					HORIZONTAL = "Horizontal";
				}
				try
				{
					Input.GetAxis(VERTICAL);
				}
				catch
				{
					Debug.LogWarning("Vertical2 Axis not set up. Reverting to Vertical Axis 1");
					HORIZONTAL = "Vertical";
				}


				whichInput = "2";
			}
		}
	}
}
