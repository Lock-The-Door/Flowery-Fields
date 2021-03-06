using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopBuyButton : MonoBehaviour
{
    public Shop Shop;
    public Button BuyButton;
    public TextMeshProUGUI text;
    public Shop.ShopItems ShopItem = Shop.ShopItems.Null;

    // Start is called before the first frame update
    void Start()
    {
        BuyButton = GetComponent<Button>();
        BuyButton.onClick.AddListener(() => Shop.SendMessage("BuyItem", ShopItem));

        text = GetComponentInChildren<TextMeshProUGUI>();

        text.text = Shop.ShopItemPrices[ShopItem].ToString(); // Set price visual
    }
}
