using UnityEngine;
using System.Collections;

/* Play the marching sounds 
 * 
 */ 
public class MarchingSoundManager : MonoBehaviour 
{
	// Hooked up in the Editor: March_Beat2, March_Beat1, March_Beat3
	public AudioClip[] MarchingSounds;

	// Which beat to play
	private int beatnum = 1;
	private AudioSource marchingPlayer;

	// Use this for initialization
	void Start () 
	{
		beatnum = 1; // Reset "song" to first beat
		marchingPlayer = GetComponent<AudioSource>();
		if(marchingPlayer == null)
		{
			Debug.Log("No Marching Sound Audiosource?");
		}

		// Marching sounds should be March_Beat2, March_Beat1, March_Beat3.
		if(MarchingSounds == null)
		{
			Debug.Log("Marching sounds are not set up. Hook them up in the editor.(March_Beat2, March_Beat1, March_Beat3)");
		}

	}

	public void PlayMarchingBeat()
	{
		// Need to make sure the marching sounds are set up
		if(marchingPlayer == null || MarchingSounds == null || MarchingSounds.Length != 3) return;

		int num = beatnum % 2;

		if (beatnum % 7 == 0)
		{
			num = 2;
		}
		else if (beatnum % 8 == 0)
		{
			num = 1;
			beatnum = 1;
		}

		AudioClip ac = MarchingSounds[num];
		marchingPlayer.clip = ac;
		marchingPlayer.Play();

		beatnum += 1;

	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
