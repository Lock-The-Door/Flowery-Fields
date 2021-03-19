using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private GameObject SettingsPanel;

    public AudioMixer AudioMixer;
    public Slider MasterVolumeSlider;

    private void Start()
    {
        // Setup refs
        SettingsPanel = transform.GetChild(0).gameObject;

        // Load Settings

        // Audio
        MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) // Show/Hide Settings menu
            SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }

    public void VolumeChanged(float newVolume)
    {
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(newVolume) * 20 - 10);
        PlayerPrefs.SetFloat("MasterVolume", newVolume);
    }
}
