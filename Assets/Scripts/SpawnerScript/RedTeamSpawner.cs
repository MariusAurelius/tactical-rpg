using System;
using System.Collections.Generic;
using UnityEngine;
using AgentScript;

public class RedTeamSpawner : MonoBehaviour
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
                new CharacterComposition { type = typeof(Peasant), count = gameSettings.RedPeasants },
                new CharacterComposition { type = typeof(Warrior), count = gameSettings.RedWarriors },
                new CharacterComposition { type = typeof(Archer), count = gameSettings.RedArchers }
            };

            Debug.Log("Red team composition data loaded successfully.");
        }
        else
        {
            Debug.LogError("Failed to load red team composition data. JSON file not found or invalid.");
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
        int unit_id = 1;
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
                    unitComponent.team = Team.RED; // Team ID for red team
                    unitComponent.id = unit_id;
                    unit_id+=2; // équipe bleue: que des id impairs
                    character.name = $"Red_{composition.type.Name}({unitComponent.id})";
                }

                // Assign the character to the team parent object
                character.transform.parent = teamParent;
                Debug.Log($"Spawned {composition.type.Name} for red team");
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