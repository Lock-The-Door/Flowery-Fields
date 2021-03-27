using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGameFunctions : MonoBehaviour
{
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