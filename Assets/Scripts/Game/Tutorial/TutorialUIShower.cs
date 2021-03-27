using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIShower : MonoBehaviour
{
    public TutorialPlayer Player;
    public TutorialFlow TutorialFlow;
    public GameObject FinishDay;
    public GameObject ShopButton;
    public GameObject BuyButton;
    public GameObject CloseButton;
    public TutorialShop Shop;
    public void CheckForObjectives()
    {
        string objectiveName = TutorialFlow.CurrentObjective.ObjectiveReferenceName;

        if (objectiveName == "Finish Day")
        {
            FinishDay.SetActive(true);
            return;
        }

        // SHOP
        if (objectiveName == "Shop")
        {
            // Perfect time to give the $1.2k
            Player.Money += 1200;

            // Show shop icon
            ShopButton.SetActive(true);
            ShopButton.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(ShopOpened()));
        }
    }

    IEnumerator ShopOpened()
    {
        // Remove access to shop
        ShopButton.SetActive(false);

        // Shop will automaticly open
        // Show the close button when three flower beds have been bought
        // remove the arrow and place it where the new close button is
        yield return new WaitUntil(() => Shop.ShopItems.Find(shopItem => shopItem.Name == "Flower Beds").Level == 3);
        BuyButton.transform.GetChild(1).gameObject.SetActive(false); // Hide arrow
        CloseButton.SetActive(true);
        CloseButton.GetComponent<Button>().onClick.AddListener(ShopClosed);
    }
    void ShopClosed()
    {
        TutorialFlow.UpdateObjective();
    }
}
