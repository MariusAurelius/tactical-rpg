using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterComposition
{
    public Type type; // Utilisez System.Type pour le type d'unité
    public int count;
}

[System.Serializable]
public class CompositionData
{
    public List<CharacterComposition> compositions;
}

public class CompositionProvider : MonoBehaviour
{
    private CompositionData compositionData;
    private const string _SAVE_FILENAME = "SavedGameSettings.json";

    void Start()
    {
        LoadCompositionData();
    }

    void LoadCompositionData()
    {
        // Load the game settings from the JSON file
        GameSettings gameSettings = GameSettingsMenu.GetSavedGameSettings();

        if (gameSettings != null)
        {
            // Create the composition data based on the game settings
            compositionData = new CompositionData
            {
                compositions = new List<CharacterComposition>
                {
                    new CharacterComposition { type = typeof(Peasant), count = gameSettings.BluePeasants },
                    new CharacterComposition { type = typeof(Warrior), count = gameSettings.BlueWarriors },
                    new CharacterComposition { type = typeof(Archer), count = gameSettings.BlueArchers }
                }
            };

            Debug.Log("Composition data loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load composition data. JSON file not found or invalid.");
        }
    }
}