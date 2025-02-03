using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour handling the GameSettingsMenu scene.
/// </summary>
public class GameSettingsMenu : MonoBehaviour
{

    /// <summary>
    /// The name of the JSON file containing the saved game settings to load.
    /// </summary>
    private const string _SAVE_FILENAME = "SavedGameSettings.json";

    // POUR L'INSTANT CETTE SCENE N'A QUE LE NOMBRE DE TROUPES ET PAS LE NOMBRES DE TROUPES PAR TYPE D'UNITE. 

    private enum TroopColor
    {
        Blue,
        Red
    }

    // UI components to set the game settings

    // blue troops
    [SerializeField] private TMP_Text BlueTroopsText;
    [SerializeField] private Slider BlueTroopsSlider;
    [SerializeField] private Toggle RandomizeBlueTroopsToggle;
    [SerializeField] private TMP_Text MinBlueTroopsText;
    [SerializeField] private TMP_Text MaxBlueTroopsText;

    // red troops
    [SerializeField] private TMP_Text RedTroopsText;
    [SerializeField] private Slider RedTroopsSlider;
    [SerializeField] private Toggle RandomizeRedTroopsToggle;
    [SerializeField] private TMP_Text MinRedTroopsText;
    [SerializeField] private TMP_Text MaxRedTroopsText;


    // Slider colors : when toggling Randomize number of troops, the text and slider will toggle to gray or back to their original color. 
    private Color originalBlueTroopsSliderHandleColor;
    private Color originalBlueTroopsSliderFillColor;

    private Color originalRedTroopsSliderHandleColor;
    private Color originalRedTroopsSliderFillColor;


    [SerializeField] private TMP_Dropdown MapDropdown;

    private GameSettings _gameSettings;

    private void Start()
    {
        _gameSettings = new();

        originalBlueTroopsSliderHandleColor = GetSliderHandleColor(BlueTroopsSlider);
        originalBlueTroopsSliderFillColor = GetSliderFillColor(BlueTroopsSlider);
        originalRedTroopsSliderHandleColor = GetSliderHandleColor(RedTroopsSlider);
        originalRedTroopsSliderFillColor = GetSliderFillColor(RedTroopsSlider);
        LoadSavedGameSettings();
        UpdateGameSettingsUI();

    }

    /// <summary>
    /// Called on scene exit to update the game settings before object destruction.
    /// </summary>
    public void ExitScene()
    {
        UpdateGameSettings(); // Update before transitioning
        SaveGameSettings();      // Save changes
        ScenesManager.Instance.LoadMainMenu();
    }

    public Color GetSliderFillColor(Slider slider)
    {
        Image fillImage = slider.fillRect.GetComponent<Image>();
        return fillImage.color;
    }

    public Color GetSliderHandleColor(Slider slider)
    {
        return slider.colors.normalColor;
    }

    private void SetSliderFillColor(Slider slider, Color newColor)
    {
        Image fillImage = slider.fillRect.GetComponent<Image>();
        fillImage.color = newColor;
    }

    private void SetSliderHandleColor(Slider slider, Color newColor)
    {
        var sliderColors = slider.colors;
        sliderColors.normalColor = newColor;
        slider.colors = sliderColors;
    }


    // functions used to update slider and text values, used by the functions that are called on +/- button click

    /// <summary>
    /// Increases the value of the slider and the text by 1.
    /// </summary>
    /// <param name="slider">Slider to change the value of.</param>
    /// <param name="text">TMP_Text showing the current int value.</param>
    private void IncreaseValue(Slider slider, TMP_Text text)
    {
        if (slider.value > 49) // cap at 50
        {
            return;
        }

        slider.value += 1f;

        int value = (int)slider.value;

        text.text = value.ToString();
    }

    /// <summary>
    /// Increases the value of the text by 1.
    /// </summary>
    /// <param name="text">TMP_Text showing the current value.</param>
    private void IncreaseValue(TMP_Text text)
    {
        string txt = text.text.Trim();
        if (int.TryParse(txt, out int value))
        {
            if (value <= 49)
            {
                value++;
            }
            text.text = value.ToString();
        }
        else
        {
            Debug.LogError("Error: the text object did not contain an integer.");
        }
    }

    /// <summary>
    /// Decreases the value of the slider and the text by 1.
    /// </summary>
    /// <param name="slider">Slider to change the value of.</param>
    /// <param name="text">TMP_Text showing the current value.</param>
    private void DecreaseValue(Slider slider, TMP_Text text)
    {
        if (slider.value < 1)
        {
            return;
        }

        slider.value -= 1f;

        int value = (int)slider.value;

        text.text = value.ToString();
    }

    /// <summary>
    /// Decreases the value of the text by 1.
    /// </summary>
    /// <param name="text">TMP_Text showing the current value.</param>
    private void DecreaseValue(TMP_Text text)
    {
        string txt = text.text.Trim();
        if (int.TryParse(txt, out int value))
        {
            if (value >= 1)
            {
                value--;
            }
            text.text = value.ToString();
        }
        else
        {
            Debug.LogError("Error: the text object did not contain an integer.");
        }
    }


    // change specific value for blue and red troops amount (on +/- button click)

    /// <summary>
    /// Increases the number of blue troops by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseBlueTroops()
    {
        IncreaseValue(BlueTroopsSlider, BlueTroopsText);
    }

    /// <summary>
    /// Decreases the number of blue troops by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseBlueTroops()
    {
        DecreaseValue(BlueTroopsSlider, BlueTroopsText);
    }

    /// <summary>
    /// Increases the number of red troops by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseRedTroops()
    {
        IncreaseValue(RedTroopsSlider, RedTroopsText);
    }

    /// <summary>
    /// Decreases the number of red troops by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseRedTroops()
    {
        DecreaseValue(RedTroopsSlider, RedTroopsText);
    }


    // function used for min and max change

    /// <summary>
    /// Checks if a string <c><paramref name="a"/></c> is inferior or equal to a string <c><paramref name="b"/></c>, where 
    /// both strings contain an integer. 
    /// </summary>
    /// <param name="a">String containing only digits</param>
    /// <param name="b">String containing only digits</param>
    public bool IsInferiorEqualTo(string a, string b)
    {
        a = a.Trim();
        b = b.Trim();
        if (int.TryParse(a, out int a_int) && int.TryParse(b, out int b_int))
        {
            return a_int <= b_int;
        }
        Debug.LogError("Error: one of the 2 strings did not contain an integer.");
        return false;
    }


    // change Min and Max Blue Troops (on +/- button click)

    public void IncreaseMinBlueTroops()
    {
        if (IsInferiorEqualTo(MaxBlueTroopsText.text, MinBlueTroopsText.text)) // cant increase min if min >= max <=> max <= min
        {
            return;
        }
        IncreaseValue(MinBlueTroopsText);
    }

    public void DecreaseMinBlueTroops()
    {
        DecreaseValue(MinBlueTroopsText);
    }

    public void IncreaseMaxBlueTroops()
    {
        IncreaseValue(MaxBlueTroopsText);
    }

    public void DecreaseMaxBlueTroops()
    {
        if (IsInferiorEqualTo(MaxBlueTroopsText.text, MinBlueTroopsText.text)) // cant decrease if max <= min
        {
            return;
        }
        DecreaseValue(MaxBlueTroopsText);
    }


    // change Min and Max Red Troops (on +/- button click)

    public void IncreaseMinRedTroops()
    {
        if (IsInferiorEqualTo(MaxRedTroopsText.text, MinRedTroopsText.text)) // cant increase min if min >= max <=> max <= min
        {
            return;
        }
        IncreaseValue(MinRedTroopsText);
    }

    public void DecreaseMinRedTroops()
    {
        DecreaseValue(MinRedTroopsText);
    }

    public void IncreaseMaxRedTroops()
    {
        IncreaseValue(MaxRedTroopsText);
    }

    public void DecreaseMaxRedTroops()
    {
        if (IsInferiorEqualTo(MaxRedTroopsText.text, MinRedTroopsText.text)) // cant decrease if max <= min
        {
            return;
        }
        DecreaseValue(MaxRedTroopsText);
    }


    // update number of troops text via slider

    /// <summary>
    /// Updates the number of blue troops text via the slider.
    /// </summary>
    public void UpdateBlueTroopsText(float blueTroops)
    {
        BlueTroopsText.text = ((int)blueTroops).ToString();
    }

    /// <summary>
    /// Updates the number of red troops text via the slider.
    /// </summary>
    public void UpdateRedTroopsText(float redTroops)
    {
        RedTroopsText.text = ((int)redTroops).ToString();
    }


    // change UI appearance based on whether randomize is toggled on/off

    /// <summary>
    /// "Minimizes" the slider and text appearance by changing their colors to gray, OR the Min and Max texts 
    /// if <c><paramref name="randomize"/></c> is true.
    /// </summary>
    /// <param name="troopColor">TroopColor.Blue or TroopColor.Red</param>
    /// <param name="randomize"> True if the 'randomize' part must be minimized, else false.</param>
    private void MinimizeObjectAppearance(TroopColor troopColor, bool randomize)
    {
        if (troopColor == TroopColor.Blue)
        {
            if (randomize) // randomize part of BlueTroops to be minimized
            {
                MinBlueTroopsText.color = Color.gray;
                MaxBlueTroopsText.color = Color.gray;
            }
            else // specific value part (text and slider) to be minimized: apply gray filter to slider and text
            {
                SetSliderHandleColor(BlueTroopsSlider, Color.gray);
                SetSliderFillColor(BlueTroopsSlider, Color.gray);

                BlueTroopsText.color = Color.gray;
            }
        }
        else if (troopColor == TroopColor.Red)
        {
            if (randomize) // randomize part of RedTroops to be minimized
            {
                MinRedTroopsText.color = Color.gray;
                MaxRedTroopsText.color = Color.gray;
            }
            else // specific value part (text and slider) to be minimized: apply gray filter to slider and text
            {
                SetSliderHandleColor(RedTroopsSlider, Color.gray);
                SetSliderFillColor(RedTroopsSlider, Color.gray);

                RedTroopsText.color = Color.gray;
            }
        }
        else
        {
            Debug.LogWarning("Wrong parameter passed");
        }
    }

    /// <summary>
    /// "Maximizes" the slider and text appearance by reverting their colors back to their original ones, OR the Min and Max texts 
    /// if <c><paramref name="randomize"/></c> is true.
    /// </summary>
    /// <param name="troopColor">TroopColor.Blue or TroopColor.Red</param>
    /// <param name="randomize"> True if the 'randomize' part must be maximized, else false.</param>
    private void MaximizeObjectAppearance(TroopColor troopColor, bool randomize)
    {
        if (troopColor == TroopColor.Blue)
        {
            if (randomize) // randomize part of BlueTroops to be maximized
            {
                MinBlueTroopsText.color = Color.white;
                MaxBlueTroopsText.color = Color.white;
            }
            else // specific value part (text and slider) to be maximized: restore original colors
            {
                SetSliderHandleColor(BlueTroopsSlider, originalBlueTroopsSliderHandleColor);
                SetSliderFillColor(BlueTroopsSlider, originalBlueTroopsSliderFillColor);

                BlueTroopsText.color = Color.white;
            }
        }
        else if (troopColor == TroopColor.Red)
        {

            if (randomize) // randomize part of RedTroops to be maximized
            {
                MinRedTroopsText.color = Color.white;
                MaxRedTroopsText.color = Color.white;
            }
            else // specific value part (text and slider) to be maximized: restore original colors
            {
                SetSliderHandleColor(RedTroopsSlider, originalRedTroopsSliderHandleColor);
                SetSliderFillColor(RedTroopsSlider, originalRedTroopsSliderFillColor);

                RedTroopsText.color = Color.white;
            }
        }
        else
        {
            Debug.LogWarning("Wrong parameter passed");
        }
    }

    /// <summary>
    /// Called when the 'Randomize blue troops' checkbox is toggled: minimizes / maximizes the appropriate UI elements.
    /// </summary>
    /// <param name="randomize">The checkbox state after toggling.</param>
    public void OnRandomizeBlueTroopsToggle(bool randomize)
    {
        if (randomize == true) // minimize the specific value UI elements, maximize the random value UI elements.
        {
            MinimizeObjectAppearance(TroopColor.Blue, false);
            MaximizeObjectAppearance(TroopColor.Blue, true);
        }
        else // maximize the specific value UI elements, minimize the random value UI elements.
        {
            MaximizeObjectAppearance(TroopColor.Blue, false);
            MinimizeObjectAppearance(TroopColor.Blue, true);
        }
    }

    /// <summary>
    /// Called when the 'Randomize red troops' checkbox is toggled: minimizes / maximizes the appropriate UI elements.
    /// </summary>
    /// <param name="randomize">The checkbox state after toggling.</param>
    public void OnRandomizeRedTroopsToggle(bool randomize)
    {
        if (randomize == true) // minimize the specific value UI elements, maximize the random value UI elements.
        {
            MinimizeObjectAppearance(TroopColor.Red, false);
            MaximizeObjectAppearance(TroopColor.Red, true);
        }
        else // maximize the specific value UI elements, minimize the random value UI elements.
        {
            MaximizeObjectAppearance(TroopColor.Red, false);
            MinimizeObjectAppearance(TroopColor.Red, true);
        }
    }


    // functions for getting the values from the UI components

    public int GetBlueTroops()
    {
        return (int)BlueTroopsSlider.value;
    }

    public int GetRedTroops()
    {
        return (int)RedTroopsSlider.value;
    }

    /// <summary>
    /// Gets the integer from the string.
    /// </summary>
    /// <param name="text">string containing only digits.</param>
    /// <param name="default_value">The default value to return.</param> 
    public int GetIntFromText(string text, int default_value)
    {
        text = text.Trim();
        if (int.TryParse(text, out int integer))
        {
            return integer;
        }
        else
        {
            Debug.LogError("Error: the string did not contain an integer.");
            return default_value;
        }
    }

    public int GetMinBlueTroops()
    {
        return GetIntFromText(MinBlueTroopsText.text, 3);
    }

    public int GetMaxBlueTroops()
    {
        return GetIntFromText(MaxBlueTroopsText.text, 7);
    }

    public int GetMinRedTroops()
    {
        return GetIntFromText(MinRedTroopsText.text, 3);
    }

    public int GetMaxRedTroops()
    {
        return GetIntFromText(MaxRedTroopsText.text, 7);
    }

    public bool GetRandomizeBlueTroops()
    {
        return RandomizeBlueTroopsToggle.isOn;
    }

    public bool GetRandomizeRedTroops()
    {
        return RandomizeRedTroopsToggle.isOn;
    }

    public string GetMapName()
    {
        return MapDropdown.options[MapDropdown.value].text;
    }


    // functions for saving and loading game settings to and from the JSON file

    /// <summary>
    /// Updates the game settings data members based on the value of each corresponding UI element.
    /// </summary>
    private void UpdateGameSettings()
    {
        _gameSettings ??= new(); // same as - if game settings == null: game settings = new()

        _gameSettings.BlueTroops = GetBlueTroops();
        _gameSettings.MinBlueTroops = GetMinBlueTroops();
        _gameSettings.MaxBlueTroops = GetMaxBlueTroops();
        _gameSettings.RedTroops = GetRedTroops();
        _gameSettings.MinRedTroops = GetMinRedTroops();
        _gameSettings.MaxRedTroops = GetMaxRedTroops();
        _gameSettings.RandomizeBlueTroops = GetRandomizeBlueTroops();
        _gameSettings.RandomizeRedTroops = GetRandomizeRedTroops();
        _gameSettings.MapName = GetMapName();
    }

    /// <summary>
    /// Saves the game settings to the JSON file.
    /// </summary>
    private void SaveGameSettings()
    {
        FileHandler.SaveToJSON<GameSettings>(_gameSettings, _SAVE_FILENAME);
        Debug.Log("Saving game settings: ");
        _gameSettings.Log();
    }

    /// <summary>
    /// Loads the saved game settings from the JSON file into the <c>_gameSettings</c> field.
    /// </summary>
    private void LoadSavedGameSettings()
    {
        _gameSettings = FileHandler.ReadFromJSON<GameSettings>(_SAVE_FILENAME);
        if (_gameSettings != null)
        {
            Debug.Log("Saved game settings loaded: ");
            _gameSettings.Log();
        }
        else
        {
            _gameSettings = new();
        }
    }

    /// <summary>
    /// Returns the saved game settings from the JSON file.
    /// </summary>
    public static GameSettings GetSavedGameSettings()
    {
        GameSettings gameSettings = FileHandler.ReadFromJSON<GameSettings>(_SAVE_FILENAME);
        if (gameSettings == null)
        {
            Debug.LogWarning("Saved game settings are null, returning default game settings instead. ");
            return new();
        }
        return gameSettings;
    }


    // functions for setting UI components' values from the saved game settings

    private void SetBlueTroopsUI()
    {
        BlueTroopsSlider.value = _gameSettings.BlueTroops;
        BlueTroopsText.text = _gameSettings.BlueTroops.ToString();
    }

    private void SetRedTroopsUI()
    {
        RedTroopsSlider.value = _gameSettings.RedTroops;
        RedTroopsText.text = _gameSettings.RedTroops.ToString();
    }


    /// <summary>
    /// Updates the UI elements of the Game Settings Menu scene with the loaded game settings.
    /// </summary>
    private void UpdateGameSettingsUI()
    {
        if (_gameSettings != null)
        {
            _gameSettings.Log();
            SetBlueTroopsUI();
            MinBlueTroopsText.text = _gameSettings.MinBlueTroops.ToString();
            MaxBlueTroopsText.text = _gameSettings.MaxBlueTroops.ToString();

            SetRedTroopsUI();
            MinRedTroopsText.text = _gameSettings.MinRedTroops.ToString();
            MaxRedTroopsText.text = _gameSettings.MaxRedTroops.ToString();

            RandomizeBlueTroopsToggle.isOn = _gameSettings.RandomizeBlueTroops;
            RandomizeRedTroopsToggle.isOn = _gameSettings.RandomizeRedTroops;

            MapDropdown.value = MapDropdown.options.FindIndex(option => option.text == _gameSettings.MapName);
        }
    }

}