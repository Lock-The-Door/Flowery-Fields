using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialShopBuyButton : MonoBehaviour
{
    public TutorialShop Shop;
    public Button BuyButton;
    public TextMeshProUGUI text;
    public string ShopItemName;
    public TutorialShopItem ShopItem;

    // Start is called before the first frame update
    void Start()
    {
        Shop = GetComponentInParent<TutorialShop>(); // Set shop

        // Get shop item from name
        ShopItem = Shop.ShopItems.FirstOrDefault(shopItem => shopItem.Name == ShopItemName);
        if (ShopItem == null)
        {
            Debug.LogWarning("Couldn't find shop item object for " + ShopItemName);
            BuyButton.interactable = false; // disable button
            text.text = "--"; // update buy button text
            return;
        }

        BuyButton.onClick.AddListener(() => Shop.SendMessage("BuyItem", ShopItem));

        Shop.UpdateBuyButtonVisual(ShopItem);
    }
}
