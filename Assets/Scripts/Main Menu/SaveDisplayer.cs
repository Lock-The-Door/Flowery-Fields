using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDisplayer : MonoBehaviour
{
    public PopupManager PopupManager;
    public GameObject ScreenBlocker;

    public RectTransform SavesScrollView;
    public SaveFileUI SaveFile;
    readonly float savesUiGap = 170;

    public async Task<List<GameMetadata>> ReloadSaves()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            return new List<GameMetadata>();

        await GameData.ConvertDirectory(); // Convert save files

        List<GameMetadata> _saveFiles = new List<GameMetadata>();

        var directoriesToSearch = Directory.EnumerateDirectories(Application.persistentDataPath + "/Saves");
        foreach (var directory in directoriesToSearch)
        {
            string GUID = new DirectoryInfo(directory).Name;
            _saveFiles.Add(await new GameMetadata().Load(GUID));
        }
        
        _saveFiles.Sort((metadata1, metadata2) => metadata2.LastVisit.CompareTo(metadata1.LastVisit));

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
