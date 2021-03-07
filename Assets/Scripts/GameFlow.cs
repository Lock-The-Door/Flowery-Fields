using TMPro;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum Weather
    {
        Sunny = 55,
        Rainy = 30,
        SuperStorm = 10,
        NaturalDisaster = 5
    }
    public Weather weather;

    public GameObject FlowerBeds;

    void Start() => GenerateWeather();
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
            weatherText.text = weather.ToString(); // Update text --temp, will be replaced with images
            return;
        }
    }

    public Player Player;
    public PopupManager PopupManager;
    public TextMeshProUGUI weatherText;
    public Shop Shop;
    public FlowerBedManager FlowerBedManager;
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
                int level = Shop.ShopItemLevels[Shop.ShopItems.FlowerBeds];
                if (level > 0)
                {
                    // Display message
                    PopupManager.ShowWindowPopup("You sold a flowerbed", "Since you were still in debt, you were forced to sell a flowerbed to help you get back on your feet.");

                    Shop.ShopItemPrices[Shop.ShopItems.FlowerBeds] -= 25 * level;// Lower flowerbed price
                    FlowerBedManager.SendMessage("RemoveFlowerBed", --Shop.ShopItemLevels[Shop.ShopItems.FlowerBeds]); // Sell flowerbed
                    Player.money += 500; // Return $500
                    Shop.UpdateBuyButtonVisual(Shop.ShopItems.FlowerBeds); // Update visuals
                }
                else
                {
                    // Display message
                    PopupManager.ShowWindowPopup("You've lost everything...", "Sadly, you've ended up with less money than you've started with. Luckily for you, your parents were nice enough to pay for your debts and give you a fresh start.");

                    Player.money = 100;
                }
            }
            else
            {
                inDebt = true;
                PopupManager.ShowWindowPopup("You're in debt!", "You are in debt! Get out of debt or you'll soon need to start selling your flowerbeds!");
            }
        }
        else
            inDebt = false;

        // Game won?
        if (!finishedGame && Player.money > 5000 && Shop.isMaxedOut)
        {
            Debug.Log("You've made a lot of money, your family is proud of you. The end! :)");
            PopupManager.ShowWindowPopup("You did it!", "You've made a lot of money, your family is proud of you. The end! :)");
            finishedGame = true;
        }


        // PLAYER
        Player.InHand = Player.Items.Nothing; // Empty hands


        // FLOWER BEDS
        // Get Flower Beds States
        var FlowerBedScripts = FlowerBeds.GetComponentsInChildren<FlowerBed>();


        // WEATHER
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
                }

                break;
        }

        // Create new weather for tomorrow
        GenerateWeather();
    }
}
