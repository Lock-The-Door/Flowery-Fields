using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    List<GameMetadata> SaveFiles = new List<GameMetadata>();

    void Start()
    {
        if (Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            string[] gameDataPaths = Directory.GetFiles(Application.persistentDataPath + "/Saves");
            foreach (string path in gameDataPaths)
            {
                Debug.Log(path);
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                GameData gameData = (GameData)bf.Deserialize(file);

                SaveFiles.Add(new GameMetadata(gameData.SaveFileName, File.GetLastWriteTime(path), gameData.GUID));
            }
        }
    }

    public void PlayGame()
    {
        if (SaveFiles.Count == 0)
        {
            SceneManager.LoadScene("Game");
            return;
        }

        GameStatics.GameGuid = SaveFiles[0].GUID;
        GameStatics.NewGame = false;

        SceneManager.LoadScene("Game");


        // Do select a save menu (not implemented)
    }
}

class GameMetadata
{
    public string SaveName;
    public System.DateTime LastVisit;
    public string GUID;

    public GameMetadata(string _name, System.DateTime _lastVisit, string _GUID)
    {
        SaveName = _name;
        LastVisit = _lastVisit;
        GUID = _GUID;
    }
}