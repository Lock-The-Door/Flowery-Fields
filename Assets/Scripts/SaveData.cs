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
    public int SaveVersion = 2;
    public string SaveName = "A Flowery Field";
    public int Days = 1;
    public GameFlow.Weather Weather = GameFlow.Weather.Sunny;
    public DateTime LastVisit = DateTime.MinValue;

    public GameMetadata() { }
    [JsonConstructor]
    public GameMetadata(string guid, int saveVersion, string saveName, int days, GameFlow.Weather weather, DateTime lastVisit)
    {
        GUID = guid;
        SaveVersion = saveVersion != default ? saveVersion : 1;
        SaveName = saveName;
        Days = days;
        Weather = weather;
        LastVisit = lastVisit;
    }

    public Task<GameMetadata> Load(string GUID)
    {
        Debug.Log("Reading metadata file... " + GUID);
        StreamReader metadataReader = File.OpenText(Application.persistentDataPath
                       + $"/Saves/{GUID}/metadata.json");
        string jsonMetadata = metadataReader.ReadToEnd();
        Debug.Log("Read JSON file");
        metadataReader.Close();
        Debug.Log(jsonMetadata);
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
    public Player.Gender PlayerGender = Player.Gender.boy;
    public int Days = 1;
    public bool InDebt = false;
    public int Money = 100;
    public List<BorrowedMoneyInfo> BorrowedMoney = new List<BorrowedMoneyInfo>();
    public int BorrowLimit = 1000;
    public GameFlow.Weather Weather = GameFlow.Weather.Sunny;
    public List<string> StorylinesSeen = new List<string>();
    public Dictionary<string, int> ShopItemLevels;
    public Dictionary<int, FlowerBed.FlowerBedState> FlowerBedStates;

    public async Task<GameData> Load(string GUID, bool isConversionLoad = false)
    {
        try
        {
            Debug.Log("Deserializing data... " + GUID);
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

            Debug.Log("Checking for game save format updates...");
            if (!isConversionLoad && deserializedData.GameMetadata.SaveVersion != new GameMetadata().SaveVersion)
            {
                await Convert(GUID, deserializedData.GameMetadata.SaveVersion);
                Debug.Log("Old format detected! Converting...");
            }
            else
                Debug.Log("No conversions needed");

            Debug.Log("Copying object data...");
            GameMetadata = deserializedData.GameMetadata;
            PlayerGender = deserializedData.PlayerGender;
            Days = deserializedData.Days;
            InDebt = deserializedData.InDebt;
            Money = deserializedData.Money;
            BorrowedMoney = deserializedData.BorrowedMoney;
            BorrowLimit = deserializedData.BorrowLimit;
            Weather = deserializedData.Weather;
            StorylinesSeen = deserializedData.StorylinesSeen;
            ShopItemLevels = deserializedData.ShopItemLevels;
            FlowerBedStates = deserializedData.FlowerBedStates;
            Debug.Log("Object data copied!");

            return this;
            
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError("Failed to deserialize save!");

            Debug.Log("Saving loaded data and creating backup copy");
            Save(true, "load_failure_backup").Wait(); // this will create a backup of the old corrupted copy and by saving, we create a fresh save that will likely work

            return new GameData()
            {
                GameMetadata = new GameMetadata()
                {
                    GUID = Guid.NewGuid().ToString(),
                    SaveName = "Corrupted Or Missing Save"
                }
            };
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
        Debug.Log($"Converting game save {pathOrGUID} from version {saveFileVersion} to latest version of {new GameMetadata().SaveVersion}");

        try
        {
            // Load game save
            GameData oldGameData = new GameData();
            GameStatics.NewGame = false; // Don't assign new GUID
            if (Guid.TryParse(pathOrGUID, out _))
            {
                await oldGameData.Load(pathOrGUID, true);
            }

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
                    await oldGameData.Load(GUID, true);

                    oldGameData.GameMetadata = new GameMetadata
                    {
                        GUID = GUID,
                        SaveName = "A Flowery Field", // Assign a save file name
                        SaveVersion = 1 // Assign a version number
                    };
                    await oldGameData.Save();

                    pathOrGUID = GUID; // Set the outer variable for recursive conversion
                    break;
                case 1:
                    // Somehow I deleted the game version and need to re assign it
                    // Need to also assign inDebt variable to fix a bug
                    oldGameData.GameMetadata = new GameMetadata
                    {
                        GUID = pathOrGUID,
                        SaveVersion = 2 // Assign a version number
                    };
                    oldGameData.InDebt = oldGameData.Money < 0; // Assign new variable, inDebt

                    await oldGameData.Save();
                    break;
            }

            GameStatics.NewGame = true; // Reset game statics

            // See if further conversion is needed
            if (oldGameData.GameMetadata.SaveVersion != new GameMetadata().SaveVersion)
                await Convert(pathOrGUID, oldGameData.GameMetadata.SaveVersion);

            Debug.Log("Finished converting to version " + oldGameData.GameMetadata.SaveVersion);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error trying while converting old save file {pathOrGUID} from version {saveFileVersion}: " + e);
        }
    }
    public static async Task ConvertDirectory()
    {
        Debug.Log("Finding saves to be converted in the saves directory");

        // VERSION 0
        string[] gameDataPaths = Directory.GetFiles(Application.persistentDataPath + "/Saves", "*dat");
        foreach (string path in gameDataPaths)
        {
            Debug.Log("Converting " + path);
            await Convert(path, 0);
        }

        // VERSION 1
        string[] directoryPaths = Directory.GetDirectories(Application.persistentDataPath + "/Saves");
        foreach (string path in directoryPaths)
        {
            Debug.Log("Testing " + path);

            StreamReader streamReader = File.OpenText(path + "/metadata.json");
            GameMetadata metadata = JsonConvert.DeserializeObject<GameMetadata>(streamReader.ReadToEnd());
            streamReader.Close();

            if (metadata.SaveVersion == 1)
                await Convert(metadata.GUID, 1);
        }

        // VERSION 2+ WILL BE CONVERTED ON LOAD
    }
}