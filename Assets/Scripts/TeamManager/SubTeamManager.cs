using System.Collections.Generic;
using UnityEngine;
using AgentScript;
using System.Linq;

[System.Serializable]
public class TeamConfiguration
{
    public int teamId; // Identifiant de l'équipe
    public Transform teamParent; // Objet parent pour l'équipe
    public int minSubTeams = 1; // Nombre minimum de sous-équipes
    public int minUnitsPerSubTeam = 1; // Nombre minimum d'unités par sous-équipe

    [HideInInspector]
    public int subTeamCount = 3; // Nombre de sous-équipes souhaité, non modifiable dans l'inspecteur Unity
}

public class SubTeamManager : MonoBehaviour
{
    public List<TeamConfiguration> teamConfigurations; // Liste des configurations d'équipe

    public static Dictionary<int, Dictionary<int, List<Unit>>> subTeams = new(); // Dictionnaire des sous-équipes par équipe, avec le numéro de chaque sous-équipe comme identifiant de la sous-équipe
    public static Dictionary<int, Unit> subTeamLeaders = new(); // Dictionnaire des leaders de sous-équipes par équipe

    void Update()
    {
        // Vérifie si la touche 'C' est pressée pour créer les sous-équipes
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Key Space pressed. Creating sub-teams...");
            foreach (var config in teamConfigurations)
            {
                CreateSubTeams(config);
            }
        }
    }

    public void CreateSubTeams(TeamConfiguration config)
    {
        // Récupère toutes les unités sous l'objet parent de l'équipe
        List<Unit> units = new List<Unit>();
        foreach (Transform child in config.teamParent)
        {
            Unit unitComponent = child.GetComponent<Unit>();
            if (unitComponent != null)
            {
                units.Add(unitComponent);
            }
        }

        Debug.Log($"Team {config.teamId} has {units.Count} units.");

        // Vérifie si les conditions sont viables pour créer des sous-équipes
        if (units.Count < config.minSubTeams * config.minUnitsPerSubTeam)
        {
            Debug.LogWarning($"Not enough units to create the required sub-teams for team {config.teamId}.");
            return;
        }

        // Efface les sous-équipes existantes pour l'équipe
        if (!subTeams.ContainsKey(config.teamId))
        {
            subTeams[config.teamId] = new();
        }
        else
        {
            subTeams[config.teamId].Clear();
        }

        // Mélange les unités pour les distribuer de manière aléatoire
        units.Shuffle();

        // Calcule le nombre de sous-équipes et distribue les unités
        int unitsPerSubTeam = units.Count / config.subTeamCount;
        int extraUnits = units.Count % config.subTeamCount;

        Debug.Log($"Creating {config.subTeamCount} sub-teams for team {config.teamId} with {unitsPerSubTeam} units per sub-team and {extraUnits} extra units.");
        for (int i = 0; i < config.subTeamCount; i++)
        {
            subTeams[config.teamId].Add(i+1, new List<Unit>());

            // Crée un nouvel objet GameObject pour la sous-équipe
            GameObject subTeamObject = new GameObject($"SubTeam_{config.teamId}_{i + 1}");
            subTeamObject.transform.parent = config.teamParent;
        }

        // Distribue les unités dans les sous-équipes
        int unitIndex = 0;
        for (int i = 0; i < config.subTeamCount; i++)
        {
            int currentSubTeamSize = unitsPerSubTeam + (i < extraUnits ? 1 : 0);
            for (int j = 0; j < currentSubTeamSize; j++)
            {
                subTeams[config.teamId][i+1].Add(units[unitIndex]);

                // Assigne l'unité à l'objet GameObject de la sous-équipe
                units[unitIndex].transform.parent = config.teamParent.Find($"SubTeam_{config.teamId}_{i + 1}");
                unitIndex++;
            }
        }

        // Assigne des leaders pour chaque sous-équipe
        foreach (var (subTeamIndex, subTeam) in subTeams[config.teamId])
        {
            foreach (var unit in subTeam)
            {
                unit.debugName = $"{unit.gameObject.name}(subteam {subTeamIndex})";
            }
            AssignLeader(config.teamId, subTeam);
        }

        // Assigne le leader à chaque unité dans la sous-équipe
            // rajouté dans le code assign leader
        // foreach (var subTeam in subTeams[config.teamId])
        // {
        //     Unit leader = subTeam.Find(unit => unit.isLeader);
        //     foreach (var unit in subTeam)
        //     {
                
        //             unit.leader = leader;
                
        //     }
        // }
    }

    public static List<Unit> GetSubTeam(Unit unit)
    {
        foreach (var (_, subTeam) in subTeams[(int)unit.team])
        {
            if (!(subTeam.Count == 0) && subTeam.First().leader == unit.leader)
            {
                return subTeam;
            }
        }
        Debug.LogWarning("Unit not found in any sub-team.");
        return null;
    }

    public static int GetSubTeamId(Unit unit)
    {
        foreach (var (subTeamId, subTeam) in subTeams[(int)unit.team])
        {
            if (!(subTeam.Count == 0) && subTeam.First().leader == unit.leader)
            {
                return subTeamId;
            }
        }
        Debug.LogWarning("Unit not found in any sub-team.");
        return -1;
    }

    public static void AssignLeader(int teamId, List<Unit> subTeam)
    {
        List<Unit> candidates = new List<Unit>();
        float maxPower = float.MinValue;

        // Trouve les candidats pour le leader en fonction de la puissance
        foreach (Unit unit in subTeam)
        {
            if (unit.gameObject != null)
            {    
                if (unit.GetPower() > maxPower)
                {
                    maxPower = unit.GetPower();
                    candidates.Clear();
                    candidates.Add(unit);
                }
                else if (unit.GetPower() == maxPower)
                {
                    candidates.Add(unit);
                }
            }
        }

        // Assigne un leader parmi les candidats
        if (candidates.Count > 0)
        {
            Unit leader = candidates[Random.Range(0, candidates.Count)];
            leader.isLeader = true;
            leader.gameObject.tag = "Leader"; // Ajoute le tag "Leader"
            subTeamLeaders[teamId] = leader;

            foreach (var unit in subTeam)
            {
                unit.leader = leader;   
            }

            Debug.Log($"Sub Team Leader assigned for team {teamId}, subteam {GetSubTeamId(leader)}: {leader.name}");
        }
        else
        {
            Debug.LogWarning("No leader found for sub-team.");
        }
    }
}

public static class ListExtensions
{
    private static System.Random rng = new System.Random();

    // Méthode pour mélanger une liste
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}