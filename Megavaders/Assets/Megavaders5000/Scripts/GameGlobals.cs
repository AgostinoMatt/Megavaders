using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Convenience class for storing globally reachable data
public class GameGlobals 
{
    // Constraints for player and enemy movement in the horizontal direction
    public static float SCREEN_CONSTRAINT_X = 7.5f;

    // The lowest y (2d oriented) world position an enemy may end up before it is considered "game over"
    public static float ENEMY_LOWEST_POINT = -5.0f;

    // The maximum number of missiles a wave of enemies will drop at any given time
    public static int MAX_ENEMY_MISSILES = 10;

    // Y Extent the beyond which a missile will be destroyed/despawned automatically
    public static float MISSILE_CONSTRAINT_Y = 5.5f;


    // Player Session Data; moved from the Play Scene to the Game Over Scene
    public static int Wave = 0; // Player's wave when Game Over hit
    public static int Score = 0; //  Player's Score when Game Over Hit

    public static void SetSessionData(int wave, int score)
    {
        Wave = wave;
        Score = score;
    }
    



}
