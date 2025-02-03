using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all the game settings data to save and load. 
/// </summary>
public class GameSettings
{

    /// <summary>
    /// The number of troops to load into the game.
    /// </summary>
    public int BlueTroops, MinBlueTroops, MaxBlueTroops, RedTroops, MinRedTroops, MaxRedTroops;

    /// <summary>
    /// Should the number of troops be randomized?
    /// </summary>
    public bool RandomizeBlueTroops, RandomizeRedTroops;
    
    /// <summary>
    /// The name of the game map to load.
    /// </summary>
    public string MapName;

    /// <summary>
    /// Initialize all the data members.
    /// </summary>
    public GameSettings()
    {
        BlueTroops = RedTroops = 5;
        MinBlueTroops = MinRedTroops = 3;
        MaxBlueTroops = MaxRedTroops = 7;
        RandomizeBlueTroops = RandomizeRedTroops = false;
        MapName = "Plane";
    }

    /// <summary>
    /// Log all of the fields to the console.
    /// </summary>
    public void Log()
    {
        Debug.Log($"Map: {MapName} | Number of blue troops: {BlueTroops} | min number of blue troops: {MinBlueTroops} | max number of blue troops: {MaxBlueTroops} | Number of red troops: {RedTroops} | min number of red troops: {MinRedTroops} | max number of red troops: {MaxRedTroops} | randomize number of blue troops: {RandomizeBlueTroops} | randomize number of red troops: {RandomizeRedTroops}");
    }
}