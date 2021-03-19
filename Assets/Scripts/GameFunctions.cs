using UnityEngine;
using UnityEngine.Audio;

public class GameFunctions : MonoBehaviour
{
    public AudioMixer AudioMixer;
    private void Start()
    {
        // Load & Apply Settings

        // Audio
        float MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20 - 10);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
