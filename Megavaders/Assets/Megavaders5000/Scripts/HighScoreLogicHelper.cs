using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreLogicHelper : MonoBehaviour 
{
    public Text FireToRestart;

    public Image HighScoreText;
    public Image HighSCoreInstructions;
    public Transform container;


    public Vector3 CaretPos = Vector3.zero;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void showHide(bool tf)
    {
        container.gameObject.SetActive(tf);
        FireToRestart.gameObject.SetActive(!tf);
    }
}
