using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialShopItem
{
    public readonly string Name;

    private readonly int startingPrice = 0;
    private readonly System.Func<int, int> priceInflation;
    public int Price => startingPrice + priceInflation.Invoke(Level) * Level;

    public int Level = 0;
    public readonly int MaxLevel = -1; // To ignore max level, use -1

    public int FamilyPaymentBonus;
    public bool IsDowngradable;

    private readonly System.Action<TutorialShopItem> UpgradeAction;

    public TutorialShopItem(string _name, int _startingPrice, System.Func<int, int> _priceInflation, int _familyPaymentBonus, int _maxLevel = -1, bool _isDowngradable = false, System.Action<TutorialShopItem> _upgradeAction = null)
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

public class TutorialShop : MonoBehaviour
{
    public List<TutorialShopItem> ShopItems = new List<TutorialShopItem>
    {
        new TutorialShopItem(_name: "Flower Beds", _startingPrice: 300, _priceInflation: level => 25 * (level + 1), _familyPaymentBonus: 10, _maxLevel: 3, _isDowngradable: true, _upgradeAction: TutorialShopItemUpgrades.FlowerBeds)
    };

    public TutorialPlayer Player;
    public TutorialFlow GameFlow;
    public PopupManager PopupManager;

    public TutorialFlowerBedManager FlowerBedManager;

    public AudioSource BuySound;
    void BuyItem(TutorialShopItem shopItem)
    {
        // See if enough money and take money away
        if (shopItem.Price > Player.Money)
        {
            Debug.Log("You are too poor! :(");
            PopupManager.ShowBottomPopup("Not enough money...", Color.red, goodAlert: false);
            return;
        }

        Player.Money -= shopItem.Price;

        Debug.Log($"Buying {shopItem.Name}. Was level {shopItem.Level}");

        shopItem.Upgrade(); // Run function to upgrade

        BuySound.Play(); // Play buy sound
        UpdateBuyButtonVisual(shopItem); // Update visuals
    }

    public void UpdateBuyButtonVisual(TutorialShopItem shopItem)
    {
        Debug.Log("Update shop visual: " + shopItem.Name);

        var buyButton = GetComponentsInChildren<TutorialShopBuyButton>().First(buyButton => buyButton.ShopItem == shopItem);

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

    public TutorialShopItem FindByName(string shopItemName) => ShopItems.Find(shopItem => shopItem.Name == shopItemName);

    public int TotalBonusFamilyPayment => ShopItems.Select(shopItem => shopItem.FamilyPaymentBonus * shopItem.Level).Sum();
    public bool IsMaxedOut => ShopItems.TrueForAll(shopItem => shopItem.Level == shopItem.MaxLevel);
}

public static class TutorialShopItemUpgrades
{
    // Manager references
    public static TutorialPlayer Player;
    public static TutorialFlowerBedManager FlowerBedManager;
    public static CenterFarm CenterFarm;

    public static void FlowerBeds(TutorialShopItem shopItem)
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