using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing all the game settings data to save and load. 
/// </summary>
public class GameSettings
{

    /// <summary>
    /// The combined maximum power of all the troops of this color.
    /// </summary>
    public int BlueMaxPower, RedMaxPower;

    /// <summary>
    /// The number of troops of this type and color to load into the game.
    /// </summary>
    public int BluePeasants, BlueWarriors, BlueArchers, RedPeasants, RedWarriors, RedArchers;

    /// <summary>
    /// Should the number of troops of each type be randomized?
    /// </summary>
    /// <remarks>
    /// If random, the combined power of all the troops will be inferior or equal to the Max Power.
    /// </remarks>
    public bool RandomizeBlueComposition, RandomizeRedComposition;
    
    /// <summary>
    /// The name of the game map to load.
    /// </summary>
    public string MapName;

    /// <summary>
    /// Initializes all the data members.
    /// </summary>
    public GameSettings()
    {
        BlueMaxPower = RedMaxPower = 26;
        BluePeasants = RedPeasants = 3;
        BlueWarriors = RedWarriors = 7;
        BlueArchers = RedArchers = 5;
        RandomizeBlueComposition = RandomizeRedComposition = false;
        MapName = "Plane";
    }

    /// <summary>
    /// Logs all of the fields to the console.
    /// </summary>
    public void Log()
    {
        Debug.Log($"Map: {MapName} | Blue max power: {BlueMaxPower} | Red max power: {RedMaxPower} | Number of blue peasants: {BluePeasants} | Number of blue warriors: {BlueWarriors} | Number of blue archers: {BlueArchers} | Number of red peasants: {RedPeasants} | Number of red warriors: {RedWarriors} | Number of red archers: {RedArchers} | randomize blue composition: {RandomizeBlueComposition} | randomize red composition: {RandomizeRedComposition}");
    }
}