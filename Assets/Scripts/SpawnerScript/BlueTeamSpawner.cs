using System;
using System.Collections.Generic;
using UnityEngine;
using AgentScript;

public class BlueTeamSpawner : MonoBehaviour
{
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
            compositions = new List<CharacterComposition>
            {
                new CharacterComposition { type = typeof(Peasant), count = gameSettings.BluePeasants },
                new CharacterComposition { type = typeof(Warrior), count = gameSettings.BlueWarriors },
                new CharacterComposition { type = typeof(Archer), count = gameSettings.BlueArchers }
            };

            Debug.Log("Blue team composition data loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load blue team composition data. JSON file not found or invalid.");
        }
    }

    public void SpawnCharacters()
    {
        int unit_id = 2;
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
                Unit unitComponent = character.GetComponent<Unit>();
                if (unitComponent != null)
                {
                    unitComponent.team = Team.BLUE; // Team ID for blue team
                    unitComponent.id = unit_id;
                    unit_id+=2; // équipe bleue: que des id pairs
                    character.name = $"Blue_{composition.type.Name}({unitComponent.id})";
                }

                // Assign the character to the team parent object
                character.transform.parent = teamParent;
                // Debug.Log($"Spawned {composition.type.Name} for blue team");
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