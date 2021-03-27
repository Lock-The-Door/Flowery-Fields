using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveFileUI : MonoBehaviour
{
    public SaveDisplayer SaveDisplayer;

    public GameMetadata GameMetadata;

    public TMP_InputField SaveNameText;
    public TextMeshProUGUI DayAndWeatherText;
    public TextMeshProUGUI LastVisitText;

    public GameObject ExtraActions;

    // Start is called before the first frame update
    void Start()
    {
        SaveNameText.text = GameMetadata.SaveName;
        DayAndWeatherText.text = $"Day {GameMetadata.Days} - {GameFlow.ToString(GameMetadata.Weather)}";
        LastVisitText.text = "Last Visit: " + GameMetadata.LastVisit.ToString("MM/dd/yyyy - hh:mm tt");
    }

    public void LoadSave()
    {
        if (ExtraActions.activeSelf) // Prevent accidental loading by not accepting clicks while the extra actions menu is open
            return;

        SaveDisplayer.ScreenBlocker.GetComponentInChildren<TextMeshProUGUI>().text = "Day " + GameMetadata.Days;
        SaveDisplayer.ScreenBlocker.SetActive(true);

        GameStatics.Loading = true;
        GameStatics.NewGame = false;
        GameStatics.GameGuid = GameMetadata.GUID;

        SceneManager.LoadScene("Game");
    }

    public void RenameSave()
    {
        SaveNameText.interactable = true;
        SaveNameText.ActivateInputField();
    }
    public void Renamed(string newName)
    {
        // Save the new metadata to the game file
        GameData gameData = new GameData().Load(GameMetadata.GUID).Result;
        gameData.GameMetadata.SaveName = newName;
        GameStatics.NewGame = false;
        gameData.Save();

        SaveDisplayer.DisplaySaves(SaveDisplayer.ReloadSaves().Result);
    }

    public void DeleteSave()
    {
        SaveDisplayer.PopupManager.ShowDecisionWindowPopup("Are you sure?", "Are you sure you want to delete this save? This is an irreversible action!", delegate(bool answer)
        {
            if (!answer)
                return;

            Directory.Delete(Application.persistentDataPath
                + $"/Saves/{GameMetadata.GUID}", true);
            SaveDisplayer.DisplaySaves(SaveDisplayer.ReloadSaves().Result);
        });
    }
}