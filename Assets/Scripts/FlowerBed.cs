using System.Collections;
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
        Drowned,
        DeadFlowers,
        WeakFlowers,
        NormalFlowers,
        BeautifulFlowers,
        SuperFlowers
    }

    public Player player;
    public FlowerBedState state = FlowerBedState.Empty;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Flower Bed Clicked");

        switch(player.InHand)
        {
            case Player.Items.Nothing:
                if (state == FlowerBedState.DeadFlowers || state == FlowerBedState.WeakFlowers || state == FlowerBedState.NormalFlowers || state == FlowerBedState.BeautifulFlowers || state == FlowerBedState.SuperFlowers)
                    state = FlowerBedState.Empty;
                break;
            case Player.Items.Seeds:
                if (state == FlowerBedState.Empty)
                    state = FlowerBedState.Planted;
                break;
            case Player.Items.WateringCan:
                if (state == FlowerBedState.Planted)
                    state = FlowerBedState.Watered;
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
            case FlowerBedState.Drowned:
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
