using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public PopupManager PopupManager;
    public LoadTutorialScene LoadTutorialScene;

    public SaveDisplayer SaveDisplayer;
    public GameObject ScreenBlocker;

    List<GameMetadata> SaveFiles = new List<GameMetadata>();

    void Start()
    {
        GameStatics.SaveInEditor = true; // I'm likely going to need to save the game if I'm coming from the main menu
    }

    public void PlayGame()
    {
        SaveFiles = SaveDisplayer.ReloadSaves().Result;

        if (SaveFiles.Count == 0) // No saves found
        {
            if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
            {
                PopupManager.ShowDecisionWindowPopup("You haven't finished the tutorial!", "Do you want to do the tutorial first?", willDoTutorial =>
                {
                    if (willDoTutorial)
                    {
                        LoadTutorialScene.PlayTutorial();
                        return;
                    }

                    // If not doing tutorial, just play the game
                    PlayerPrefs.SetInt("TutorialComplete", 1);
                    PlayGame();
                });

                return;
            }

            NewGame();
            return;
        }

        // Returning user, still ask if the user wants to do the tutorial
        if (PlayerPrefs.GetInt("TutorialComplete", 0) == 0)
        {
            PopupManager.ShowDecisionWindowPopup("Welcome back!", "It looks like you've played before, but we don't know if you've tried out the tutorial yet. Want to do it now?", doTutorial =>
            {
                // Say the tutorial is completed regardless of option
                PlayerPrefs.SetInt("TutorialComplete", 1);

                if (doTutorial)
                {
                    LoadTutorialScene.PlayTutorial();
                    return;
                }

                // If not doing tutorial, just show save select
                PlayGame();
            });
            return;
        }

        // Do select a save menu
        SaveDisplayer.gameObject.SetActive(true);
        SaveDisplayer.DisplaySaves(SaveFiles);
    }

    public void NewGame()
    {
        // Show screen blocker
        ScreenBlocker.SetActive(true);

        GameStatics.Loading = true;
        GameStatics.NewGame = true;
        GameStatics.GameGuid = null;

        SceneManager.LoadScene("Game");
    }
}