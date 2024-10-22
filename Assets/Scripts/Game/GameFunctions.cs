using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GameStatics
{
    public static bool SaveInEditor = false;

    public static string GameGuid;
    public static bool NewGame = true;
    public static GameData LoadedGame = new GameData();

    public static bool Loading = false;
}

public class GameFunctions : MonoBehaviour
{
    public GameFlow GameFlow;
    public Player Player;
    public StorylineManager StorylineManager;
    public PopupManager PopupManager;
    public Shop Shop;
    public BorrowMoney BorrowMoney;
    public FlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;
    public NextDayScreen NextDayScreen;
    private void Start()
    {
        // Hook events
        Application.wantsToQuit += QuitConfirmation;

        // Set manager refs for static class
        ShopItemUpgrades.Player = Player;
        ShopItemUpgrades.FlowerBedManager = FlowerBedManager;
        ShopItemUpgrades.CenterFarm = CenterFarm;

        // Load game data
        if (!GameStatics.NewGame)
            LoadGame().Wait();

        GameStatics.Loading = false;
    }

    private bool QuitConfirmation()
    {
        // Ask quit confirmation
        PopupManager.ShowDecisionWindowPopup("Take a break?", "Are you sure you want to take a break?", answer => 
        {
            if (answer)
            {
                PopupManager.ShowWindowPopup("Have a nice break!", "Your flower farm will be kept safe and things will stay right where you left them.");
                SaveGame().Wait();
                Application.wantsToQuit -= QuitConfirmation; // Unbind this function to prevent it from triggering on exit
                Application.Quit();
            }
        }, false);

        return false; // Cancel quit to wait for user decision
    }

    public WindowPopup WindowPopup;

    async Task LoadGame()
    {
        Debug.Log("Loading game data...");

        try
        {
            await GameStatics.LoadedGame.Load(GameStatics.GameGuid);

            Debug.Log("Loading game data to variables");
            Player.PlayerGender = GameStatics.LoadedGame.PlayerGender;
            GameFlow.Days = GameStatics.LoadedGame.Days;
            GameFlow.InDebt = GameStatics.LoadedGame.InDebt;
            Player.Money = GameStatics.LoadedGame.Money;
            BorrowMoney.DailyPayments = GameStatics.LoadedGame.BorrowedMoney;
            BorrowMoney.maxMoneyAvalibleToBorrow = GameStatics.LoadedGame.BorrowLimit;
            StorylineManager.StorylinesSeen = GameStatics.LoadedGame.StorylinesSeen;
            StorylineManager.Storylines.FindAll(storyline => (storyline.DayOfTrigger > 0 && storyline.DayOfTrigger <= GameFlow.Days) || StorylineManager.StorylinesSeen.Contains(storyline.Name)).ForEach(storyline => storyline.RunStoryline(PopupManager)); // Redo all storyline actions
            GameFlow.SetWeather(GameStatics.LoadedGame.Weather);
            Shop.ShopItems.ForEach(shopItem => { for (int i = 0; i < GameStatics.LoadedGame.ShopItemLevels[shopItem.Name]; i++) shopItem.Upgrade(); });
            FlowerBedManager.transform.GetComponentsInChildren<FlowerBed>().ToList().ForEach(flowerbed => flowerbed.UpdateFlowerbedState(GameStatics.LoadedGame.FlowerBedStates[flowerbed.id]));
            Debug.Log("Variables set!");

            Debug.Log("Game data loaded!");

            StartCoroutine(NextDayScreen.ShowScreen(GameStatics.LoadedGame.Days));
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load game save");
            Debug.LogError(e);

            Canvas OverlayCanvas = new GameObject().AddComponent<Canvas>();
            OverlayCanvas.gameObject.AddComponent<GraphicRaycaster>();
            OverlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            WindowPopup windowPopup = Instantiate(WindowPopup, OverlayCanvas.transform);
            windowPopup.queuedPopup = false;
            windowPopup.TitleText.text = "Looks like we ran into an error!";
            windowPopup.DetailsText.text = e.Message + "\nLook in the log files for more information and maybe make a Github issue.";
            windowPopup.callbackAction = () => SceneManager.LoadScene("Main Menu");
            windowPopup.transform.GetChild(0).GetComponent<RectTransform>().offsetMin = new Vector2(100, 50);
            windowPopup.transform.GetChild(0).GetComponent<RectTransform>().offsetMax = new Vector2(-100, -50);
            windowPopup.gameObject.SetActive(true);
        }
    }

    public async Task SaveGame(bool makeBackupCopy = false, string backupName = "backup")
    {
        Debug.Log("Saving game...");

        // don't save in editor
        if (!GameStatics.SaveInEditor && Application.isEditor)
            return;

        // Write values to game data object
        Debug.Log("Copying game variables to game data class...");
        GameStatics.LoadedGame.PlayerGender = Player.PlayerGender;
        GameStatics.LoadedGame.Days = GameFlow.Days;
        GameStatics.LoadedGame.InDebt = GameFlow.InDebt;
        GameStatics.LoadedGame.Money = Player.Money;
        GameStatics.LoadedGame.BorrowedMoney = BorrowMoney.DailyPayments;
        GameStatics.LoadedGame.BorrowLimit = BorrowMoney.maxMoneyAvalibleToBorrow;
        GameStatics.LoadedGame.Weather = GameFlow.weather;
        GameStatics.LoadedGame.StorylinesSeen = StorylineManager.StorylinesSeen;
        GameStatics.LoadedGame.ShopItemLevels = Shop.ShopItems.Select(shopItem => new { name = shopItem.Name, level = shopItem.Level }).ToDictionary(x => x.name, x => x.level);
        GameStatics.LoadedGame.FlowerBedStates = FlowerBedManager.transform.GetComponentsInChildren<FlowerBed>().Select(flowerbed => new { flowerbed.id, flowerbed.state }).ToDictionary(x => x.id, x => x.state);
        Debug.Log("Game variables copied!");

        await GameStatics.LoadedGame.Save(makeBackupCopy, backupName);

        GameStatics.NewGame = false;
        GameStatics.GameGuid = GameStatics.LoadedGame.GameMetadata.GUID;
        Debug.Log("Game data saved!");
    }

    public void ExitGame()
    {
        var gameAwaiter = SaveGame();

        Application.wantsToQuit -= QuitConfirmation; // Remove listener on game exit

        gameAwaiter.Wait(); // remove listener while saving game

        // Return to menu on exit
        SceneManager.LoadScene("Main Menu");
    }
}