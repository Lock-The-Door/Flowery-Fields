using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public enum ShopItems
    {
        Null,
        FlowerBeds
    }

    public Dictionary<ShopItems, int> ShopItemPrices = new Dictionary<ShopItems, int>
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

        var buyButton = GetComponentsInChildren<ShopBuyButton>().First(buyButton => buyButton.ShopItem == shopItem);

        buyButton.text.text = ShopItemPrices[shopItem].ToString(); // Update price visuals

        // Prevent Overleveling
        if (ShopItemLevels.TryGetValue(shopItem, out int currentLevel) && currentLevel == ShopItemMaxLevels[shopItem])
        {
            buyButton.BuyButton.interactable = false;
            buyButton.text.GetComponent<RectTransform>().sizeDelta = new Vector2(buyButton.text.GetComponent<RectTransform>().sizeDelta.x, 90); // change text size
            buyButton.text.text = "Sold Out!";
            buyButton.transform.Find("Money Icon").gameObject.SetActive(false); // Disable money icon image
            // Change buy button image
        }
    }
}
