using System;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour
{
    public AudioMixer AudioMixer;

    // Start is called before the first frame update
    void Start()
    {
        // Reset game statics
        GameStatics.NewGame = true;
        GameStatics.LoadedGame = new GameData();
        GameStatics.GameGuid = Guid.Empty.ToString();

        // Load & Apply Settings
        // Audio
        float MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
        float MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.35f);
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20);
        float SoundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
        AudioMixer.SetFloat("SoundEffectVolume", Mathf.Log10(SoundEffectVolume) * 20);
    }
}
