using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/* Entry screen for the game
 * Flip Flop between current high scores and Ememy score values
 * 
 * Press fire to start!
 * 
 */ 
public class SceneGameStartManager : MonoBehaviour 
{
	// Values Hooked up in Editor

	public Transform PlayerAndEnemiesContainer;
	public Transform EnemyContainer;

	public GameObject Explosion;

	public GameObject[] ScoreValues;		// Labels that display the value of an Enemy; 
	public HighScoresViewManager HighScoresViewer;

	// Hooked up in Editor End



	// Auto-kill the game if it has not been started before autoKillTime
	// Must have AUTO_KILL_GAME #defined
#if AUTO_KILL_GAME
	float autoKillTime = 90.0f;
#endif

	// Use this for initialization
	void Start () 
	{
		

		HighScoresViewer.DisplayHighScores();

		PlayerAndEnemiesContainer.gameObject.SetActive(false);
		HighScoresViewer.gameObject.SetActive(true);

		StartCoroutine("ManagePlayerAndEnemies");

	}

	/* ManagePlayerAndEnemies 
	 *  - Displayes the High Scores screen for a short time (default scene behavior)
	 *  - Sets the appropriate container game object to active (disables scores)
	 *  - explodes enemies and shows their score;
	 *  - restarts Coroutine
	 */ 
	IEnumerator ManagePlayerAndEnemies()
	{

		yield return new WaitForSeconds(4.0f);

		PlayerAndEnemiesContainer.gameObject.SetActive(true);
		foreach(Transform t in EnemyContainer)
			t.gameObject.SetActive(true);

		HighScoresViewer.gameObject.SetActive(false);

		yield return new WaitForSeconds(3.0f);
        explodeEnemy(0);
		yield return new WaitForSeconds(0.35f);
        explodeEnemy(1);
		yield return new WaitForSeconds(0.35f);
        explodeEnemy(2);


		// All done.. 
		yield return new WaitForSeconds(2.0f);
		foreach(GameObject go in ScoreValues)
			go.SetActive(false);
		
		PlayerAndEnemiesContainer.gameObject.SetActive(false);
		HighScoresViewer.gameObject.SetActive(true);

		StartCoroutine("ManagePlayerAndEnemies");


	}

	void explodeEnemy(int which)
	{
		Transform t = EnemyContainer.GetChild(which);
		Vector3 pos = t.position;
		t.gameObject.SetActive(false);

		GameObject go = Instantiate(Explosion) as GameObject;
		go.transform.position = pos;

		go = ScoreValues[which];
		go.SetActive(true);
		go.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);


	}
	
	// Update is called once per frame
	void Update () 
	{
		if(InputHelper.FIREBUTTON != "")
		{
			if (Input.GetButtonUp(InputHelper.FIREBUTTON) )
			{
				SceneManager.LoadScene("GamePlay");
			}
		}

#if AUTO_KILL_GAME
		autoKillTime -= Time.deltaTime;
		if(autoKillTime < 0)
		{
			gameObject.SendMessage("DoQuit");
		}
#endif

	}


}
