using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// Class to define the composition of characters
public class CharacterComposition
{
    public CharacterType type;
    public int count;
}

[System.Serializable]
// Class to define the composition of a team
public class CompositionData
{
    public List<CharacterComposition> compositions;
}


public class CompositionProvider : MonoBehaviour
{
    public CompositionData compositionData;

    void Start()
    {
        LoadCompositionData();
    }

    void LoadCompositionData()
    {
        // Directly define the composition data here
        compositionData = new CompositionData
        {
            compositions = new List<CharacterComposition>
            {
                new CharacterComposition { type = CharacterType.Warrior, count = 5 },
                new CharacterComposition { type = CharacterType.Peasent, count = 3 },
                new CharacterComposition { type = CharacterType.Knight, count = 2 }
            }
        };
    }
}