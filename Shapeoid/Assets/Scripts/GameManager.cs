﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private int noteValue = 100;
    private int multiplier;
    private int streak;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerPrefs.SetInt("Score", 0);
        PlayerPrefs.SetInt("Energy", 50);
        Debug.Log(PlayerPrefs.GetInt("Energy"));

        multiplier = 1;
        streak = 0;

        UpdateGUIValues();
    }

    // a method to increase the streak and determine the current multiplier
    public void AddStreak()
    {
        if (PlayerPrefs.GetInt("Energy") + 1 < 50)
        {
            PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") + 1);
            Debug.Log(PlayerPrefs.GetInt("Energy"));
        }

        streak++;

        if (streak >= 24) multiplier = 4;
        else if (streak >= 16) multiplier = 3;
        else if (streak >= 8) multiplier = 2;
        else multiplier = 1;

        UpdateGUIValues();
    }

    // a method to reset the streak
    public void ResetStreak()
    {
        PlayerPrefs.SetInt("Energy", PlayerPrefs.GetInt("Energy") - 2);
        Debug.Log(PlayerPrefs.GetInt("Energy"));

        if (PlayerPrefs.GetInt("Energy") < 0)
        {
            Lose();
        }

        streak = 0;
        multiplier = 1;

        UpdateGUIValues();
    }

    // a method to handle losing
    public void Lose()
    {
        Debug.Log("You suck!");
    }

    // a method to handle winning
    public void Win()
    {
        Debug.Log("You rock!");
    }

    // a method to update the player prefs values
    public void UpdateGUIValues()
    {
        PlayerPrefs.SetInt("Multiplier", multiplier);
        PlayerPrefs.SetInt("Streak", streak);
    }

    // a method to return the points of a hit note
    public int GetScore()
    {
        return noteValue * multiplier;
    }
}
