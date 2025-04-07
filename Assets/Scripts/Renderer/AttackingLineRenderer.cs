using UnityEngine;

public class AttackLineRenderer
{
    private LineRenderer lineRenderer;

    public AttackLineRenderer(GameObject owner)
    {
        // Ajoute un LineRenderer au GameObject de l'unité
        lineRenderer = owner.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        lineRenderer.positionCount = 2; // Deux points pour tracer une ligne
        lineRenderer.enabled = false; // Désactivé par défaut
    }

    /// <summary>
    /// Active ou désactive le trait bleu entre l'unité et sa cible.
    /// </summary>
    /// <param name="isActive">`true` pour activer le trait, `false` pour le désactiver.</param>
    /// <param name="startPosition">Position de départ (l'unité).</param>
    /// <param name="endPosition">Position d'arrivée (la cible).</param>
    public void ToggleLine(bool isActive, Vector3 startPosition = default, Vector3 endPosition = default)
    {
        if (lineRenderer == null)
        {
            Debug.LogWarning("LineRenderer is not initialized.");
            return;
        }

        if (isActive)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, startPosition); // Point de départ : position de l'unité
            lineRenderer.SetPosition(1, endPosition);   // Point d'arrivée : position de l'ennemi
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}