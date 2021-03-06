using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FlowerBed : MonoBehaviour, IPointerClickHandler
{
    public enum FlowerBedState
    {
        Locked,
        Empty,
        Planted,
        Watered,
        DrownedFlowers,
        DeadFlowers,
        WeakFlowers,
        NormalFlowers,
        BeautifulFlowers,
        SuperFlowers
    }

    public Player player;
    public FlowerBedState state = FlowerBedState.Empty;
    public const int SeedsPrice = 7;
    public const int WaterPrice = 1;
    private Dictionary<FlowerBedState, int> FlowerSellPrice = new Dictionary<FlowerBedState, int> { 
        {FlowerBedState.DeadFlowers, 0},
        {FlowerBedState.DrownedFlowers, 0},
        {FlowerBedState.WeakFlowers, 5},
        {FlowerBedState.NormalFlowers, 15},
        {FlowerBedState.BeautifulFlowers, 50},
        {FlowerBedState.SuperFlowers, 100},
    };

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Flower Bed Clicked");

        Debug.Log("Navigating to... " + transform.position.x + ", " + transform.position.y);
        player.SendMessage("Navigate", new object[] { transform.position, gameObject });

        Debug.Log("Doing action...");

        switch(player.InHand)
        {
            case Player.Items.Shovel:
                if (FlowerSellPrice.TryGetValue(state, out int sellPrice))
                {
                    state = FlowerBedState.Empty;
                    player.money += sellPrice;
                }
                break;
            case Player.Items.Seeds:
                if (state == FlowerBedState.Empty)
                {
                    if (player.money < SeedsPrice)
                    {
                        Debug.Log("Not enough money...");
                        return;
                    }
                    state = FlowerBedState.Planted;
                    player.money -= SeedsPrice; 
                }
                break;
            case Player.Items.WateringCan:
                if (state == FlowerBedState.Planted)
                {
                    if (player.money < WaterPrice)
                    {
                        Debug.Log("Not enough money...");
                        return;
                    }
                    state = FlowerBedState.Watered;
                    player.money -= WaterPrice;
                }
                break;
        }
    }

    void Update()
    {
        SpriteRenderer SpriteRenderer = GetComponent<SpriteRenderer>();

        switch (state)
        {
            case FlowerBedState.Empty:
                SpriteRenderer.color = Color.white;
                break;
            case FlowerBedState.Planted:
                SpriteRenderer.color = new Color(0.439f, 0.231f, 0.184f);
                break;
            case FlowerBedState.Watered:
                SpriteRenderer.color = Color.cyan;
                break;
            case FlowerBedState.DrownedFlowers:
                SpriteRenderer.color = Color.blue;
                break;
            case FlowerBedState.DeadFlowers:
                SpriteRenderer.color = Color.black;
                break;
            case FlowerBedState.WeakFlowers:
                SpriteRenderer.color = Color.grey;
                break;
            case FlowerBedState.NormalFlowers:
                SpriteRenderer.color = Color.green;
                break;
            case FlowerBedState.BeautifulFlowers:
                SpriteRenderer.color = Color.yellow;
                break;
            case FlowerBedState.SuperFlowers:
                SpriteRenderer.color = Color.red;
                break;
        }
    }
}
