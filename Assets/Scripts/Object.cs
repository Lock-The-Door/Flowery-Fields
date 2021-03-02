using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Object : MonoBehaviour, IPointerClickHandler
{
    public Player player;
    public Player.Items item;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Object Clicked");
        if (player.InHand != item)
            player.InHand = item;
        else
            player.InHand = Player.Items.Nothing;
    }

    void Update()
    {
        if (player.InHand == item)
            GetComponent<SpriteRenderer>().enabled = false;
        else
            GetComponent<SpriteRenderer>().enabled = true;
    }
}
