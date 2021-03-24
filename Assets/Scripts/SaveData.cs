using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class GameMetadata
{
    public string GUID = Guid.Empty.ToString();
    public string SaveName = "A Flowery Field";
    public int Days = 0;
    public GameFlow.Weather Weather = GameFlow.Weather.Sunny;
    public DateTime LastVisit = DateTime.MinValue;

    public Task<GameMetadata> Load(string GUID)
    {
        Debug.Log("Reading metadata file...");
        StreamReader metadataReader = File.OpenText(Application.persistentDataPath
                       + $"/Saves/{GUID}/metadata.json");
        string jsonMetadata = metadataReader.ReadToEnd();
        Debug.Log("Read JSON file");
        metadataReader.Close();
        GameMetadata gameMetadata = JsonConvert.DeserializeObject<GameMetadata>(jsonMetadata);
        Debug.Log("JSON Deserialized");
        Debug.Log("Metadata read and loaded as game metadata");

        Debug.Log("Copying game metadata");
        this.GUID = gameMetadata.GUID;
        SaveName = gameMetadata.SaveName;
        Days = gameMetadata.Days;
        Weather = gameMetadata.Weather;
        LastVisit = gameMetadata.LastVisit;
        Debug.Log("Game metadata copied!");

        return Task.FromResult(this);
    }
}

[Serializable]
public class GameData
{
    // Metadata
    public GameMetadata GameMetadata = new GameMetadata();

    // Game Data
    public Player.Gender PlayerGender;
    public int Days;
    public int Money;
    public List<BorrowedMoneyInfo> BorrowedMoney;
    public GameFlow.Weather Weather;
    public List<string> StorylinesSeen;
    public Dictionary<string, int> ShopItemLevels;
    public Dictionary<int, FlowerBed.FlowerBedState> FlowerBedStates;

    public Task<GameData> Load(string GUID)
    {
        try
        {
            Debug.Log("Deserializing data...");
            if (!File.Exists(Application.persistentDataPath
                    + $"/Saves/{GUID}/save.dat"))
            {
                Debug.LogWarning("Save file does not exist or no save file specified");
                throw new Exception("Save file does not exist or no save file specified");
            }
            // Deserialize game file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.OpenRead(Application.persistentDataPath
                       + $"/Saves/{GUID}/save.dat");
            GameData deserializedData = (GameData)bf.Deserialize(file);
            file.Close();
            Debug.Log("Game data deserialized!");

            Debug.Log("Copying object data...");
            GameMetadata = deserializedData.GameMetadata;
            PlayerGender = deserializedData.PlayerGender;
            Days = deserializedData.Days;
            Money = deserializedData.Money;
            BorrowedMoney = deserializedData.BorrowedMoney;
            Weather = deserializedData.Weather;
            StorylinesSeen = deserializedData.StorylinesSeen;
            ShopItemLevels = deserializedData.ShopItemLevels;
            FlowerBedStates = deserializedData.FlowerBedStates;
            Debug.Log("Object data copied!");

            return Task.FromResult(this);
            
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Failed to deserialize save!");

            Debug.Log("Saving loaded data and creating backup copy");
            Save(true, "load_failure_backup").Wait(); // this will create a backup of the old corrupted copy and by saving, we create a fresh save that will likely work

            throw e;
        }
    }

    public Task Save(bool makeBackupCopy = false, string backupName = "backup")
    {
        // backup copy
        if (makeBackupCopy && File.Exists(Application.persistentDataPath
             + $"/Saves/{GameMetadata.GUID}/save.dat"))
        {
            Debug.Log($"Creating backup: {backupName}");

            File.Copy(Application.persistentDataPath
             + $"/Saves/{GameMetadata.GUID}/save.dat",
             Application.persistentDataPath
             + $"/Saves/{GameMetadata.GUID}/backup-{backupName}.dat.backup", true);

            Debug.Log("Saved backup copy of save file");
        }

        if (GameStatics.NewGame)
        {
            Debug.Log("Assigning guid");
            GameMetadata.GUID = Guid.NewGuid().ToString(); // Give new saves guids
        }

        Debug.Log("Directories created");
        Directory.CreateDirectory(Application.persistentDataPath + $"/Saves/{GameMetadata.GUID}"); // Create folder if not already made

        // Create json metadata
        Debug.Log("Creating metadata...");
        GameMetadata.Days = Days;
        GameMetadata.Weather = Weather;
        GameMetadata.LastVisit = DateTime.Now;
        StreamWriter metadataFile = File.CreateText(Application.persistentDataPath
             + $"/Saves/{GameMetadata.GUID}/metadata.json");
        metadataFile.Write(JsonConvert.SerializeObject(GameMetadata));
        metadataFile.Close();
        Debug.Log("Metadata file created!");

        // Create save file data
        Debug.Log("Serializing game data...");
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
             + $"/Saves/{GameMetadata.GUID}/save.dat");
        binaryFormatter.Serialize(file, this);
        file.Close();
        Debug.Log("Game data serialized & saved!");

        return Task.CompletedTask;
    }

    public static async Task Convert(string pathOrGUID, int saveFileVersion)
    {
        try
        {
            switch (saveFileVersion)
            {
                case 0:
                    // Save files were stored in root save folder and there was no json file and backup files were not a thing
                    // Goal: move the save file into a new folder and rename it to "save.dat" game saves also didn't have names so give them a default name
                    string GUID = Path.GetFileNameWithoutExtension(pathOrGUID);
                    Directory.CreateDirectory(Application.persistentDataPath + "/Saves/" + GUID);
                    File.Copy(pathOrGUID, Application.persistentDataPath + $"/Saves/{GUID}/save.dat"); // Copy the file with new file name
                    File.Delete(pathOrGUID); // Remove the old file
                    // Game data has moved metadata to separate class, simply resave the file with new metadata class to create metadata file
                    GameData oldGameData = new GameData();
                    await oldGameData.Load(GUID);
                    GameStatics.NewGame = false; // Don't assign new GUID
                    oldGameData.GameMetadata = new GameMetadata();
                    oldGameData.GameMetadata.GUID = GUID;
                    oldGameData.GameMetadata.SaveName = "A Flowery Field"; // Assign a save file name
                    await oldGameData.Save();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error trying while converting old save file {pathOrGUID} from version {saveFileVersion}: " + e);
        }
    }
    public static async Task ConvertDirectory()
    {
        // VERSION 0
        string[] gameDataPaths = Directory.GetFiles(Application.persistentDataPath + "/Saves", "*dat");
        foreach (string path in gameDataPaths)
        {
            Debug.Log(path);
            await Convert(path, 0);
        }
    }
}