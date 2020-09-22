using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/* HighScoreController -
 *   Manages the rendering of high scores data items
 *   in the High Scores Viewer
 * 
 *   High Score data is managed and modified by the HighScoreDataManager class
 *   -- those data modifications are reflected immediately.  Caret position is
 *   	updated via data this way as well.
 * 
 */ 
public class HighScoreController : MonoBehaviour
{

	// Values Hooked up in the editor
    public Transform Caret;

    public Text scoreText;
    public Text playerInitials;



    HighScore highscore;

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (playerInitials.text != highscore.initials)
            playerInitials.text = highscore.initials;

        if(highscore.caretLoc > -1)
        {
            if (Caret.gameObject.activeSelf == false)
                Caret.gameObject.SetActive(true);
            float px = -70;
            RectTransform t = Caret as RectTransform;

            Vector2 pos = t.anchoredPosition;
            pos.x = px + (highscore.caretLoc * 25);
            pos.y = -10;
            t.anchoredPosition = pos;
        }
        else
        {
            if (Caret.gameObject.activeSelf != false)
                Caret.gameObject.SetActive(false);

        }


	}

	// Set a reference to the High Score data to render
    public void SetData(HighScore hs)
    {
        highscore = hs;
        Caret.gameObject.SetActive(false);

        scoreText.text = hs.score.ToString();
        playerInitials.text = hs.initials;

    }
}
