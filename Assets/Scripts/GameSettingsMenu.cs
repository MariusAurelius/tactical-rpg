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

    private enum TroopColor
    {
        Blue,
        Red
    }

    private enum TroopType
    {
        None,
        Peasant,
        Archer, 
        Warrior
    }

    // UI components to set the game settings

    // blue troops
    [SerializeField] private TMP_Text BlueMaxPowerText;
    [SerializeField] private Slider BlueMaxPowerSlider;

    [SerializeField] private Toggle RandomizeBlueCompositionToggle;

    [SerializeField] private TMP_Text BluePeasantsText;
    [SerializeField] private Slider BluePeasantsSlider;

    [SerializeField] private TMP_Text BlueWarriorsText;
    [SerializeField] private Slider BlueWarriorsSlider;

    [SerializeField] private TMP_Text BlueArchersText;
    [SerializeField] private Slider BlueArchersSlider;


    // red troops
    [SerializeField] private TMP_Text RedMaxPowerText;
    [SerializeField] private Slider RedMaxPowerSlider;

    [SerializeField] private Toggle RandomizeRedCompositionToggle;

    [SerializeField] private TMP_Text RedPeasantsText;
    [SerializeField] private Slider RedPeasantsSlider;

    [SerializeField] private TMP_Text RedWarriorsText;
    [SerializeField] private Slider RedWarriorsSlider;

    [SerializeField] private TMP_Text RedArchersText;
    [SerializeField] private Slider RedArchersSlider;


    [SerializeField] private TMP_Dropdown MapDropdown;

    /// <summary>
    /// The current combined power of all the troops of this color, incremented / decremented when the number of troops of a type is 
    /// incremented / decremented.
    /// </summary>
    private int _bluePower, _redPower;

    /// <summary>
    /// The power of this troop type, to get from this troop's class.
    /// </summary>
    private int _peasantPower, _warriorPower, _archerPower;

    private GameSettings _gameSettings;

    private void Start()
    {
        Debug.Log("starting");
        _peasantPower = 5;
        _archerPower = 7;
        _warriorPower = 10; // replace with class value

        _gameSettings = new();

        Debug.Log("gamesettings created");
        LoadSavedGameSettings();
        Debug.Log("gamesettings loaded");
        UpdateGameSettingsUI();
        Debug.Log("gamesettings ui updated");

        _bluePower = _peasantPower * _gameSettings.BluePeasants + _archerPower * _gameSettings.BlueArchers + _warriorPower * _gameSettings.BlueWarriors;
        _redPower = _peasantPower * _gameSettings.RedPeasants + _archerPower * _gameSettings.RedArchers + _warriorPower * _gameSettings.RedWarriors;

        UpdateNumberOfTroops(TroopColor.Blue);
        UpdateNumberOfTroops(TroopColor.Red);
        Debug.Log("end start");
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


    // functions used to update slider and text values, used by the functions that are called on +/- button click

    /// <summary>
    /// Increases the value of the slider and the text by 1.
    /// </summary>
    /// <param name="slider">Slider to change the value of.</param>
    /// <param name="text">TMP_Text showing the current int value.</param>
    /// <returns><c>True</c> if successfully incremented the value, else <c>False</c>.</returns>
    private bool IncreaseValue(Slider slider, TMP_Text text)
    {
        if (slider.value > slider.maxValue - 1)
        {
            return false;
        }

        slider.SetValueWithoutNotify(slider.value + 1f);

        int value = (int)slider.value;

        text.text = value.ToString();

        return true;
    }

    /// <summary>
    /// Sets the value of the slider and text with <c><paramref name="value"/></c>, without triggering any other functions.
    /// </summary>
    private void SetValue(Slider slider, TMP_Text text, float value) {
        slider.SetValueWithoutNotify(value);
        text.text = value.ToString();

    }

    /// <summary>
    /// Increases the value of the text by 1.
    /// </summary>
    /// <param name="text">TMP_Text showing the current value.</param>
    /// <returns><c>True</c> if successfully incremented the value, else <c>False</c>.</returns>
    private bool IncreaseValue(TMP_Text text)
    {
        string txt = text.text.Trim();
        if (int.TryParse(txt, out int value))
        {
            if (value <= 99) // cap was at 100 : INVALID NOW
            {
                value++;
                text.text = value.ToString();
                return true;
            }
            return false;
        }
        else
        {
            Debug.LogError("Error: the text object did not contain an integer.");
            return false;
        }
    }

    /// <summary>
    /// Decreases the value of the slider and the text by 1.
    /// </summary>
    /// <param name="slider">Slider to change the value of.</param>
    /// <param name="text">TMP_Text showing the current value.</param>
    /// <returns><c>True</c> if successfully decremented the value, else <c>False</c>.</returns>
    private bool DecreaseValue(Slider slider, TMP_Text text)
    {
        if (slider.value < slider.minValue + 1)
        {
            return false;
        }

        slider.SetValueWithoutNotify(slider.value - 1f);

        int value = (int)slider.value;

        text.text = value.ToString();

        return true;
    }

    /// <summary>
    /// Decreases the value of the text by 1.
    /// </summary>
    /// <param name="text">TMP_Text showing the current value.</param>
    /// <returns><c>True</c> if successfully decremented the value, else <c>False</c>.</returns>
    private bool DecreaseValue(TMP_Text text)
    {
        string txt = text.text.Trim();
        if (int.TryParse(txt, out int value))
        {
            if (value >= 1) // NOT NECESSARILLY MIN OF SLIDER
            {
                value--;
                text.text = value.ToString();
                return true;
            }
            return false;
        }
        else
        {
            Debug.LogError("Error: the text object did not contain an integer.");
            return false;
        }
    }


    // change specific value for blue and red max power amount (on +/- button click)

    /// <summary>
    /// Increases the blue max power by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseBlueMaxPower()
    {
        IncreaseValue(BlueMaxPowerSlider, BlueMaxPowerText);
        // UpdateNumberOfTroops(TroopColor.Blue); // only necessary if decreasing troops
    }

    /// <summary>
    /// Decreases the blue max power by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseBlueMaxPower()
    {
        DecreaseValue(BlueMaxPowerSlider, BlueMaxPowerText);
        UpdateNumberOfTroops(TroopColor.Blue);
    }

    /// <summary>
    /// Increases the red max power by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseRedMaxPower()
    {
        IncreaseValue(RedMaxPowerSlider, RedMaxPowerText);
        // UpdateNumberOfTroops(TroopColor.Red); // only necessary if decreasing troops
    }

    /// <summary>
    /// Decreases the red max power by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseRedMaxPower()
    {
        DecreaseValue(RedMaxPowerSlider, RedMaxPowerText);
        UpdateNumberOfTroops(TroopColor.Red);
    }

    /// <summary>
    /// Updates the number of troops of each type if the combined power is superior to the max power, so that the combined power
    /// of all the troops is inferior or equal to the max power.
    /// </summary>
    /// <param name="troopColor">the color of the team to update the troops of.</param>
    /// <param name="troopTypeToIgnore">if given, the troop type to avoid taking away from.</param>
    private void UpdateNumberOfTroops(TroopColor troopColor, TroopType troopTypeToIgnore = TroopType.None)
    {
        switch (troopColor)
        {
            case TroopColor.Blue:
                if (_bluePower > BlueMaxPowerSlider.value)
                {
                    var powerDiff = _bluePower - BlueMaxPowerSlider.value;
                    while (powerDiff > 0)
                    {
                        if (powerDiff >= _warriorPower && troopTypeToIgnore != TroopType.Warrior && BlueWarriorsSlider.value > 0)
                        {
                            DecreaseBlueWarriors();
                            Debug.Log("must decrease warriors");
                        }
                        else if (powerDiff >= _archerPower && troopTypeToIgnore != TroopType.Archer && BlueArchersSlider.value > 0)
                        {
                            DecreaseBlueArchers();
                            Debug.Log("must decrease archers");
                        }
                        else if (troopTypeToIgnore != TroopType.Peasant && BluePeasantsSlider.value > 0)
                        {
                            DecreaseBluePeasants();
                            Debug.Log("must decrease peasants");
                        }
                        if (powerDiff == _bluePower - BlueMaxPowerSlider.value) // blue power hasn't changed: can't decrease anymore 
                                                                                // due to troop type to ignore > decrease it anyways
                        {
                            troopTypeToIgnore = TroopType.None;
                            Debug.Log("undoing troop type to ignore");
                            // break;
                        }
                        powerDiff = _bluePower - BlueMaxPowerSlider.value;
                    }
                }
                break;

            case TroopColor.Red:
                if (_redPower > RedMaxPowerSlider.value)
                {
                    var powerDiff = _redPower - RedMaxPowerSlider.value;
                    while (powerDiff > 0)
                    {
                        if (powerDiff >= _warriorPower && troopTypeToIgnore != TroopType.Warrior && RedWarriorsSlider.value > 0)
                        {
                            DecreaseRedWarriors();
                            Debug.Log("must decrease warriors");
                        }
                        else if (powerDiff >= _archerPower && troopTypeToIgnore != TroopType.Archer && RedArchersSlider.value > 0)
                        {
                            DecreaseRedArchers();
                            Debug.Log("must decrease archers");
                        }
                        else if (troopTypeToIgnore != TroopType.Peasant && RedPeasantsSlider.value > 0)
                        {
                            DecreaseRedPeasants();
                            Debug.Log("must decrease peasants");
                        }
                        if (powerDiff == _redPower - RedMaxPowerSlider.value) // red power hasn't changed: can't decrease anymore 
                                                                                // due to troop type to ignore > decrease it anyways
                        {
                            troopTypeToIgnore = TroopType.None;
                            // break;
                            Debug.Log("undoing troop type to ignore");
                        }
                        powerDiff = _redPower - RedMaxPowerSlider.value;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Updates the value of <c>_bluePower</c> based on the slider values.
    /// </summary>
    private void UpdateBluePower() {
        _bluePower =(int) (_peasantPower * BluePeasantsSlider.value + _archerPower * BlueArchersSlider.value + _warriorPower * BlueWarriorsSlider.value);
    }

    /// <summary>
    /// Updates the value of <c>_redPower</c> based on the slider values.
    /// </summary>
    private void UpdateRedPower() {
        _redPower =(int) (_peasantPower * RedPeasantsSlider.value + _archerPower * RedArchersSlider.value + _warriorPower * RedWarriorsSlider.value);
    }

    // change specific value for blue and red troops amount (on +/- button click)

    /// <summary>
    /// Increases the number of blue peasants by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseBluePeasants()
    {
        if (_bluePower + _peasantPower <= BlueMaxPowerSlider.value)
        {
            var increased = IncreaseValue(BluePeasantsSlider, BluePeasantsText);
            if (increased)
            {
                _bluePower += _peasantPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of blue peasants by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseBluePeasants()
    {
        var decreased = DecreaseValue(BluePeasantsSlider, BluePeasantsText);
        if (decreased)
        {
            _bluePower -= _peasantPower;
        }
    }

    /// <summary>
    /// Increases the number of red peasants by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseRedPeasants()
    {
        if (_redPower + _peasantPower <= RedMaxPowerSlider.value)
        {
            var increased = IncreaseValue(RedPeasantsSlider, RedPeasantsText);
            if (increased)
            {
                _redPower += _peasantPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of red peasants by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseRedPeasants()
    {
        var decreased = DecreaseValue(RedPeasantsSlider, RedPeasantsText);
        if (decreased)
        {
            _redPower -= _peasantPower;
        }
    }

    /// <summary>
    /// Increases the number of blue warriors by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseBlueWarriors()
    {
        if (_bluePower + _warriorPower <= BlueMaxPowerSlider.value)
        {
            var increased = IncreaseValue(BlueWarriorsSlider, BlueWarriorsText);
            if (increased)
            {
                _bluePower += _warriorPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of blue warriors by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseBlueWarriors()
    {
        var decreased = DecreaseValue(BlueWarriorsSlider, BlueWarriorsText);
        if (decreased)
        {
            _bluePower -= _warriorPower;
        }
    }

    /// <summary>
    /// Increases the number of red warriors by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseRedWarriors()
    {
        if (_redPower + _warriorPower <= RedMaxPowerSlider.value)
        {
            var increased = IncreaseValue(RedWarriorsSlider, RedWarriorsText);
            if (increased)
            {
                _redPower += _warriorPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of red warriors by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseRedWarriors()
    {
        var decreased = DecreaseValue(RedWarriorsSlider, RedWarriorsText);
        if (decreased)
        {
            _redPower -= _warriorPower;
        }
    }

    /// <summary>
    /// Increases the number of blue archers by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseBlueArchers()
    {
        if (_bluePower + _archerPower <= BlueMaxPowerSlider.value)
        {
            var increased = IncreaseValue(BlueArchersSlider, BlueArchersText);
            if (increased)
            {
                _bluePower += _archerPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of blue archers by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseBlueArchers()
    {
        var decreased = DecreaseValue(BlueArchersSlider, BlueArchersText);
        if (decreased)
        {
            _bluePower -= _archerPower;
        }
    }

    /// <summary>
    /// Increases the number of red archers by 1, and updates the corresponding UI.
    /// </summary>
    public void IncreaseRedArchers()
    {
        if (_redPower + _archerPower <= RedMaxPowerSlider.value)
        {
            var increased = IncreaseValue(RedArchersSlider, RedArchersText);
            if (increased)
            {
                _redPower += _archerPower;
            }
        }
    }

    /// <summary>
    /// Decreases the number of red archers by 1, and updates the corresponding UI.
    /// </summary>
    public void DecreaseRedArchers()
    {
        var decreased = DecreaseValue(RedArchersSlider, RedArchersText);
        if (decreased)
        {
            _redPower -= _archerPower;
        }
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

    
    // update values via slider

    /// <summary>
    /// Updates the blue max power and its text via the slider.
    /// </summary>
    public void UpdateBlueMaxPower(float blueMaxPower)
    {
        UpdateNumberOfTroops(TroopColor.Blue);
        BlueMaxPowerText.text = ((int)blueMaxPower).ToString();
    }

    /// <summary>
    /// Updates the red max power and its text via the slider.
    /// </summary>
    public void UpdateRedMaxPower(float redMaxPower)
    {
        UpdateNumberOfTroops(TroopColor.Red);
        RedMaxPowerText.text = ((int)redMaxPower).ToString();
    }

    /// <summary>
    /// Updates the amount of blue peasants and its text via the slider.
    /// </summary>
    public void UpdateBluePeasants(float bluePeasants)
    {
        UpdateBluePower();
        BluePeasantsText.text = ((int)bluePeasants).ToString();
        UpdateNumberOfTroops(TroopColor.Blue, TroopType.Peasant);
    }

    /// <summary>
    /// Updates the amount of red peasants and its text via the slider.
    /// </summary>
    public void UpdateRedPeasants(float redPeasants)
    {
        UpdateRedPower();
        RedPeasantsText.text = ((int)redPeasants).ToString();
        UpdateNumberOfTroops(TroopColor.Red, TroopType.Peasant);
    }

    /// <summary>
    /// Updates the amount of blue archers and its text via the slider.
    /// </summary>
    public void UpdateBlueArchers(float blueArchers)
    {
        UpdateBluePower();
        BlueArchersText.text = ((int)blueArchers).ToString();
        UpdateNumberOfTroops(TroopColor.Blue, TroopType.Archer);
    }

    /// <summary>
    /// Updates the amount of red archers and its text via the slider.
    /// </summary>
    public void UpdateRedArchers(float redArchers)
    {
        UpdateRedPower();
        RedArchersText.text = ((int)redArchers).ToString();
        UpdateNumberOfTroops(TroopColor.Red, TroopType.Archer);
    }

    /// <summary>
    /// Updates the amount of blue warriors and its text via the slider.
    /// </summary>
    public void UpdateBlueWarriors(float blueWarriors)
    {
        UpdateBluePower();
        BlueWarriorsText.text = ((int)blueWarriors).ToString();
        UpdateNumberOfTroops(TroopColor.Blue, TroopType.Warrior);
    }

    /// <summary>
    /// Updates the amount of red warriors and its text via the slider.
    /// </summary>
    public void UpdateRedWarriors(float redWarriors)
    {
        UpdateRedPower();
        RedWarriorsText.text = ((int)redWarriors).ToString();
        UpdateNumberOfTroops(TroopColor.Red, TroopType.Warrior);
    }


    /// <summary>
    /// Called when the 'Randomize blue composition' checkbox is toggled: randomizes the number of blue peasants, warriors, and archers.
    /// </summary>
    /// <param name="randomize">The checkbox state after toggling.</param>
    public void OnRandomizeBlueCompositionToggle(bool randomize)
    {
        if (randomize == true)
        {
            // randomize # of blue peasants, warriors, and archers
        }
    }

    /// <summary>
    /// Called when the 'Randomize red composition' checkbox is toggled: randomizes the number of red peasants, warriors, and archers.
    /// </summary>
    /// <param name="randomize">The checkbox state after toggling.</param>
    public void OnRandomizeRedCompositionToggle(bool randomize)
    {
        if (randomize == true)
        {
            // randomize # of red peasants, warriors, and archers
        }
    }


    // functions for getting the values from the UI components

    public int GetBlueMaxPower()
    {
        return (int)BlueMaxPowerSlider.value;
    }

    public int GetRedMaxPower()
    {
        return (int)RedMaxPowerSlider.value;
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

    public int GetBluePeasants()
    {
        return (int)BluePeasantsSlider.value;
    }

    public int GetBlueArchers()
    {
        return (int)BlueArchersSlider.value;
    }

    public int GetBlueWarriors()
    {
        return (int)BlueWarriorsSlider.value;
    }

    public int GetRedPeasants()
    {
        return (int)RedPeasantsSlider.value;
    }

    public int GetRedArchers()
    {
        return (int)RedArchersSlider.value;
    }
    
    public int GetRedWarriors()
    {
        return (int)RedWarriorsSlider.value;
    }

    public bool GetRandomizeBlueComposition()
    {
        return RandomizeBlueCompositionToggle.isOn;
    }

    public bool GetRandomizeRedComposition()
    {
        return RandomizeRedCompositionToggle.isOn;
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

        _gameSettings.BlueMaxPower = GetBlueMaxPower();
        _gameSettings.BluePeasants = GetBluePeasants();
        _gameSettings.BlueArchers = GetBlueArchers();
        _gameSettings.BlueWarriors = GetBlueWarriors();
        _gameSettings.RandomizeBlueComposition = GetRandomizeBlueComposition();

        _gameSettings.RedMaxPower = GetRedMaxPower();
        _gameSettings.RedPeasants = GetRedPeasants();
        _gameSettings.RedArchers = GetRedArchers();
        _gameSettings.RedWarriors = GetRedWarriors();
        _gameSettings.RandomizeRedComposition = GetRandomizeRedComposition();
        
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

    private void SetBlueMaxPowerUI()
    {
        SetValue(BlueMaxPowerSlider, BlueMaxPowerText, _gameSettings.BlueMaxPower);
    }

    private void SetRedMaxPowerUI()
    {
        SetValue(RedMaxPowerSlider, RedMaxPowerText, _gameSettings.RedMaxPower);
    }

    private void SetBluePeasantsUI()
    {
        SetValue(BluePeasantsSlider, BluePeasantsText, _gameSettings.BluePeasants);
    }

    private void SetRedPeasantsUI()
    {
        SetValue(RedPeasantsSlider, RedPeasantsText, _gameSettings.RedPeasants);
    }

    private void SetBlueArchersUI()
    {
        SetValue(BlueArchersSlider, BlueArchersText, _gameSettings.BlueArchers);
    }

    private void SetRedArchersUI()
    {
        SetValue(RedArchersSlider, RedArchersText, _gameSettings.RedArchers);
    }

    private void SetBlueWarriorsUI()
    {
        SetValue(BlueWarriorsSlider, BlueWarriorsText, _gameSettings.BlueWarriors);
    }

    private void SetRedWarriorsUI()
    {
        SetValue(RedWarriorsSlider, RedWarriorsText, _gameSettings.RedWarriors);
    }

    /// <summary>
    /// Updates the UI elements of the Game Settings Menu scene with the loaded game settings.
    /// </summary>
    private void UpdateGameSettingsUI()
    {
        if (_gameSettings != null)
        {
            _gameSettings.Log();
            
            SetBlueMaxPowerUI();
            SetBluePeasantsUI();
            SetBlueArchersUI();
            SetBlueWarriorsUI();
            RandomizeBlueCompositionToggle.isOn = _gameSettings.RandomizeBlueComposition;

            SetRedMaxPowerUI();
            SetRedPeasantsUI();
            SetRedArchersUI();
            SetRedWarriorsUI();
            RandomizeRedCompositionToggle.isOn = _gameSettings.RandomizeRedComposition;

            MapDropdown.value = MapDropdown.options.FindIndex(option => option.text == _gameSettings.MapName);
        }
    }

}