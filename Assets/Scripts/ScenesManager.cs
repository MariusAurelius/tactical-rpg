using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance;

    private void Awake() {
        Instance = this;
    }

    public enum Scenes { // have to be in same order as build
        TitleScreen, 
        MainMenu,
        SettingsMenu, 
        PlayMenu,
        LocalGameSettings,
        CardSelectionMenu,
        IndividualCardSelectionMenu,
        Game
    }

    public void Test() {
        Debug.Log("button clicked.");
    }

    public void LoadScene(Scenes scene) {
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadTitleScreen() {
        SceneManager.LoadScene(Scenes.TitleScreen.ToString());
    }

    public void LoadMainMenu() {
        SceneManager.LoadScene(Scenes.MainMenu.ToString());
    }

    public void LoadSettingsMenu() {
        SceneManager.LoadScene(Scenes.SettingsMenu.ToString());
    }
    
    public void LoadPlayMenu() {
        SceneManager.LoadScene(Scenes.PlayMenu.ToString());
    }

    public void LoadLocalGameSettings() {
        SceneManager.LoadScene(Scenes.LocalGameSettings.ToString());
    }

    public void LoadCardSelectionMenu() {
        SceneManager.LoadScene(Scenes.CardSelectionMenu.ToString());
    }

    public void LoadIndividualCardSelectionMenu() {
        SceneManager.LoadScene(Scenes.IndividualCardSelectionMenu.ToString());
    }

    public void LoadGame() {
        SceneManager.LoadScene(Scenes.Game.ToString());
    }

    public void QuitGame() {
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}