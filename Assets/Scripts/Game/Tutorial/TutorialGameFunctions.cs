using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TutorialGameFunctions : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public TutorialPlayer Player;
    public PopupManager PopupManager;
    public TutorialFlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;
    private void Start()
    {
        // Hook events
        Application.wantsToQuit += QuitConfirmation;

        // Set manager refs for static class
        TutorialShopItemUpgrades.Player = Player;
        TutorialShopItemUpgrades.FlowerBedManager = FlowerBedManager;
        TutorialShopItemUpgrades.CenterFarm = CenterFarm;

        // Load & Apply Settings
        // Audio
        float MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20 - 10);
    }

    private bool QuitConfirmation()
    {
        // Ask quit confirmation
        PopupManager.ShowDecisionWindowPopup("Take a break?", "Are you sure you want to take a break?", answer => 
        {
            if (answer)
            {
                Application.Quit();
            }
        }, false);

        return false; // Cancel quit to wait for user decision
    }

    public WindowPopup WindowPopup;

    public void ExitGame()
    {
        Application.wantsToQuit -= QuitConfirmation; // Remove listener on game exit

        // Return to menu on exit
        SceneManager.LoadScene("Main Menu");
    }
}