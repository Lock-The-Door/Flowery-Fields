using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItem
{
    public readonly string Name;
    public int Price;
    public int Level = 0;
    public readonly int MaxLevel = -1; // To ignore max level, use -1

    public ShopItem(string _name, int _price, int _maxLevel = -1)
    {
        Name = _name;
        Price = _price;
        MaxLevel = _maxLevel;
    }
}

public class Shop : MonoBehaviour
{
    public List<ShopItem> ShopItems = new List<ShopItem>
    {
        new ShopItem("Flower Beds", 300, 11),
        new ShopItem("Shoes", 100, 25)
    };

    public Player Player;
    public GameFlow GameFlow;
    public PopupManager PopupManager;

    public StorylineManager StorylineManager;
    public FlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;
    void BuyItem(ShopItem shopItem)
    {
        // See if enough money
        int cost = shopItem.Price;
        if (cost > Player.money)
        {
            Debug.Log("You are too poor! :(");
            PopupManager.ShowBottomPopup("Not enough money...", Color.red);
            return;
        }

        Player.money -= cost;

        Debug.Log($"Buying {shopItem.Name}. Was level {shopItem.Level}");

        switch (shopItem.Name)
        {
            case "Flower Beds":
                FlowerBedManager.SendMessage("MakeFlowerBed", shopItem.Level); // Create Flowerbed

                // Center Farm
                switch (shopItem.Level)
                {
                    case 1:
                        CenterFarm.bottomUnlocked = true;
                        break;
                    case 4:
                        CenterFarm.topUnlocked = true;
                        break;
                    case 7:
                        CenterFarm.leftSideUnlocked = true;
                        break;
                }

                // Reprice flowerbeds
                shopItem.Price += 25 * (shopItem.Level + 1);

                GameFlow.familyPayment += 10; // Increase family payments
                break;
            case "Shoes":
                // Increase walkspeed
                Player.walkspeed += .5f;

                // Reprice shoes
                shopItem.Price += 10 + 2 * shopItem.Level;

                GameFlow.familyPayment += 2; // Increase family payments
                break;
        }

        shopItem.Level++;

        UpdateBuyButtonVisual(shopItem);

        StorylineManager.ShowStoryline("Being Generous"); // Storyline trigger
    }

    public void UpdateBuyButtonVisual(ShopItem shopItem)
    {
        Debug.Log("Update shop visual: " + shopItem);

        var buyButton = GetComponentsInChildren<ShopBuyButton>().First(buyButton => buyButton.ShopItem == shopItem);

        buyButton.text.text = "$" + shopItem.Price.ToString(); // Update price text

        // Prevent Overleveling
        if (shopItem.Level == shopItem.MaxLevel)
        {
            buyButton.BuyButton.interactable = false;
            buyButton.text.text = "Sold Out!";
        }
        else if (buyButton.BuyButton.interactable == false) // re-enable buy button
            buyButton.BuyButton.interactable = true;
    }

    public bool isMaxedOut => ShopItems.Any(shopItem => shopItem.Level == shopItem.MaxLevel);
}
