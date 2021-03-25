using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialTool : MonoBehaviour, IPointerClickHandler
{
    public TutorialFlow TutorialFlow;
    public TutorialPlayer player;
    public TutorialPlayer.Items item;

    public AudioSource PickupSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Object Clicked");

        Debug.Log("Navigating to... " + transform.position.x + ", " + transform.position.y);
        player.Navigate(new List<Vector3> { transform.position + new Vector3(-2, 0) }, InteractTool);
    }
    private void InteractTool()
    {
        if (TutorialFlow.CurrentObjective.ObjectiveReferenceName != item.ToString() || player.InHand == item)
            return;

        Debug.Log("Grabbing Item...");

        PickupSound.Play();

        if (player.InHand != item)
            player.InHand = item;
        else
            player.InHand = TutorialPlayer.Items.Nothing;

        Arrow.SetActive(false);
        TutorialFlow.UpdateObjective();
    }

    public GameObject Arrow;
    void ObjectiveUpdated()
    {
        if (TutorialFlow.CurrentObjective.ObjectiveReferenceName == item.ToString())
            Arrow.SetActive(true);
    }

    void Update()
    {
        if (player.InHand == item)
            GetComponent<SpriteRenderer>().enabled = false;
        else
            GetComponent<SpriteRenderer>().enabled = true;
    }
}
