using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterSpawner : MonoBehaviour
{
    public int teamId;
    public GameObject warriorPrefab;
    public GameObject peasentPrefab;
    public GameObject knightPrefab;
    public Transform spawnPoint;
    public List<CharacterComposition> compositions;
    public Transform teamParent; // Parent object for the team

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

    GameObject GetPrefabByCharacterType(CharacterType characterType)
    {
        switch (characterType)
        {
            case CharacterType.Warrior:
                return warriorPrefab;
            case CharacterType.Peasent:
                return peasentPrefab;
            case CharacterType.Knight:
                return knightPrefab;
            default:
                return null;
        }
    }
}