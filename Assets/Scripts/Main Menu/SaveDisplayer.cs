using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveDisplayer : MonoBehaviour
{
    public PopupManager PopupManager;

    public RectTransform SavesScrollView;
    public SaveFileUI SaveFile;

    float savesUiGap = 170;

    public List<GameMetadata> ReloadSaves()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            return new List<GameMetadata>();

        List<GameMetadata> _saveFiles = new List<GameMetadata>();

        string[] gameDataPaths = Directory.GetFiles(Application.persistentDataPath + "/Saves", "*dat");
        foreach (string path in gameDataPaths)
        {
            try
            {
                Debug.Log(path);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                GameData gameData = (GameData)bf.Deserialize(file);

                _saveFiles.Add(new GameMetadata(gameData.SaveFileName, gameData.Days, gameData.Weather, File.GetLastWriteTime(path), gameData.GUID));
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error trying while loading save file information: " + e);
            }
        }
        _saveFiles.Sort((metadata1, metadata2) => metadata1.LastVisit.CompareTo(metadata2.LastVisit));

        return _saveFiles;
    }

    public void DisplaySaves(List<GameMetadata> saves)
    {
        SavesScrollView.GetComponentsInChildren<SaveFileUI>().ToList().ForEach(saveFileUi => Destroy(saveFileUi.gameObject)); // Remove all children

        SavesScrollView.sizeDelta = new Vector2(SavesScrollView.sizeDelta.x, savesUiGap * saves.Count); // Set size based on # of saves

        float saveUiY = -20;
        foreach (GameMetadata metadata in saves)
        {
            SaveFileUI saveFileUI = Instantiate(SaveFile, SavesScrollView);
            saveFileUI.SaveDisplayer = this;

            saveFileUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, saveUiY);
            saveFileUI.GameMetadata = metadata;

            saveUiY -= savesUiGap;
        }
    }
}
