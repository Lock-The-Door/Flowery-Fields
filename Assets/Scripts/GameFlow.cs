using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public enum Weather
    {
        Sunny = 55,
        Rainy = 30,
        SuperStorm = 10,
        NaturalDisaster = 5
    }
    public Weather weather = Weather.Sunny;

    public int familyPayment = 20;

    public GameObject FlowerBeds;

    public StorylineManager StorylineManager;

    void Start()
    {
        GenerateWeather(); // Generate weather for first day

        StorylineManager.ShowStoryline("The Adventure Begins"); // Show starter story

        // Set variables in randomiser
        RandomEvents.Player = Player;
        RandomEvents.PopupManager = PopupManager;
        RandomEvents.FlowerBedManager = FlowerBedManager;
    }

    void GenerateWeather()
    {
        int randomWeatherInt = Random.Range(0,101);

        var weatherTypes = (Weather[]) System.Enum.GetValues(typeof(Weather));


        foreach (Weather randomWeather in weatherTypes)
        {
            if (randomWeatherInt > (int)randomWeather)
                continue;

            // Matching weather
            weather = randomWeather;
            Debug.Log("Weather is: " + weather);
            WeatherGui.GetComponentInChildren<TextMeshProUGUI>().text = weather.ToString(); // Update text
            WeatherGui.GetComponent<Image>().sprite = WeatherImages[System.Array.IndexOf(weatherTypes.Reverse().ToArray(), weather)]; // Update Image
            return;
        }
    }

    public Player Player;
    public PopupManager PopupManager;
    public FlowerBedManager FlowerBedManager;
    public GameObject WeatherGui;
    public List<Sprite> WeatherImages;
    public Shop Shop;
    public BorrowMoney BorrowMoney;
    public bool finishedGame = false;
    public bool inDebt = false;

    

    void FinishDay()
    {
        Debug.Log("Finishing day!");

        // Manage Debt
        if (Player.money < 0)
        {
            if (inDebt)
            {
                Debug.Log("You are in debt");
                // Try to sell flowerbeds first to cover debt
                ShopItem shopItem = Shop.ShopItems.Find(shopItem => shopItem.Name == "Flower Bed");
                if (shopItem.Level > 0)
                {
                    // Display message
                    PopupManager.ShowWindowPopup("You sold a flowerbed", "Since you were still in debt, you were forced to sell a flowerbed to help you get back on your feet.");

                    shopItem.Price -= 25 * shopItem.Level;// Lower flowerbed price
                    FlowerBedManager.SendMessage("RemoveFlowerBed", --shopItem.Level); // Sell flowerbed
                    Player.money += 500; // Return $500
                    Shop.UpdateBuyButtonVisual(shopItem); // Update visuals
                }
                else
                {
                    // Display message
                    PopupManager.ShowWindowPopup("You've lost everything...", "Sadly, you've ended up with less money than you've started with. Luckily for you, your parents were nice enough to pay for your debts and give you a fresh start.");

                    // Reset values
                    Player.money = 100;
                    familyPayment = 20;
                }
            }
            else
            {
                inDebt = true;
                PopupManager.ShowWindowPopup("You're in debt!", "You are in debt! Get out of debt or you'll soon need to start selling your flowerbeds!");
            }
        }
        else
        {
            inDebt = false;

            Player.money -= familyPayment; // Pay family
            Player.money -= BorrowMoney.totalDailyPayment; // Pay loans
            BorrowMoney.UpdateDailyPayments();

            RandomEvents.Run(); // Random events
        }

        // Game won?
        if (!finishedGame && Player.money > 5000 && Shop.isMaxedOut)
        {
            Debug.Log("You've made a lot of money, your family is proud of you. The end! :)");
            StorylineManager.ShowStoryline("The End");
            finishedGame = true;
        }


        // PLAYER
        Player.InHand = Player.Items.Nothing; // Empty hands


        // FLOWER BEDS
        // Get Flower Beds States
        var FlowerBedScripts = FlowerBeds.GetComponentsInChildren<FlowerBed>();


        // WEATHER
        // Apply logic to flower beds
        Debug.Log(weather);
        switch (weather)
        {
            case Weather.Sunny:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    FlowerBed.FlowerBedState state = FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.DeadFlowers;
                            else
                                state = FlowerBed.FlowerBedState.WeakFlowers; // rare chance of surviving flowers
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            if (randomFlowerChance > 0.1)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 90% normal
                            else if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 5% chance of better flowers
                            else if (randomFlowerChance > 0.01)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 4% chance of weak flowers
                            else
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 1% chance of super flowers
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            state = FlowerBed.FlowerBedState.BeautifulFlowers;
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.55)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 45% chance of downgrade
                            else if (randomFlowerChance > 0.45)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 10% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.85)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 15% chance of downgrade
                            else if (randomFlowerChance > 0.6)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 25% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 50% chance of flower death
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 25% chance of upgrade
                            break;
                    }

                    FlowerBedScript.UpdateFlowerbedState(state);
                }

                break;

            case Weather.Rainy:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    FlowerBed.FlowerBedState state = FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.2)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 80% chance of normal flowers
                            else if (randomFlowerChance > 0.02)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 18% chance of better flowers
                            else
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 2% chance of super flowers
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.DrownedFlowers;
                            else
                                state = FlowerBed.FlowerBedState.WeakFlowers; // rare chance of surviving flowers
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            state = FlowerBed.FlowerBedState.BeautifulFlowers;
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.7)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 30% chance of downgrade
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.7)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 30% chance of downgrade
                            else if (randomFlowerChance > 0.4)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 30% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.7)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 30% chance of flower death
                            else if (randomFlowerChance > 0.4)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 30% chance of upgrade
                            break;
                    }

                    FlowerBedScript.UpdateFlowerbedState(state);
                }

                break;

            case Weather.SuperStorm:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    FlowerBed.FlowerBedState state = FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 50% death rate
                            else if (randomFlowerChance > 0.20)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 20% flowers are better
                            else if (randomFlowerChance > 0.15)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 5% of flowers are super flowers
                            else
                                state = FlowerBed.FlowerBedState.NormalFlowers; // rest 15% are normal
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            state = FlowerBed.FlowerBedState.DrownedFlowers;
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 75% 1 downgrade
                            else if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 20% double downgrade
                            else
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 5% triple downgrade
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.6)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 40% 1 downgrade
                            else if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 10% double downgrade
                            else if (randomFlowerChance > 0.49)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 1% triple downgrade
                            else if (randomFlowerChance < 0.29)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 20% upgrade chance
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.75)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 25% chance of downgrade
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 50% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 50% chance of flower death
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 25% chance of upgrade
                            break;
                    }

                    FlowerBedScript.UpdateFlowerbedState(state);
                }

                break;

            case Weather.NaturalDisaster:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    ref FlowerBed.FlowerBedState state = ref FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.95)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 5% chance for super flowers
                            else if (randomFlowerChance > 0.9)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 5% chance for beautiful flowers
                            else if (randomFlowerChance > 0.85)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 5% chance for normal flowers
                            else if (randomFlowerChance > 0.8)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 5% chance for weak flowers
                            else
                                state = FlowerBed.FlowerBedState.DeadFlowers; // otherwise, dead
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.DrownedFlowers; // 95% death rate
                            else
                                state = FlowerBed.FlowerBedState.WeakFlowers;
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            if (randomFlowerChance > 0.75)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 25% 1 downgrade
                            else if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 25% double downgrade
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 25% triple downgrade
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.75)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 25% 1 downgrade
                            else if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 25% double downgrade
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 25% upgrade chance
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.8)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 20% chance of downgrade
                            else if (randomFlowerChance > 0.6)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 20% dead
                            else if (randomFlowerChance > 0.4)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 20% chance of upgrade
                            else if (randomFlowerChance > 0.2)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 20% chance of double upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.6)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 40% chance of flower death
                            else if (randomFlowerChance > 0.2)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 40% chance of upgrade
                            break;
                    }

                    FlowerBedScript.UpdateFlowerbedState(state);
                }

                break;
        }

        // Create new weather for tomorrow
        GenerateWeather();
    }

    
}

public class RandomEvent
{
    public enum EventType
    {
        Money,
        Flower
    }

    public string name;
    public string description;
    public EventType eventType;
    public float chance;
    public System.Delegate eventFunction;

    public RandomEvent (string _name, string _description, EventType _eventType, float _chance, System.Delegate _eventFunction)
    {
        name = _name;
        description = _description;
        chance = _chance;
        eventFunction = _eventFunction;
    }
}
public static class RandomEvents
{
    public static List<RandomEvent> RandomEventList = new List<RandomEvent>
    {
        new RandomEvent("Some Sweet Extra Cash!", "Your family managed to get a hold of some extra money", RandomEvent.EventType.Money, 0.10f, new System.Action(ExtraCash)),
        new RandomEvent("Someone stole your flowers!", "Your flowers were stolen by a thief!", RandomEvent.EventType.Flower, 0.02f, new System.Action(StolenFlowers)),
        new RandomEvent("Pollination", "Some of your flowers got lucky and got pollinated, making them better!", RandomEvent.EventType.Flower, 0.1f, new System.Action(Pollination)),
        new RandomEvent("Pollination Storm!!!", "All your flowers were upgraded while you were gone today!", RandomEvent.EventType.Flower, 0.01f, new System.Action(PollinationStorm))
    };

    public static void Run()
    {
        List<RandomEvent> RandomEventsToRun = new List<RandomEvent>();
        foreach (RandomEvent randomEvent in RandomEventList)
        {
            if (Random.value <= randomEvent.chance)
                RandomEventsToRun.Add(randomEvent);
        }

        foreach (RandomEvent.EventType eventType in (RandomEvent.EventType[])System.Enum.GetValues(typeof(RandomEvent.EventType)))
        {
            var randomEventsOfType = RandomEventsToRun.FindAll(randomEvent => randomEvent.eventType == eventType);

            if (randomEventsOfType.Count == 0)
                continue;

            RunEvent(randomEventsOfType[Random.Range(0, randomEventsOfType.Count)]);
        }
    }

    public static PopupManager PopupManager;
    static void RunEvent(RandomEvent randomEventToRun)
    {
        PopupManager.ShowWindowPopup(randomEventToRun.name, randomEventToRun.description);
        randomEventToRun.eventFunction.DynamicInvoke();
    }


    // Random event functions
    public static Player Player;

    // Money Events

    static void ExtraCash()
    {
        Player.money += 100;
    }


    // Flower Events
    public static FlowerBedManager FlowerBedManager;
    static FlowerBed[] FlowerBeds => FlowerBedManager.GetComponentsInChildren<FlowerBed>();

    static void StolenFlowers()
    {
        foreach (FlowerBed flowerBed in FlowerBeds)
            flowerBed.UpdateFlowerbedState(FlowerBed.FlowerBedState.Empty);
    }

    static void Pollination()
    {
        foreach (FlowerBed flowerBed in FlowerBeds)
        {
            if (Random.value > 0.5f)
                Pollinate(flowerBed);
        }
    }
    static void PollinationStorm()
    {
        foreach (FlowerBed flowerBed in FlowerBeds)
            Pollinate(flowerBed);
    }
    static void Pollinate(FlowerBed flowerBed)
    {
        FlowerBed.FlowerBedState newState = flowerBed.state;

        switch (newState)
        {
            case FlowerBed.FlowerBedState.DeadFlowers:
            case FlowerBed.FlowerBedState.DrownedFlowers:
                newState = FlowerBed.FlowerBedState.WeakFlowers;
                break;
            case FlowerBed.FlowerBedState.WeakFlowers:
                newState = FlowerBed.FlowerBedState.NormalFlowers;
                break;
            case FlowerBed.FlowerBedState.NormalFlowers:
                newState = FlowerBed.FlowerBedState.BeautifulFlowers;
                break;
            case FlowerBed.FlowerBedState.BeautifulFlowers:
                newState = FlowerBed.FlowerBedState.SuperFlowers;
                break;
        }

        flowerBed.UpdateFlowerbedState(newState);
    }
}