using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class handling scene loading.
/// </summary>
public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public enum Scenes
    { // have to be in the same order as build settings
        TitleScreen,
        MainMenu,
        GeneralSettingsMenu,
        GameSettingsMenu,
        Maze,
        Jungle
    }

    public void LoadScene(Scenes? scene)
    {
        if (scene == null)
        {
            Debug.LogWarning("Scene is null. Cannot load scene.");
            return;
        }
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadTitleScreen()
    {
        SceneManager.LoadScene(Scenes.TitleScreen.ToString());
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(Scenes.MainMenu.ToString());
    }

    public void LoadGeneralSettingsMenu()
    {
        SceneManager.LoadScene(Scenes.GeneralSettingsMenu.ToString());
    }

    public void LoadGameSettingsMenu()
    {
        SceneManager.LoadScene(Scenes.GameSettingsMenu.ToString());
    }

    /// <summary>
    /// Loads the game scene based on the saved game settings.
    /// </summary>
    public void LoadGame()
    {
        GameSettings gameSettings = GameSettingsMenu.GetSavedGameSettings();
        string mapScene = gameSettings.MapName;

        if (Application.CanStreamedLevelBeLoaded(mapScene))
        {
            SceneManager.LoadScene(mapScene);
        }
        else
        {
            Debug.LogWarning($"Scene '{mapScene}' does not exist. Loading default scene 'Maze'.");
            SceneManager.LoadScene("Maze");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    /// <summary>
    /// Gets the current scene as a Scenes enum value.
    /// </summary>
    public Scenes? GetCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (Enum.TryParse(sceneName, out Scenes sceneEnum))
        {
            return sceneEnum;
        }
        else
        {
            Debug.LogWarning($"current scene '{sceneName}' not in the Scenes enum.");
            return null;
        }
    }
}