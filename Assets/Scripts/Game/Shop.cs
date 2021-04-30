using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItem
{
    public readonly string Name;

    private readonly int startingPrice = 0;
    private readonly System.Func<int, int> priceInflation;
    public int Price => startingPrice + priceInflation.Invoke(Level) * Level;

    public int Level = 0;
    public readonly int MaxLevel = -1; // To ignore max level, use -1

    public int FamilyPaymentBonus;
    public bool IsDowngradable;

    private readonly System.Action<ShopItem> UpgradeAction;

    public ShopItem(string _name, int _startingPrice, System.Func<int, int> _priceInflation, int _familyPaymentBonus, int _maxLevel = -1, bool _isDowngradable = false, System.Action<ShopItem> _upgradeAction = null)
    {
        Name = _name;
        startingPrice = _startingPrice;
        priceInflation = _priceInflation;
        FamilyPaymentBonus = _familyPaymentBonus;
        MaxLevel = _maxLevel;
        IsDowngradable = _isDowngradable;
        UpgradeAction = _upgradeAction;
    }

    public void Upgrade()
    {
        Level++; // Increase level

        Debug.Log("Running upgrade action for: " + Name);
        if (UpgradeAction != null)
            UpgradeAction.Invoke(this);
        else
            Debug.Log(Name + " has no attached action");
    }
}

public class Shop : MonoBehaviour
{
    public static List<ShopItem> ShopItems = new List<ShopItem>
    {
        new ShopItem(_name: "Flower Beds", _startingPrice: 300, _priceInflation: level => 25 * (level + 1), _familyPaymentBonus: 10, _maxLevel: 11, _isDowngradable: true, _upgradeAction: ShopItemUpgrades.FlowerBeds),
        new ShopItem(_name: "Shoes", _startingPrice: 150, _priceInflation: level => 10 + 2 * level, _familyPaymentBonus: 2, _maxLevel: 25, _isDowngradable: true),
        new ShopItem(_name: "Security", _startingPrice: 250, _priceInflation: level => 50 + 5 * level, _familyPaymentBonus: 15, _maxLevel: 5, _isDowngradable: true),
        new ShopItem(_name: "Luck", _startingPrice: 375, _priceInflation: level => 100 + 10 * level, _familyPaymentBonus: 45, _maxLevel: 5),
        new ShopItem(_name: "Better bees", _startingPrice: 200, _priceInflation: level => 50 + 10 * level, _familyPaymentBonus: 20, _maxLevel: 3),
        new ShopItem(_name: "Discounts", _startingPrice: 500, _priceInflation: level => 25 + 25 * level, _familyPaymentBonus: 25, _maxLevel: 5)
    };

    public Player Player;
    public GameFlow GameFlow;
    public PopupManager PopupManager;

    public StorylineManager StorylineManager;
    public FlowerBedManager FlowerBedManager;

    public AudioSource BuySound;
    public void BuyItem(ShopItem shopItem)
    {
        // See if enough money and take money away
        var discountShopItem = ShopItems.Find(shopItem => shopItem.Name == "Discounts");
        int cost = Mathf.RoundToInt(shopItem.Price * (1 - (float)discountShopItem.Level / discountShopItem.MaxLevel * 0.5f));
        if (cost > Player.Money)
        {
            Debug.Log("You are too poor! :(");
            PopupManager.ShowBottomPopup("Not enough money...", Color.red, goodAlert: false);
            return;
        }

        Player.Money -= cost;

        Debug.Log($"Buying {shopItem.Name}. Was level {shopItem.Level}");

        shopItem.Upgrade(); // Run function to upgrade

        BuySound.Play(); // Play buy sound
        UpdateBuyButtonVisuals(); // Update visuals

        StorylineManager.ShowStoryline("Being Generous"); // Storyline trigger
    }

    public void UpdateBuyButtonVisuals() => ShopItems.ForEach(shopItem => UpdateBuyButtonVisual(shopItem));

    public void UpdateBuyButtonVisual(ShopItem shopItem)
    {
        Debug.Log("Update shop visual: " + shopItem.Name);

        var buyButton = GetComponentsInChildren<ShopBuyButton>().First(buyButton => buyButton.ShopItemName == shopItem.Name);

        var discountShopItem = ShopItems.Find(shopItem => shopItem.Name == "Discounts");
        int cost = Mathf.RoundToInt(shopItem.Price * (1 - (float)discountShopItem.Level / discountShopItem.MaxLevel * 0.5f));
        buyButton.text.text = "$" + cost.ToString(); // Update price text

        // Prevent Overleveling
        if (shopItem.Level == shopItem.MaxLevel)
        {
            buyButton.BuyButton.interactable = false;
            buyButton.text.text = "Sold Out!";
        }
        else if (buyButton.BuyButton.interactable == false) // re-enable buy button
            buyButton.BuyButton.interactable = true;
    }

    public ShopItem FindByName(string shopItemName) => ShopItems.Find(shopItem => shopItem.Name == shopItemName);

    public int TotalBonusFamilyPayment => ShopItems.Select(shopItem => shopItem.FamilyPaymentBonus * shopItem.Level).Sum();
    public bool IsMaxedOut => ShopItems.TrueForAll(shopItem => shopItem.Level == shopItem.MaxLevel);
}

public static class ShopItemUpgrades
{
    // Manager references
    public static Player Player;
    public static FlowerBedManager FlowerBedManager;
    public static CenterFarm CenterFarm;

    public static void FlowerBeds(ShopItem shopItem)
    {
        FlowerBedManager.SendMessage("MakeFlowerBed", shopItem.Level - 1); // Create Flowerbed

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
    }
}