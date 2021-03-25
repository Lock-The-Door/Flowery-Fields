using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameFlow;

public class TutorialFlow : MonoBehaviour
{
    public Weather weather = Weather.Sunny;
    public static string ToString(Weather weather) { return WeatherText[weather]; }
    static Dictionary<Weather, string> WeatherText = new Dictionary<Weather, string>()
    {
        { Weather.Sunny, "Sunny" },
        { Weather.Rainy, "Rainy" },
        { Weather.Superstorm, "Superstorm" },
        { Weather.NaturalDisaster, "Natural Disaster" }
    };

    public const int FamilyPayment = 10;

    public GameObject FlowerBeds;
    public GameObject WeatherGui;
    public List<Sprite> WeatherImages;

    public Camera Camera;
    public Light2D Light2D;
    float CameraMaxBrightness;
    Weather[] weatherTypes = (Weather[])System.Enum.GetValues(typeof(Weather));
    Dictionary<Weather, float> WeatherLightingIntensity = new Dictionary<Weather, float>()
    {
        { Weather.Sunny, 1 },
        { Weather.Rainy, 0.75f },
        { Weather.Superstorm, 0.5f },
        { Weather.NaturalDisaster, 0.75f }
    };
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
    }

    public TutorialPlayer Player;
    public AudioSource MajorClick;
    public NextDayScreen NextDayScreen;
    public PopupManager PopupManager;

    public static Objective CurrentObjective;
    private int nextObjectiveNumber = 0;

    private void Start()
    {
        Color.RGBToHSV(Camera.backgroundColor, out _, out _, out CameraMaxBrightness);

        UpdateObjective();
    }

    int Days = 1;
    IEnumerator FinishDay()
    {
        Debug.Log("Finishing a totally not rigged day");

        // Play sound
        MajorClick.Play();

        // Check if tutorial is over
        if (Days == 5)
            SceneManager.LoadScene(0);

        // Show black screen
        NextDayScreen.gameObject.SetActive(true);
        StartCoroutine(NextDayScreen.ShowScreen(++Days));
        yield return new WaitUntil(() => NextDayScreen.time >= 1);

        // PLAYER
        Player.InHand = TutorialPlayer.Items.Nothing; // Empty hands
        Player.Navigate(new List<Vector3> { new Vector3(6, 2, 1) }, playSelectSound: false); // Return to spawn loc

        // FLOWER BEDS
        // Get Flower Beds States
        var FlowerBedScripts = FlowerBeds.GetComponentsInChildren<TutorialFlowerBed>();

        // WEATHER
        Debug.Log(weather);

        // Apply "logic" to flower beds

        // Go through flowerbeds
        switch (Days)
        {
            case 2:
                foreach (var flowerBed in FlowerBedScripts)
                    flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.NormalFlowers);

                SetWeather(Weather.Rainy);
                break;
            case 3:
                foreach (var flowerBed in FlowerBedScripts)
                    flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.BeautifulFlowers);

                SetWeather(Weather.Superstorm);
                break;
            case 4:
                foreach (var flowerBed in FlowerBedScripts)
                {
                    int randomNumber = Random.Range(0, 69420);
                    if (randomNumber % 2 == 0)
                        flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.SuperFlowers);
                    else
                        flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.WeakFlowers);
                }

                SetWeather(Weather.NaturalDisaster);
                break;
            case 5:
                int flowersKilled = 0;
                foreach (var flowerBed in FlowerBedScripts)
                {
                    if (flowerBed.state == TutorialFlowerBed.FlowerBedState.WeakFlowers)
                        if (flowersKilled < 2)
                        {
                            flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.DeadFlowers);
                            flowersKilled++;
                        }
                        else
                            flowerBed.UpdateFlowerbedState(TutorialFlowerBed.FlowerBedState.SuperFlowers);
                }

                SetWeather(Weather.Sunny);
                break;
        }

        nextObjectiveNumber = 0;
        UpdateObjective();
    }

    List<List<Objective>> DailyObjectives = new List<List<Objective>>()
    {
        // Day 1
        new List<Objective>()
        {
            new Objective("Seeds", true, true, "Let's plant some flowers!", "Go and grab your flower seeds by clicking on them and then plant them by clicking on the flowerbeds!"),
            new Objective("Planted", true),
            new Objective("WateringCan", true, true, "Now we need water", "Great job! Since today is sunny, you should water you flowers. Click on the watering can and then click on all the flowerbeds."),
            new Objective("Watered", true),
            new Objective("Finish Day", true, true, "A day coming to an end", "We've done what we need to so let's go to the next day to see our flowers!")
        },

        // Day 2
        new List<Objective>()
        {
            new Objective("Shovel", true, true, "Your flowers grew!", "Just like that, you grew some flowers! Now go grab a shovel and harvest them to collect money!"),
            new Objective("Empty", true),
            new Objective("Shop", true, true, "MONEY!!!!", "Your family got super lucky and won $1200 for you somehow, you should go and spend it on some flowerbeds"),
            new Objective("Seeds", true, true, "It's going to be a rainy day", "Don't water your flowers on rainy days or during superstorms. Otherwise, your flowers will drown"),
            new Objective("Planted", false),
            new Objective("Finish Day", true, true, "Good work!", "Now we can proceed to the next day!")
        },

        // Day 3
        new List<Objective>()
        {
            new Objective("Shovel", true, true, "Rainy days are great days!", "Looks like you somehow got lucky and grew beautiful flowers. These sell for more money"),
            new Objective("Empty", false),
            new Objective("Seeds", true, true, "Weather update!", "SUPERSTORM INCOMING!!! Superstorms may kill a few flowers but they yield great rewards!"),
            new Objective("Planted", false),
            new Objective("Finish Day", true, true, "It's getting exciting", "Your first superstorm is happening, maybe you'll get a superflower and hopefully your flowers don't die.")
        },

        // Day 4
        new List<Objective>()
        {
            new Objective("Shovel", true, true, "You got some superflowers!", "You got pretty lucky with the superstorm and most of your flowers became superflowers with none of your flowers dying! Unfortunately some of your flowers became weak flowers, selling for less."),
            new Objective("Conditional Harvest", true, true, "Natural disaster incoming!!!", "Uh oh! There is going to be a natural disaster. Usually you should harvest your good flowers and keep the rest for a chance to upgrade them."),
            new Objective("Finish Day", true, true, "Hoping for the best", "Hopefully, your plants will survive. Hopefully you'll get superflowers and not dead flowers...")
        },

        // Day 5
        new List<Objective>()
        {
            new Objective("Shovel", true, true, "Oh no exactly two flowers died!", "At least you got super lucky and converted your flowers into superflowers! Dead flowers sell for nothing and will remain dead."),
            new Objective("Empty", false),
            new Objective("Finish Day", true, true, "Great Job!", "With the help of this tutorial rigging everything in your favour, you completed the tutorial! Now you know how to make some money selling flowers! Just finish the day to go back to the main menu!")
        }
    };

    public GameObject Table;
    public TutorialFlowerBedManager TutorialFlowerBedManager;
    public TutorialUIShower TutorialUIShower;
    public void UpdateObjective()
    {
        Objective nextObjective = DailyObjectives[Days-1][nextObjectiveNumber++];

        if (nextObjective.ShowPopup)
            PopupManager.ShowWindowPopup(nextObjective.IntructionTitle, nextObjective.InstructionDescription);

        CurrentObjective = nextObjective;

        // Update arrows
        Table.BroadcastMessage("ObjectiveUpdated"); // Tools
        TutorialFlowerBedManager.SendMessage("ObjectiveCheck"); // Flowerbeds
        TutorialUIShower.SendMessage("CheckForObjectives"); // Ui for finish day and shop
    }
}

public class Objective
{
    public string ObjectiveReferenceName;
    public bool ObjectiveArrow;

    public bool ShowPopup;
    public string IntructionTitle;
    public string InstructionDescription;

    public Objective(string _objectiveReferenceName, bool _objectiveArrow, bool _showPopup = false, string _instructionTitle = null, string _instructionDescription = null)
    {
        ObjectiveReferenceName = _objectiveReferenceName;
        ObjectiveArrow = _objectiveArrow;

        ShowPopup = _showPopup;
        IntructionTitle = _instructionTitle;
        InstructionDescription = _instructionDescription;
    }
}
