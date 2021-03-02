using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private void Start()
    {
        GenerateWeather();
    }

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

    void FinishDay()
    {
        Debug.Log("Finishing day!");

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
                            else if (randomFlowerChance > 0.1)
                                state = FlowerBed.FlowerBedState.NormalFlowers; // 10% chance of upgrade
                            break;
                    }
                }

                break;
        }

        // Create new weather for tomorrow
        GenerateWeather();
    }
}
