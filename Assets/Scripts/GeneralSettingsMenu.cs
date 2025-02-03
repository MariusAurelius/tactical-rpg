using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// MonoBehaviour handling the GeneralSettingsMenu scene.
/// </summary>
public class GeneralSettingsMenu : MonoBehaviour
{
    public AudioMixer MainAudioMixer;
    public Slider VolumeSlider;
    public TMP_Dropdown GraphicsDropdown;
    public TMP_Dropdown ResolutionDropdown;
    private Resolution[] _resolutions;

    public Toggle FullscreenToggle;

    /// <summary>
    /// Initializes the visual gameobjects of the scene. 
    /// </summary>
    private void Start()
    {
        _resolutions = Screen.resolutions;
        ResolutionDropdown.ClearOptions();

        List<string> options = new();
        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }


        // update UI with actual values

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();

        VolumeSlider.value = GetVolume();

        FullscreenToggle.isOn = Screen.fullScreen;

        GraphicsDropdown.value = QualitySettings.GetQualityLevel();
        GraphicsDropdown.RefreshShownValue();

    }

    public void SetVolume(float volume)
    {
        MainAudioMixer.SetFloat("MainVolume", volume);
    }

    public float GetVolume()
    {
        float value;
        bool result = MainAudioMixer.GetFloat("MainVolume", out value);
        if (result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}