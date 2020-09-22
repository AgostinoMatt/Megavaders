using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighScoresViewManager : MonoBehaviour 
{

	public Transform HighScoresContainer; // Transform where we want to toss all the high scores

	public GameObject HighScoresRenderer; // A renderer used to display High Score data


	private HighScoreDataManager highScoresData;	// Data Management of our high scores


	int newHighScoreIndex = -1;				// Location in the HighScoresData list where the new high score is located
	HighScoreController currentHighScoreController = null;



	// Use this for initialization
	void Start () 
	{
		if(highScoresData == null)
		{
			initScoreData();
		}


	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void initScoreData()
	{
		highScoresData = new HighScoreDataManager();
		highScoresData.Load(); // Load the scores...

	}

	public int StartHighScoreCollection(int score, int wave)
	{
		newHighScoreIndex = highScoresData.AddScore("___", score, wave);
		return newHighScoreIndex;
	}

	public bool HasHighscore(int score, int wave)
	{
		return highScoresData.IsHighScore(score);
	}

	// Complete new score collection
	// Returns the location of the current high score controller Caret.
	public Vector3 NewScoreDone()
	{
		highScoresData.NewScoreDone();

		Vector3 pos = Vector3.zero;

		if(currentHighScoreController)
		{
			Vector2 vPos = RectTransformUtility.WorldToScreenPoint(null, currentHighScoreController.Caret.position);
			pos = new Vector3(vPos.x, vPos.y, 0);
		}

		currentHighScoreController 	= null;
		newHighScoreIndex  			= -1;

		return pos;
	}


	public void NewScoreIndexDir(int dir)
	{
		highScoresData.NewScoreNdxDir(dir);
	}
	public void UpdateLetter(int dir)
	{
		highScoresData.UpdateLetter(dir);
	}



	public void DisplayHighScores()
	{
		if(highScoresData == null) initScoreData();

		clearScoreContainer();
		List<HighScore> scores = this.highScoresData.highScores;
		for(int cnt = 0; cnt < scores.Count; cnt ++)
		{
			GameObject scoreGO = Instantiate(HighScoresRenderer) as GameObject;
			HighScoreController hsc = scoreGO.GetComponent<HighScoreController>();
			hsc.SetData(scores[cnt]);

			if(scores[cnt].caretLoc > -1)
				currentHighScoreController = hsc;
			

			scoreGO.transform.SetParent(this.HighScoresContainer, false);

		}
	}


	private void clearScoreContainer()
	{
		if(HighScoresContainer == null) return;

		while (HighScoresContainer.childCount > 0)
			DestroyImmediate(HighScoresContainer.GetChild(0).gameObject);

	}

}
