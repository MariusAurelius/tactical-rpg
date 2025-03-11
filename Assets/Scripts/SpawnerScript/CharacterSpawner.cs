using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class making the characters spawn characters based on the composition data.
/// </summary> 
public class CharacterSpawner : MonoBehaviour
{
    public int teamId;

    public GameObject warriorPrefab;
    public GameObject peasantPrefab;
    public GameObject archerPrefab;

    public Transform spawnPoint;
    public Transform teamParent; // Parent object for the team

    private List<CharacterComposition> compositions;
    private const string _SAVE_FILENAME = "SavedGameSettings.json";

    void Start()
    {
        LoadCompositionData();
    }

    void LoadCompositionData()
    {
        // Load the game settings from the JSON file
        GameSettings gameSettings = FileHandler.ReadFromJSON<GameSettings>(_SAVE_FILENAME);

        if (gameSettings != null)
        {
            // Assuming you want to load BlueCompositions for this example
            compositions = new List<CharacterComposition>
            {
                new CharacterComposition { type = typeof(Peasant), count = gameSettings.BluePeasants },
                new CharacterComposition { type = typeof(Warrior), count = gameSettings.BlueWarriors },
                new CharacterComposition { type = typeof(Archer), count = gameSettings.BlueArchers }
            };

            Debug.Log("Composition data loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load composition data. JSON file not found or invalid.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnCharacters();
        }
    }

    void SpawnCharacters()
    {
        foreach (var composition in compositions)
        {
            GameObject prefab = GetPrefabByCharacterType(composition.type);
            if (prefab == null)
            {
                Debug.LogError($"Prefab not found for type {composition.type}");
                continue;
            }

            for (int i = 0; i < composition.count; i++)
            {
                GameObject character = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
                Character characterComponent = character.GetComponent<Character>();
                if (characterComponent != null)
                {
                    characterComponent.teamId = teamId;
                }

                // Assign the character to the team parent object
                character.transform.parent = teamParent;
            }
        }
    }

    GameObject GetPrefabByCharacterType(Type characterType)
    {
        if (characterType == typeof(Warrior))
        {
            return warriorPrefab;
        }
        else if (characterType == typeof(Peasant))
        {
            return peasantPrefab;
        }
        else if (characterType == typeof(Archer))
        {
            return archerPrefab;
        }
        else
        {
            return null;
        }
    }
}