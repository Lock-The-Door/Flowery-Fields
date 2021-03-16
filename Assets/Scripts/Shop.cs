using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItem
{
    public readonly string Name;

    private int startingPrice = 0;
    private System.Func<int, int> priceInflation;
    public int Price => startingPrice + priceInflation.Invoke(Level) * Level;

    public int Level = 0;
    public readonly int MaxLevel = -1; // To ignore max level, use -1

    public int FamilyPaymentBonus;

    private System.Action<ShopItem> UpgradeAction;

    public ShopItem(string _name, int _startingPrice, System.Func<int, int> _priceInflation, int _familyPaymentBonus, System.Action<ShopItem> _upgradeAction = null, int _maxLevel = -1)
    {
        Name = _name;
        startingPrice = _startingPrice;
        priceInflation = _priceInflation;
        FamilyPaymentBonus = _familyPaymentBonus;
        UpgradeAction = _upgradeAction;
        MaxLevel = _maxLevel;
    }

    public void Upgrade()
    {
        Debug.Log("Running upgrade action for: " + Name);
        if (UpgradeAction != null)
            UpgradeAction.Invoke(this);
        else
            Debug.Log(Name + " has no attached action");
    }
}

public class Shop : MonoBehaviour
{
    public List<ShopItem> ShopItems = new List<ShopItem>
    {
        new ShopItem(_name: "Flower Beds", _startingPrice: 300, _priceInflation: level => 25 * (level + 1), _familyPaymentBonus: 10, _upgradeAction: ShopItemUpgrades.FlowerBeds, _maxLevel: 11),
        new ShopItem(_name: "Shoes", _startingPrice: 100, _priceInflation: level => 10 + 2 * level, _familyPaymentBonus: 2, _maxLevel: 25),

    };

    public Player Player;
    public GameFlow GameFlow;
    public PopupManager PopupManager;

    public StorylineManager StorylineManager;
    public FlowerBedManager FlowerBedManager;
    public CenterFarm CenterFarm;

    private void Start()
    {
        // Set manager refs for static class
        ShopItemUpgrades.Player = Player;
        ShopItemUpgrades.FlowerBedManager = FlowerBedManager;
        ShopItemUpgrades.CenterFarm = CenterFarm;
    }

    void BuyItem(ShopItem shopItem)
    {
        // See if enough money and take money away
        int cost = shopItem.Price;
        if (cost > Player.money)
        {
            Debug.Log("You are too poor! :(");
            PopupManager.ShowBottomPopup("Not enough money...", Color.red);
            return;
        }

        Player.money -= cost;

        Debug.Log($"Buying {shopItem.Name}. Was level {shopItem.Level}");

        shopItem.Upgrade(); // Run function to upgrade

        shopItem.Level++; // Increase level

        UpdateBuyButtonVisual(shopItem); // Update visuals

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

    public int totalBonnusFamilyPayment => ShopItems.Select(shopItem => shopItem.FamilyPaymentBonus * shopItem.Level).Sum();
    public bool isMaxedOut => ShopItems.Any(shopItem => shopItem.Level == shopItem.MaxLevel);
}

public static class ShopItemUpgrades
{
    // Manager references
    public static Player Player;
    public static FlowerBedManager FlowerBedManager;
    public static CenterFarm CenterFarm;

    public static void FlowerBeds(ShopItem shopItem)
    {
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
    }
}