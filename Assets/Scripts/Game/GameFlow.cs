using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour
{
    public enum Weather
    {
        Sunny = 55,
        Rainy = 30,
        Superstorm = 10,
        NaturalDisaster = 5
    }
    public Weather weather = Weather.Sunny;
    public static string ToString(Weather weather) { return WeatherText[weather]; }
    static readonly Dictionary<Weather, string> WeatherText = new Dictionary<Weather, string>()
    {
        { Weather.Sunny, "Sunny" },
        { Weather.Rainy, "Rainy" },
        { Weather.Superstorm, "Superstorm" },
        { Weather.NaturalDisaster, "Natural Disaster" }
    };

    public int FamilyPayment => 10 + Shop.TotalBonusFamilyPayment;

    public GameObject FlowerBeds;

    public StorylineManager StorylineManager;

    void Start()
    {
        // Set vars
        Color.RGBToHSV(Camera.backgroundColor, out float _, out float _, out CameraMaxBrightness);

        // Show today's storyline
        StorylineManager.CheckForNewStoryline(Days);

        if (GameStatics.NewGame) // only do this if it's a new game
        {
            DiscordRichPresenceManager.UpdateActivity("Playing", Days); // Rich Presence 1st day
            SetWeather(Weather.Sunny); // Always sunny first day
        }
        else
        {
            StartCoroutine(WaitForData());
        }

        // Set variables in randomiser
        RandomEvents.Player = Player;
        RandomEvents.Shop = Shop;
        RandomEvents.PopupManager = PopupManager;
        RandomEvents.FlowerBedManager = FlowerBedManager;
    }

    IEnumerator WaitForData()
    {
        if (GameStatics.Loading)
            yield return new WaitUntil(() => !GameStatics.Loading);

        // Code after wait
        DiscordRichPresenceManager.UpdateActivity("Playing", Days); // Load rich presence
    }

    void Update() => DiscordRichPresenceManager.RunCallbacks();

    readonly Dictionary<Weather, float> WeatherLightingIntensity = new Dictionary<Weather, float>()
    {
        { Weather.Sunny, 1 },
        { Weather.Rainy, 0.75f },
        { Weather.Superstorm, 0.5f },
        { Weather.NaturalDisaster, 0.75f }
    };

    public Camera Camera;
    float CameraMaxBrightness;
    readonly Weather[] weatherTypes = (Weather[])System.Enum.GetValues(typeof(Weather));
    void GenerateWeather()
    {
        int randomWeatherInt = Random.Range(0,101);

        foreach (Weather randomWeather in weatherTypes)
        {
            Debug.Log(randomWeatherInt);
            Debug.Log(randomWeather);

            if (randomWeatherInt > (int)randomWeather)
            {
                randomWeatherInt -= (int)randomWeather;
                continue;
            }

            // Matching weather
            SetWeather(randomWeather);
            break;
        }
    }
    public void SetWeather(Weather newWeather)
    {
        weather = newWeather;

        // Post weather generation
        Debug.Log("Weather is: " + weather);
        WeatherGui.GetComponentInChildren<TextMeshProUGUI>().text = ToString(weather); // Update text
        WeatherGui.GetComponent<Image>().sprite = WeatherImages[System.Array.IndexOf(weatherTypes.Reverse().ToArray(), weather)]; // Update Image

        // Change lighting
        // Camera bg
        Color.RGBToHSV(Camera.backgroundColor, out float BgH, out float BgS, out float _);
        Camera.backgroundColor = Color.HSVToRGB(BgH, BgS, CameraMaxBrightness * WeatherLightingIntensity[weather]);
        // Light 2d
        Light2D.intensity = WeatherLightingIntensity[weather];

        StorylineManager.ShowStoryline(ToString(weather)); // Storyline
    }

    public Player Player;
    public PopupManager PopupManager;
    public FlowerBedManager FlowerBedManager;
    public GameObject WeatherGui;
    public List<Sprite> WeatherImages;
    public Shop Shop;
    public BorrowMoney BorrowMoney;
    public bool finishedGame = false;
    public bool InDebt = false;

    public Dictionary<Weather, Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>> WeatherLogicData = new Dictionary<Weather, Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>>
    {
        { Weather.Sunny,
            new Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>
            {
                // Normal Circumstances
                { FlowerBed.FlowerBedState.Planted,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.50f }, // Most flowers will die without water
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.05f } // Small chance of weak flowers
                        // Nothing happens to the rest of the flowers
                    }
                },
                { FlowerBed.FlowerBedState.Watered,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.90f }, // 90% normal
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.05f }, // 5% chance of better flowers
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.04f }, // 4% chance of weak flowers
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.01f } // 1% chance of super flowers
                    }
                },

                // Unharvested Flowers
                { FlowerBed.FlowerBedState.SuperFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 1.00f }
                    }
                },
                { FlowerBed.FlowerBedState.BeautifulFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.45f }, // 45% chance of downgrade
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.10f } // 10% chance of upgrade
                    }
                },
                { FlowerBed.FlowerBedState.NormalFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.15f }, // 15% chance of downgrade
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.25f } // 25% chance of upgrade
                    }
                },
                { FlowerBed.FlowerBedState.WeakFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.50f }, // 50% chance of flower death
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.25f } // 25% chance of upgrade
                    }
                }
            }
        },

        { Weather.Rainy,
            new Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>
            {
                // Normal Circumstances
                { FlowerBed.FlowerBedState.Planted,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.80f }, // 80% chance of normal flowers
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.18f }, // 18% chance of better flowers
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.02f } // 2% chance of super flowers
                    }
                },
                { FlowerBed.FlowerBedState.Watered,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DrownedFlowers, 0.95f },
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.05f } // rare chance of surviving flowers
                    }
                },

                // Unharvested Flowers
                { FlowerBed.FlowerBedState.SuperFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 1.00f }
                    }
                },
                { FlowerBed.FlowerBedState.BeautifulFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.30f }, // 30% chance of downgrade
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.10f } // 10% chance of upgrade
                    }
                },
                { FlowerBed.FlowerBedState.NormalFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.30f }, // 30% chance of downgrade
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.30f } // 30% chance of upgrade
                    }
                },
                { FlowerBed.FlowerBedState.WeakFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.30f }, // 30% chance of flower death
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.30f } // 30% chance of upgrade
                    }
                }
            }
        },

        { Weather.Superstorm,
            new Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>
            {
                // Normal Circumstances
                { FlowerBed.FlowerBedState.Planted,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.50f }, // 50% death rate
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.20f }, // 20% flowers are better
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.15f }, // 5% of flowers are super flowers
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.15f } // rest 15% are normal
                    }
                },
                { FlowerBed.FlowerBedState.Watered,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DrownedFlowers, 1.00f }
                    }
                },

                // Unharvested Flowers
                { FlowerBed.FlowerBedState.SuperFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.75f }, // 75% 1 downgrade
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.20f }, // 20% double downgrade
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.05f } // 5% triple downgrade
                    }
                },
                { FlowerBed.FlowerBedState.BeautifulFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.40f }, // 40% 1 downgrade
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.10f }, // 10% double downgrade
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.01f }, // 1% triple downgrade
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.20f } // 20% upgrade chance
                    }
                },
                { FlowerBed.FlowerBedState.NormalFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.25f }, // 25% chance of downgrade
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.50f } // 50% chance of upgrade
                    }
                },
                { FlowerBed.FlowerBedState.WeakFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.50f }, // 50% chance of flower death
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.25f } // 25% chance of upgrade
                    }
                }
            }
        },

        { Weather.NaturalDisaster,
            new Dictionary<FlowerBed.FlowerBedState, Dictionary<FlowerBed.FlowerBedState, float>>
            {
                // Normal Circumstances
                { FlowerBed.FlowerBedState.Planted,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.05f }, // 5% of flowers are super flowers
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.05f }, // 5% chance for beautiful flowers
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.05f }, // 5% chance for normal flowers
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.05f }, // 5% chance for weak flowers
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.80f } // otherwise, dead
                    }
                },
                { FlowerBed.FlowerBedState.Watered,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.06f }, // 6% of flowers are super flowers
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.06f }, // 6% chance for beautiful flowers
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.06f }, // 6% chance for normal flowers
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.06f }, // 6% chance for weak flowers
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.76f } // otherwise, dead
                    }
                },

                // Unharvested Flowers
                { FlowerBed.FlowerBedState.SuperFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.25f }, // 25% 1 downgrade
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.25f }, // 25% double downgrade
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.25f } // 25% triple downgrade
                    }
                },
                { FlowerBed.FlowerBedState.BeautifulFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.25f }, // 25% 1 downgrade
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.25f }, // 25% double downgrade
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.25f } // 25% upgrade chance
                    }
                },
                { FlowerBed.FlowerBedState.NormalFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.SuperFlowers, 0.20f }, // 20% chance of double upgrade
                        { FlowerBed.FlowerBedState.BeautifulFlowers, 0.20f }, // 20% chance of upgrade
                        { FlowerBed.FlowerBedState.WeakFlowers, 0.20f }, // 20% chance of downgrade
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.20f }, // 20% dead
                    }
                },
                { FlowerBed.FlowerBedState.WeakFlowers,
                    new Dictionary<FlowerBed.FlowerBedState, float>
                    {
                        { FlowerBed.FlowerBedState.DeadFlowers, 0.40f }, // 40% chance of flower death
                        { FlowerBed.FlowerBedState.NormalFlowers, 0.40f } // 40% chance of upgrade
                    }
                }
            }
        }
    };

    
    public Light2D Light2D;
    public AudioSource MajorClick;
    public NextDayScreen NextDayScreen;
    public GameFunctions GameFunctions;
    public int Days = 1;
    IEnumerator FinishDay()
    {
        Debug.Log("Finishing day!");

        GameStatics.Loading = true;

        // Play sound
        MajorClick.Play();

        Player.Navigate(new List<Vector3> { new Vector3(6, 2, 1) }, playSelectSound: false); // Return to spawn loc

        // Show black screen
        NextDayScreen.gameObject.SetActive(true);
        StartCoroutine(NextDayScreen.ShowScreen(++Days));

        yield return new WaitUntil(() => NextDayScreen.time >= 1);

        DiscordRichPresenceManager.UpdateActivity("Playing", Days); // Discord Rich Presence

        // Manage Debt
        if (Player.Money < 0)
        {
            if (InDebt)
            {
                Debug.Log("You are in debt");
                // Try to sell flowerbeds first to cover debt
                var shopItems = Shop.ShopItems.FindAll(shopItem => shopItem.IsDowngradable && shopItem.Level > 0);
                shopItems.Sort((shopItem1, shopItem2) => shopItem1.Price.CompareTo(shopItem2.Price));
                if (shopItems.Count == 0)
                {
                    // Display message
                    PopupManager.ShowWindowPopup("You've lost everything...", "Sadly, you've ended up with less money than you've started with. Luckily for you, your parents were nice enough to pay for your debts and give you a fresh start.", goodAlert: false);

                    // Reset values
                    Player.Money = 100;
                }
                else
                {
                    int shopItemIndex = 0;
                    while (shopItems[shopItemIndex].Price + Player.Money < 0 && shopItemIndex < shopItems.Count)
                        shopItemIndex++;

                    // Display message
                    PopupManager.ShowWindowPopup($"You sold your {shopItems[shopItemIndex].Name}", $"Since you were still in debt, you were forced to sell some things to help you get back on your feet.", goodAlert: false);

                    Player.Money += shopItems[shopItemIndex].Price; // Return money
                    int newLevel = --shopItems[shopItemIndex].Level;

                    if (shopItems[shopItemIndex].Name == "Flower Beds") // flower bed edge case
                        FlowerBedManager.SendMessage("RemoveFlowerBed", newLevel); // Sell flowerbed

                    Shop.UpdateBuyButtonVisual(shopItems[shopItemIndex]); // Update visuals
                }
            }
            else
            {
                InDebt = true;
                PopupManager.ShowWindowPopup("You're in debt!", "You are in debt! Get out of debt or you'll soon need to start selling your things!", goodAlert: false);
            }
        }
        else
        {
            InDebt = false;

            Player.Money -= FamilyPayment; // Pay family
            Player.Money -= BorrowMoney.TotalDailyPayment; // Pay loans
            BorrowMoney.UpdateDailyPayments();
        }

        // Game won?
        if (!finishedGame && Player.Money > 5000 && Shop.IsMaxedOut)
        {
            Debug.Log($"You've made a lot of money, your family is proud of you. The end! :) ({Days} days to complete game)");
            StorylineManager.ShowStoryline("The End");
            finishedGame = true;
        }


        // PLAYER
        Player.InHand = Player.Items.Nothing; // Empty hands

        // STORYLINE
        StorylineManager.CheckForNewStoryline(Days);
        
        // FLOWER BEDS
        // Get Flower Beds States
        var FlowerBedScripts = FlowerBeds.GetComponentsInChildren<FlowerBed>();


        // WEATHER
        Debug.Log(weather);

        // Apply logic to flower beds

        // Get chances for weather
        if (!WeatherLogicData.TryGetValue(weather, out var flowerbedStateChances))
            WeatherLogicData.TryGetValue(Weather.Sunny, out flowerbedStateChances);

        // Go through flowerbeds
        foreach (FlowerBed flowerBed in FlowerBedScripts)
        {
            if (!flowerbedStateChances.TryGetValue(flowerBed.state, out var chances))
                continue;
            float randomChance = Random.value;

            foreach (var possibleChance in chances)
            {
                if (possibleChance.Value > randomChance)
                {
                    flowerBed.UpdateFlowerbedState(possibleChance.Key);
                    break;
                }

                randomChance -= possibleChance.Value;
            }
        }

        RandomEvents.Run(); // Random events

        // Create new weather for tomorrow
        GenerateWeather();

        GameFunctions.SaveGame(true, "finish_day_backup").Wait(); // Save data everyday while making backup copies just in case

        GameStatics.Loading = false;
    }
}

public class RandomEvent
{
    public enum RandomEventType
    {
        Money,
        Flower
    }

    ShopItem ShopItem => RandomEvents.Shop.FindByName(ShopItemName);
    readonly string ShopItemName;

    public readonly string Name;
    public readonly string Description;
    public readonly bool IsGoodEvent;
    public readonly RandomEventType EventType;
    public readonly int StartingChance;
    public int Chance => IsGoodEvent ? StartingChance * (ShopItem.Level + 1) : StartingChance * (1 - ShopItem.Level / ShopItem.MaxLevel);
    public readonly System.Delegate eventFunction;

    public RandomEvent(string _name, string _description, bool _isGoodEvent, RandomEventType _eventType, int _chance, System.Delegate _eventFunction, string _shopItemName)
    {
        ShopItemName = _shopItemName;

        Name = _name;
        Description = _description;
        IsGoodEvent = _isGoodEvent;
        EventType = _eventType;
        StartingChance = _chance;
        eventFunction = _eventFunction;
    }
}
public static class RandomEvents
{
    public static List<RandomEvent> RandomEventList = new List<RandomEvent>
    {
        new RandomEvent("Some Sweet Extra Cash!", "Your family managed to get a hold of some extra money", true, RandomEvent.RandomEventType.Money, 10, new System.Action(ExtraCash), "Luck"),
        new RandomEvent("Someone stole your flowers!", "Your flowers were stolen by a thief!", false, RandomEvent.RandomEventType.Flower, 2, new System.Action(StolenFlowers), "Security"),
        new RandomEvent("Pollination", "Some of your flowers got lucky and got pollinated, making them better!", true, RandomEvent.RandomEventType.Flower, 10, new System.Action(Pollination), "Luck"),
        new RandomEvent("Pollination Storm!!!", "All your flowers were upgraded while you were gone today!", true, RandomEvent.RandomEventType.Flower, 1, new System.Action(PollinationStorm), "Luck")
    };

    public static void Run()
    {
        Debug.Log("Running random events");

        List<RandomEvent> RandomEventsToRun = new List<RandomEvent>();
        foreach (RandomEvent randomEvent in RandomEventList)
        {
            if (Random.Range(0, 101) <= randomEvent.Chance)
                RandomEventsToRun.Add(randomEvent);
        }

        foreach (RandomEvent.RandomEventType eventType in (RandomEvent.RandomEventType[])System.Enum.GetValues(typeof(RandomEvent.RandomEventType)))
        {
            var randomEventsOfType = RandomEventsToRun.FindAll(randomEvent => randomEvent.EventType == eventType);

            if (randomEventsOfType.Count == 0)
                continue;

            RunEvent(randomEventsOfType[Random.Range(0, randomEventsOfType.Count)]);
        }
    }

    public static PopupManager PopupManager;
    static void RunEvent(RandomEvent randomEventToRun)
    {
        Debug.Log("Running random event: " + randomEventToRun.Name);

        PopupManager.ShowWindowPopup(randomEventToRun.Name, randomEventToRun.Description, goodAlert: randomEventToRun.IsGoodEvent);
        randomEventToRun.eventFunction.DynamicInvoke();
    }


    // Random event functions
    public static Player Player;
    public static Shop Shop;

    // Money Events

    static void ExtraCash()
    {
        Player.Money += 100;
    }


    // Flower Events
    public static FlowerBedManager FlowerBedManager;
    static FlowerBed[] FlowerBeds => FlowerBedManager.GetComponentsInChildren<FlowerBed>();

    static void StolenFlowers()
    {
        foreach (FlowerBed flowerBed in FlowerBeds)
            flowerBed.UpdateFlowerbedState(FlowerBed.FlowerBedState.Empty);
    }

    static int PollinationCycles => Shop.ShopItems.Find(shopItem => shopItem.Name == "Better bees").Level + 1; // calculate number of times to try for pollination from pollination level
    static void Pollination()
    {
        for (int pollinationCycle = 0; pollinationCycle < PollinationCycles; pollinationCycle++)
        {
            foreach (FlowerBed flowerBed in FlowerBeds)
            {
                if (Random.value > 0.5f)
                    Pollinate(flowerBed);
            }
        }
    }
    static void PollinationStorm()
    {
        foreach (FlowerBed flowerBed in FlowerBeds)
            Pollinate(flowerBed);

        Pollination(); // Run a secondary pollination cycle
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