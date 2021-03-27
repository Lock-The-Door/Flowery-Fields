using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    private GameObject SettingsPanel;

    public AudioMixer AudioMixer;
    public Slider MasterVolumeSlider;
    public Slider MusicVolumeSlider;
    public Slider SoundEffectVolumeSlider;

    private void Start()
    {
        // Setup refs
        SettingsPanel = transform.GetChild(0).gameObject;

        // Load Settings

        // Audio
        MasterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        MusicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.35f);
        SoundEffectVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) // Show/Hide Settings menu
            SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }

    // SOUND SETTINGS
    private void VolumeChanged(string volumeName, float newVolume)
    {
        AudioMixer.SetFloat(volumeName, Mathf.Log10(newVolume) * 20);
        PlayerPrefs.SetFloat(volumeName, newVolume);
    }
    public void MasterVolumeChanged(float newVolume) => VolumeChanged("MasterVolume", newVolume);
    public void MusicVolumeChanged(float newVolume) => VolumeChanged("MusicVolume", newVolume);
    public void SoundEffectVolumeChanged(float newVolume) => VolumeChanged("SoundEffectVolume", newVolume);
}
