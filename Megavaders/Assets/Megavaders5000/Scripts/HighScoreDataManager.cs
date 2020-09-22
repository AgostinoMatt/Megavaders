using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;


/* Manages the loading and saving of score data
 * 
 * 
 */ 
public class HighScoreDataManager 
{

	public static string HIGHSCORE_FILENAME = "/gamedata.dat";

	static string VALID_CHARACTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_!$";

    public int validndx = 0;
    public int charndx = 0;

    public List<HighScore> highScores;
    public int maxScores = 10;	// Maximum number of scores to store

    public HighScore newScore = null;



    public HighScoreDataManager() 
    {
		validndx = VALID_CHARACTERS.Length;
        highScores = new List<HighScore>();
    }


	public static string GetGameScoreFile()
	{
		return Application.persistentDataPath + HIGHSCORE_FILENAME;;
	}


    public int AddScore(string initials, int score, int wave)
    {

        HighScore hs = new HighScore();
        hs.initials = initials;
        hs.wave = wave;
        hs.score = score;

        int newindx = -1;
        for (int cnt = 0; cnt < highScores.Count; cnt++)
        {
            HighScore currScore = highScores[cnt];
            if (score > currScore.score)
            {
                newindx = cnt;
                break;
            }
                
                
        }

        if (newindx == -1 && highScores.Count < maxScores)
        {
            highScores.Add(hs);
            newindx = highScores.IndexOf(hs);
        }
        else if (newindx > -1)
        {
            highScores.Insert(newindx, hs);
        }

        newScore = null;
        if(newindx > -1)
        {
            newScore = hs;
            hs.caretLoc = 0;
            charndx = 0;

            if (highScores.Count > maxScores)
                highScores.RemoveAt(highScores.Count - 1);

            Save();

        }

//        Debug.Log("NEW INDEX: " + newindx);
        return newindx;

    }

    // Move the character ndx left or right
    public void NewScoreNdxDir(int dir)
    {
		if(newScore == null) return;

        charndx += dir;
        if (charndx >= 3)
            charndx = 0;

        if (charndx < 0)
            charndx = 2;
        if(newScore != null)
            newScore.caretLoc = charndx;

        validndx = 0;
    }
    public void UpdateLetter(int dir)
    {
		if(newScore == null) return;

        validndx += dir;
		int len = VALID_CHARACTERS.Length-1;
        if (validndx < 0)
            validndx = len;
        if (validndx > len)
            validndx = 0;

		char[] t = VALID_CHARACTERS.ToCharArray();
        char newt = t[validndx];

        t = newScore.initials.ToCharArray();
        t[charndx] = newt;
        newScore.initials = new string(t);
    }

    public void NewScoreDone()
    {
        if (newScore != null)  newScore.caretLoc = -1;

        Save();
        newScore = null;
		validndx = VALID_CHARACTERS.Length;
        charndx = 0;
    }

    // Check for high score and provide the position it would be in
    // -1 "score" is not a high score
    public int HighScoreIndex(int score)
    {
        
        for(int cnt = 0; cnt < highScores.Count; cnt ++)
        {
            HighScore currScore = highScores[cnt];
            if (score > currScore.score)
                return cnt;
        }

        // If the number of scores is less than MaxScores
        if( highScores.Count < maxScores)
            return highScores.Count;

        // nope, you dont meet any criteria for a score
        return -1;

    }

	// Is score a high score?
	public bool IsHighScore(int score)
	{
		for(int cnt = 0; cnt < highScores.Count; cnt ++)
		{
			HighScore currScore = highScores[cnt];
			if (score > currScore.score)
				return true;
		}
		return false;
	}

    public void Save()
    {
        
		FileStream file = null;

        try
        {
			BinaryFormatter bf = new BinaryFormatter();
			file = File.Open(GetGameScoreFile(), FileMode.OpenOrCreate);
            bf.Serialize(file, highScores);
        }
        catch( Exception e)
        {
            Debug.Log("Could not serialize data :: " + e.Message);
        }
		finally
		{
			if(file != null)
	        	file.Close();
		}

    }
    public void Load()
    {
		bool highScoresLoadedOK  = false;
		if(File.Exists(GetGameScoreFile()))
        {
			FileStream file = null;
            try
            {
				BinaryFormatter bf = new BinaryFormatter();
				file = File.Open(GetGameScoreFile(), FileMode.Open);

                highScores = bf.Deserialize(file) as List<HighScore>;
                for (int cnt = 0; cnt < highScores.Count; cnt++)
                    highScores[cnt].caretLoc = -1;

				highScoresLoadedOK = true;
                         
            }
            catch(Exception e)
            {
                Debug.Log("Could not deserialize data :: " + e.Message);
            }
			finally
			{
				if(file != null)
            		file.Close();
			}
        }

		// High scores don't exist, or during the process, something went wrong.
		if(!highScoresLoadedOK)
		{
			highScores.Add( new HighScore("RUN",500));
			highScores.Add( new HighScore("EFI",400));
			highScores.Add( new HighScore("RE.",300));
			highScores.Add( new HighScore("COM",200));
			highScores.Add( new HighScore("-+-",100));
			highScores.Add( new HighScore("GET",90));
			highScores.Add( new HighScore("THE",80));
			highScores.Add( new HighScore("FUN",70));
			highScores.Add( new HighScore("GOI",60));
			highScores.Add( new HighScore("NG!",50));
		}

    }
}

[Serializable]
public class HighScore
{

    public int caretLoc = -1;

    public string initials = "";
    public int score = 0;
    public int wave = 0;

	public HighScore() {}
	public HighScore(string inits, int insc) 
	{
		initials = inits;
		score = insc;
	}

    override public string ToString()
    {
        return string.Format("Initials {0} | Score: {1} | Wave {2}", initials, score.ToString(), wave.ToString());
    }
}
