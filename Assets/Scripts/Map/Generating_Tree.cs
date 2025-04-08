using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratingTree : MonoBehaviour
{
    [Header("Tree Generation Settings")]
    [SerializeField] private GameObject treePrefab; // Le prefab de l'arbre à générer
    [SerializeField] private int numberOfTrees = 50; // Nombre d'arbres à générer
    [SerializeField] private float minSpawnRange = 5f; // Distance minimale par rapport au centre
    [SerializeField] private float maxSpawnRange = 50f; // Distance maximale par rapport au centre

    private Bounds floorBounds;

    void Start()
    {
        // Vérifie si le prefab d'arbre est assigné
        if (treePrefab == null)
        {
            Debug.LogError("Tree prefab is not assigned in the inspector.");
            return;
        }

        // Récupère les dimensions du sol
        Renderer floorRenderer = GetComponent<Renderer>();
        if (floorRenderer != null)
        {
            floorBounds = floorRenderer.bounds;
        }
        else
        {
            Debug.LogError("No Renderer found on the floor object. Cannot determine bounds.");
            return;
        }

        // Génère les arbres
        GenerateTrees();
    }

    private void GenerateTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            // Génère une position aléatoire dans les limites du sol
            Vector3 randomPosition = GetRandomPositionOnFloor();

            // Instancie l'arbre à la position générée
            Instantiate(treePrefab, randomPosition, Quaternion.identity, transform);
        }
    }

    private Vector3 GetRandomPositionOnFloor()
    {
        // Génère des coordonnées X et Z aléatoires dans les limites du sol
        float randomX = Random.Range(floorBounds.min.x, floorBounds.max.x);
        float randomZ = Random.Range(floorBounds.min.z, floorBounds.max.z);

        // Utilise la hauteur (Y) du sol pour positionner l'arbre
        float yPosition = floorBounds.max.y;

        return new Vector3(randomX, yPosition, randomZ);
    }
}