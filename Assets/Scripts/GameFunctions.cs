using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GameStatics
{
    public static string GameGuid;
    public static bool NewGame = true;
}

public class GameFunctions : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public GameFlow GameFlow;
    public Player Player;
    public StorylineManager StorylineManager;
    public Shop Shop;
    public BorrowMoney BorrowMoney;
    public FlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;
    public NextDayScreen NextDayScreen;
    private void Start()
    {
        // Hook events
        Application.wantsToQuit += () =>
        {
            SaveGame().Wait();
            return true;
        };

        // Set manager refs for static class
        ShopItemUpgrades.Player = Player;
        ShopItemUpgrades.FlowerBedManager = FlowerBedManager;
        ShopItemUpgrades.CenterFarm = CenterFarm;

        // Load & Apply Settings
        // Audio
        float MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20 - 10);


        // Load game data
        if (!GameStatics.NewGame)
            LoadGame();
    }

    public Camera Camera;
    public WindowPopup WindowPopup;
    void LoadGame()
    {
        try
        {
            if (!File.Exists(Application.persistentDataPath
                    + $"/Saves/{GameStatics.GameGuid}.dat"))
            {
                Debug.LogWarning("Save file does not exist or no save file specified");
                throw new Exception("Save file does not exist or no save file specified");
            }
            // Deserialize game file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file =
                       File.Open(Application.persistentDataPath
                       + $"/Saves/{GameStatics.GameGuid}.dat", FileMode.Open);
            GameData gameData = (GameData)bf.Deserialize(file);
            file.Close();

            // Load game data
            Player.PlayerGender = gameData.PlayerGender;
            GameFlow.Days = gameData.Days;
            Player.Money = gameData.Money;
            BorrowMoney.DailyPayments = gameData.BorrowedMoney;
            GameFlow.SetWeather(gameData.Weather);
            StorylineManager.StorylinesSeen = gameData.StorylinesSeen;
            Shop.ShopItems.ForEach(shopItem => { for (int i = 0; i < gameData.ShopItemLevels[shopItem.Name]; i++) shopItem.Upgrade(); });
            FlowerBedManager.transform.GetComponentsInChildren<FlowerBed>().ToList().ForEach(flowerbed => flowerbed.UpdateFlowerbedState(gameData.FlowerBedStates[flowerbed.id]));

            Debug.Log("Game data loaded!");

            StartCoroutine(NextDayScreen.ShowScreen(gameData.Days));
        }
        catch (Exception e)
        {
            Debug.LogError(e);

            Debug.LogError("Failed to load game save");

            Canvas OverlayCanvas = new GameObject().AddComponent<Canvas>();
            OverlayCanvas.gameObject.AddComponent<GraphicRaycaster>();
            OverlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            WindowPopup windowPopup = Instantiate(WindowPopup, OverlayCanvas.transform);
            windowPopup.TitleText.text = "Looks like we ran into an error!";
            windowPopup.DetailsText.text = e.Message + "\nLook in the log files for more information.";
            windowPopup.callbackAction = () =>
            {
                SaveGame(true).Wait();

                SceneManager.LoadScene("Main Menu");
            };
        }
    }

    public bool SaveInEditor = false;
    public Task SaveGame(bool makeBackupCopy = false)
    {
        // don't save in editor
        if (!SaveInEditor)
            return Task.CompletedTask;

        // backup copy
        if (makeBackupCopy && File.Exists(Application.persistentDataPath
             + $"/Saves/{GameStatics.GameGuid}.dat"))
        {
            File.Copy(Application.persistentDataPath
             + $"/Saves/{GameStatics.GameGuid}.dat",
             Application.persistentDataPath
             + $"/Saves/{GameStatics.GameGuid}.dat.backup", true);
            Debug.Log("Saved backup copy of save file");
        }

        if (GameStatics.GameGuid == null)
            GameStatics.GameGuid = Guid.NewGuid().ToString(); // Create new game guid if not already made

        Directory.CreateDirectory(Application.persistentDataPath + "/Saves"); // Create folder if not already made

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath
             + $"/Saves/{GameStatics.GameGuid}.dat");

        // Create save file data
        GameData gameData = new GameData();

        gameData.GUID = GameStatics.GameGuid;

        gameData.PlayerGender = Player.PlayerGender;
        gameData.Days = GameFlow.Days;
        gameData.Money = Player.Money;
        gameData.BorrowedMoney = BorrowMoney.DailyPayments;
        gameData.Weather = GameFlow.weather;
        gameData.StorylinesSeen = StorylineManager.StorylinesSeen;
        gameData.ShopItemLevels = Shop.ShopItems.Select(shopItem => new { name = shopItem.Name, level = shopItem.Level }).ToDictionary(x => x.name, x => x.level);
        gameData.FlowerBedStates = FlowerBedManager.transform.GetComponentsInChildren<FlowerBed>().Select(flowerbed => new { id = flowerbed.id, state = flowerbed.state }).ToDictionary(x => x.id, x => x.state);

        binaryFormatter.Serialize(file, gameData);
        file.Close();
        Debug.Log("Game data saved!");

        return Task.CompletedTask;
    }

    public void ExitGame()
    {
        SaveGame().Wait();

        // Return to menu on exit
        SceneManager.LoadScene("Main Menu");
    }
}

[Serializable]
class GameData
{
    // Metadata
    public string SaveFileName = null;
    public string GUID;

    // Game Data
    public Player.Gender PlayerGender;
    public int Days;
    public int Money;
    public List<BorrowedMoneyInfo> BorrowedMoney;
    public GameFlow.Weather Weather;
    public List<string> StorylinesSeen;
    public Dictionary<string, int> ShopItemLevels;
    public Dictionary<int, FlowerBed.FlowerBedState> FlowerBedStates;
}