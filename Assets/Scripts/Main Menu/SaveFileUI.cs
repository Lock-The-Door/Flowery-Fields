using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath
                       + $"/Saves/{GameMetadata.GUID}.dat", FileMode.Open);
        GameData gameData = (GameData)bf.Deserialize(file);
        file.Close();

        gameData.SaveFileName = newName;
        file = File.Create(Application.persistentDataPath
                       + $"/Saves/{GameMetadata.GUID}.dat");
        bf.Serialize(file, gameData);
        file.Close();

        SaveDisplayer.DisplaySaves(SaveDisplayer.ReloadSaves());
    }

    public void DeleteSave()
    {
        SaveDisplayer.PopupManager.ShowDecisionWindowPopup("Are you sure?", "Are you sure you want to delete this save? This is an irreversible action!", delegate(bool answer)
        {
            if (!answer)
                return;

            File.Delete(Application.persistentDataPath
                + $"/Saves/{GameMetadata.GUID}.dat");
            SaveDisplayer.DisplaySaves(SaveDisplayer.ReloadSaves());
        });
    }
}

public class GameMetadata
{
    public string SaveName;
    public int Days;
    public GameFlow.Weather Weather;
    public DateTime LastVisit;
    public string GUID;

    public GameMetadata(string _name, int _days, GameFlow.Weather _weather, DateTime _lastVisit, string _GUID)
    {
        SaveName = _name;
        Days = _days;
        Weather = _weather;
        LastVisit = _lastVisit;
        GUID = _GUID;
    }
}