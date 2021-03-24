using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public SaveDisplayer SaveDisplayer;

    List<GameMetadata> SaveFiles = new List<GameMetadata>();

    void Start()
    {
        GameStatics.SaveInEditor = true; // I'm likely going to need to save the game if I'm coming from the main menu

        SaveFiles = SaveDisplayer.ReloadSaves().Result;
    }

    public void PlayGame()
    {
        if (SaveFiles.Count == 0)
        {
            NewGame();
            return;
        }

        // Do select a save menu
        SaveDisplayer.gameObject.SetActive(true); 
        SaveDisplayer.DisplaySaves(SaveFiles);
    }

    public void NewGame()
    {
        GameStatics.NewGame = true;
        GameStatics.GameGuid = null;

        SceneManager.LoadScene("Game");
    }
}