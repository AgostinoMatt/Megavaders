using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



public class SceneGameOverManager : MonoBehaviour 
{
	SceneGamePlayManager gameLogic;

	public GameObject FireToContinue;
	public GameObject Instructions;

	public Text PlayerScoreDisplay;

	public GameStates gameState;

	int wave = 0;
	int score = 0;

	public GameObject ExplosionParticles;
	// Marching sounds are used to indicate score cursor movement
	public AudioClip[] MarchingSounds;
	public AudioClip ExplosionSound;


	public HighScoresViewManager highScoreViewManager;


	private AudioSource audioSource;

	float scoreDelay = -0.1f;
	float scoreDelayReset = 0.1f;

	float countdown = 10.0f;



	void Awake()
	{

        wave = GameGlobals.Wave;
        score = GameGlobals.Score;

		FireToContinue.SetActive(false);
		Instructions.SetActive(false);

	}
	// Use this for initialization
	void Start () 
	{
		audioSource = GetComponent<AudioSource>();

		if ( highScoreViewManager.HasHighscore(score, wave)  )
		{
			highScoreViewManager.StartHighScoreCollection(score, wave);

			gameState = GameStates.GETTING_SCORE;
			FireToContinue.SetActive(false);
			Instructions.SetActive(true);

		}
		else
		{
			// No High score, so -- just display the normal score set
			gameState = GameStates.GAME_OVER;
			FireToContinue.SetActive(true);
			Instructions.SetActive(false);

		}
		highScoreViewManager.DisplayHighScores();


	
	}
	
	// Update is called once per frame
	void Update () 
	{
		PlayerScoreDisplay.text = "Score: " + score.ToString();
	
		// Go Back To Main Page...
		if(gameState == GameStates.GAME_OVER)
		{
			countdown -= Time.deltaTime;
			if(countdown < 0)
			{
				SceneManager.LoadScene("GameStart");
				return;
			}

			if (Input.GetButtonUp(InputHelper.FIREBUTTON) )
			{
				Input.ResetInputAxes();
				gameState = GameStates.START;
				SceneManager.LoadScene("GameStart");

			}
		}
		else if(gameState == GameStates.GETTING_SCORE)
		{
			

			AudioClip ac = null;
			scoreDelay -= Time.deltaTime;
			
			if (Input.GetButtonUp(InputHelper.FIREBUTTON) || scoreDelay < -10.0f)
			{


				Vector3 pos = this.highScoreViewManager.NewScoreDone();

				if( pos != Vector3.zero)
				{
					GameObject go = Instantiate(ExplosionParticles) as GameObject;
					go.transform.position = pos;
				}

				gameState = GameStates.GAME_OVER;
				Input.ResetInputAxes();
//				ac = ExplosionSound;


				FireToContinue.SetActive(true);
				Instructions.SetActive(false);
				countdown = 10.0f;


				
			}
			else if (Input.GetAxis(InputHelper.HORIZONTAL) < 0 && scoreDelay < 0)
			{
				Input.ResetInputAxes();
				highScoreViewManager.NewScoreIndexDir(-1);
				ac = MarchingSounds[0];
				scoreDelay = scoreDelayReset;
				
			}
			else if (Input.GetAxis(InputHelper.HORIZONTAL) > 0 && scoreDelay < 0)
			{
				Input.ResetInputAxes();
				highScoreViewManager.NewScoreIndexDir(1);
				ac = MarchingSounds[0];
				scoreDelay = scoreDelayReset;
				
			}
			else if (Input.GetAxis(InputHelper.VERTICAL) > 0 && scoreDelay < 0)
			{
				Input.ResetInputAxes();
				highScoreViewManager.UpdateLetter(1);
				ac = MarchingSounds[2];
				scoreDelay = scoreDelayReset;
				
			}
			else if (Input.GetAxis(InputHelper.VERTICAL) < 0 && scoreDelay < 0)
			{
				Input.ResetInputAxes();
				highScoreViewManager.UpdateLetter(-1);
				ac = MarchingSounds[2];
				scoreDelay = scoreDelayReset;
			}
			if (ac != null)
			{
				// We are using our Marching Player audio player
				audioSource.clip = ac;
				audioSource.Play();
			}
		}
	}


}
