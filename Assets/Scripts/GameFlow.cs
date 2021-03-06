using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum Weather
    {
        Sunny = 50,
        Rainy = 35,
        SuperStorm = 10,
        NaturalDisaster = 5
    }
    public Weather weather;

    public GameObject FlowerBeds;

    void GenerateWeather()
    {
        int randomWeatherInt = Random.Range(0,101);
        
        foreach (Weather randomWeather in (Weather[]) System.Enum.GetValues(typeof(Weather)))
        {
            if (randomWeatherInt > (int)randomWeather)
                continue;

            // Matching weather
            Debug.Log("Weather is: " + weather);
            weather = randomWeather;
            return;
        }
    }

    public Player Player;
    public bool finishedGame = false;
    public bool inDebt = false;
    void FinishDay()
    {
        Debug.Log("Finishing day!");

        // Manage Debt
        if (Player.money < 0)
        {
            if (inDebt)
                Debug.Log("Game Over, you are in debt");
            else
                inDebt = true;
        }
        else
            inDebt = false;

        // Game won?
        if (!finishedGame && Player.money > 5000)
        {
            Debug.Log("You've made a lot of money, your family is proud of you. The end! :)");
            finishedGame = true;
        }


        // PLAYER
        Player.InHand = Player.Items.Nothing; // Empty hands


        // FLOWER BEDS
        // Get Flower Beds States
        var FlowerBedScripts = FlowerBeds.GetComponentsInChildren<FlowerBed>();

        // Apply logic to flower beds
        switch (weather)
        {
            case Weather.Sunny:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    ref FlowerBed.FlowerBedState state = ref FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            state = FlowerBed.FlowerBedState.DeadFlowers;
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            state = FlowerBed.FlowerBedState.NormalFlowers;
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            state = FlowerBed.FlowerBedState.BeautifulFlowers;
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 50% chance of downgrade
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 50% chance of downgrade
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 25% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 50% chance of flower death
                            else if (randomFlowerChance > 0.4)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 10% chance of upgrade
                            break;
                    }
                }

                break;

            case Weather.Rainy:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    ref FlowerBed.FlowerBedState state = ref FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.15)
                                state = FlowerBed.FlowerBedState.NormalFlowers;
                            else
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 15% chance of better flowers
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                                state = FlowerBed.FlowerBedState.Drowned;
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
                }

                break;

            case Weather.SuperStorm:
                foreach (FlowerBed FlowerBedScript in FlowerBedScripts)
                {
                    ref FlowerBed.FlowerBedState state = ref FlowerBedScript.state;
                    float randomFlowerChance = Random.value;

                    switch (state)
                    {
                        // Normal Circumstances
                        case FlowerBed.FlowerBedState.Planted:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 50% death rate
                            else if (randomFlowerChance > 0.375)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 25% chance of surviving flowers are better (12.5%)
                            else
                                state = FlowerBed.FlowerBedState.NormalFlowers;
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            state = FlowerBed.FlowerBedState.Drowned;
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
                            if (randomFlowerChance > 0.3)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 70% 1 downgrade
                            else if (randomFlowerChance > 0.15)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 15% double downgrade
                            else if (randomFlowerChance > 0.14)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 1% triple downgrade
                            else if (randomFlowerChance < 0.04)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 10% upgrade chance
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.5)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 50% chance of downgrade
                            else
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 50% chance of upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 75% chance of flower death
                            else if (randomFlowerChance > 0.10)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 15% chance of upgrade
                            break;
                    }
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
                            if (randomFlowerChance > 0.999)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 0.1% chance for super flowers
                            else if (randomFlowerChance > 0.994)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 0.5% chance for normal flowers
                            else if (randomFlowerChance > 0.984)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 1% chance for weak flowers
                            break;
                        case FlowerBed.FlowerBedState.Watered:
                            state = FlowerBed.FlowerBedState.Drowned;
                            break;

                        // Unharvested Flowers
                        case FlowerBed.FlowerBedState.SuperFlowers:
                            if (randomFlowerChance > 0.75)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 25% 1 downgrade
                            else if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 50% double downgrade
                            else
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 25% triple downgrade
                            break;
                        case FlowerBed.FlowerBedState.BeautifulFlowers:
                            if (randomFlowerChance > 0.65)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 35% 1 downgrade
                            else if (randomFlowerChance > 0.15)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 50% double downgrade
                            else if (randomFlowerChance > 0.1)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 5% triple downgrade
                            else if (randomFlowerChance > 0.05)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 5% upgrade chance
                            break;
                        case FlowerBed.FlowerBedState.NormalFlowers:
                            if (randomFlowerChance > 0.25)
                                state = FlowerBed.FlowerBedState.WeakFlowers; // 75% chance of downgrade
                            else if (randomFlowerChance > 0.15)
                                state = FlowerBed.FlowerBedState.BeautifulFlowers; // 10% chance of upgrade
                            else if (randomFlowerChance > 0.1)
                                state = FlowerBed.FlowerBedState.SuperFlowers; // 5% chance of double upgrade
                            break;
                        case FlowerBed.FlowerBedState.WeakFlowers:
                            if (randomFlowerChance > 0.01)
                                state = FlowerBed.FlowerBedState.DeadFlowers; // 99% chance of flower death
                            else if (randomFlowerChance > 0.005)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 0.5% chance of upgrade
                            break;
                    }
                }

                break;
        }

        // Create new weather for tomorrow
        GenerateWeather();
    }
}
