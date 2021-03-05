using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public enum ShopItems
    {
        Null,
        FlowerBeds
    }

    Dictionary<ShopItems, int> ShopItemPrices = new Dictionary<ShopItems, int>
    {
        { ShopItems.FlowerBeds, 300 }
    };

    Dictionary<ShopItems, int> ShopItemLevels = new Dictionary<ShopItems, int>
    {
        { ShopItems.FlowerBeds, 0 }
    };
    Dictionary<ShopItems, int> ShopItemMaxLevels = new Dictionary<ShopItems, int>
    {
        { ShopItems.FlowerBeds, 11 }
    };

    public Player Player;

    public FlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;
    void BuyItem(ShopItems shopItem)
    {
        // See if enough money
        int cost = ShopItemPrices[shopItem];
        if (cost > Player.money)
        {
            Debug.Log("You are too poor! :(");
            return;
        }

        Player.money -= cost;

        switch (shopItem)
        {
            case ShopItems.FlowerBeds:
                Debug.Log("Buying Flower Beds...");

                FlowerBedManager.SendMessage("MakeFlowerBed", ShopItemLevels[ShopItems.FlowerBeds]++); // Create Flowerbed

                // Center Farm
                switch (ShopItemLevels[ShopItems.FlowerBeds])
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
                ShopItemPrices[ShopItems.FlowerBeds] += 25 * ShopItemLevels[ShopItems.FlowerBeds];

                break;
        }

        // Prevent Overleveling
        if (ShopItemLevels.TryGetValue(shopItem, out int currentLevel) && currentLevel == ShopItemMaxLevels[shopItem])
        {
            var buyButton = GetComponentsInChildren<ShopBuyButton>().First(buyButton => buyButton.ShopItem == shopItem);
            buyButton.BuyButton.interactable = false;
            // Change buy button image
        }
    }
}
