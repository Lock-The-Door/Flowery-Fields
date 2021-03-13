using UnityEngine;
using UnityEngine.EventSystems;

public class Tool : MonoBehaviour, IPointerClickHandler
{
    public Player player;
    public Player.Items item;

    public AudioSource PickupSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Object Clicked");

        Debug.Log("Navigating to... " + transform.position.x + ", " + transform.position.y);
        player.SendMessage("Navigate", new object[]{ transform.position, gameObject });

        Debug.Log("Grabbing Item...");

        PickupSound.Play();

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
