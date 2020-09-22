using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/**
 * OldSchoolColorRotator
 * 
 * Rotate the colors of a maskable graphic through some colors 
 * of a bygone era.
 * 
 */

public class OldschoolColorRotator : MonoBehaviour 
{
	const int MAX_COLORS = 14;

	public MaskableGraphic ColorTarget = null;	// The target graphic item to tweak

	// Min range 0 will update every frame
	// Max range of 5 seconds will seem like forever
	[Range(0.0f, 5.0f)]
	public float RotateTimeDelay= 0.05f;		// Time in seconds to delay changing colors

	Color32[] colors;
	int currentColorIndex = 0;								

	float nextTime = 0.0f;


	// Use this for initialization
	void Start () 
	{
		colors = new Color32[MAX_COLORS];

		colors[ 0] = new Color32(255, 255, 255, 255);	// White
		colors[ 1] = new Color32(128, 128, 128, 255);	// Gray

		colors[ 2] = new Color32(255,   0,   0, 255);	// Red
		colors[ 3] = new Color32(  0, 255,   0, 255);	// Green
		colors[ 4] = new Color32(  0,   0, 255, 255);	// Blue

		colors[ 5] = new Color32(128,   0,   0, 255);	// DRed
		colors[ 6] = new Color32(  0, 128,   0, 255);	// DGreen
		colors[ 7] = new Color32(  0,   0, 128, 255);	// DBlue

		colors[ 8] = new Color32(255,   0, 255, 255);	// Magenta
		colors[ 9] = new Color32(  0, 255, 255, 255);	// Cyan
		colors[10] = new Color32(255, 255,   0, 255);	// Yellow

		colors[11] = new Color32(128,   0, 128, 255);	// DMagenta
		colors[12] = new Color32(  0, 128, 128, 255);	// DCyan
		colors[13] = new Color32(128, 128,   0, 255);	// DYellow

		nextTime = RotateTimeDelay;

	}
	
	// Update is called once per frame
	void Update () 
	{
		nextTime -= Time.deltaTime;

		if(ColorTarget != null && nextTime <= 0)
		{
			nextTime = RotateTimeDelay;

			ColorTarget.color = colors[currentColorIndex];

			currentColorIndex ++;
			if (currentColorIndex >= MAX_COLORS) currentColorIndex = 0;
		}
	}
}
