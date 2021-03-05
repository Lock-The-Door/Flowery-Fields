using UnityEngine;
using UnityEngine.UI;

public class ShopBuyButton : MonoBehaviour
{
    public Shop Shop;
    public Button BuyButton;
    public Shop.ShopItems ShopItem = Shop.ShopItems.Null;

    // Start is called before the first frame update
    void Start()
    {
        BuyButton = GetComponent<Button>();
        BuyButton.onClick.AddListener(() => Shop.SendMessage("BuyItem", ShopItem));
    }
}
