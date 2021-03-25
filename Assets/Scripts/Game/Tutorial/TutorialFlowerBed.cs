using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class TutorialFlowerBed : MonoBehaviour, IPointerClickHandler
{
    public enum FlowerBedState
    {
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

    public TutorialPlayer player;
    public int id = -1;
    public FlowerBedState state = FlowerBedState.Empty;
    private readonly Dictionary<FlowerBedState, int> FlowerSellPrice = new Dictionary<FlowerBedState, int> { 
        {FlowerBedState.DeadFlowers, 0},
        {FlowerBedState.DrownedFlowers, 0},
        {FlowerBedState.WeakFlowers, 5},
        {FlowerBedState.NormalFlowers, 15},
        {FlowerBedState.BeautifulFlowers, 50},
        {FlowerBedState.SuperFlowers, 100},
    };


    void Start()
    {
        GenerateFlowers();
    }

    readonly int flowers = 10;
    public List<TutorialFlower> flowerTypes;
    void GenerateFlowers()
    {
        transform.GetComponentsInChildren<TutorialFlower>().ToList().ForEach(flower => Destroy(flower.gameObject)); // Remove existing flowers

        for (int i = 0; i < flowers; i++) // create new flowers
        {
            var flower = Instantiate(flowerTypes[Random.Range(0, flowerTypes.Count)], transform);

            var flowerPosition = new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.70f));
            flowerPosition.z = flowerPosition.y - 0.75f;

            flower.transform.localPosition = flowerPosition;
        }
    }

    public TutorialShop Shop;
    private const int SeedsPrice = 7;
    private const int WaterPrice = 3;
    public PopupManager PopupManager;
    public CenterFarm CenterFarm;

    public AudioSource PlantingSound;
    public AudioSource WateringSound;
    public AudioSource DiggingSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Flower Bed Clicked");

        Debug.Log("Navigating to... " + transform.position.x + ", " + transform.position.y);
        List<Vector3> flowerBedEndPositions = new List<Vector3>();
        if ((CenterFarm.topUnlocked ? 8 : 4) > (transform.position + new Vector3(0, 2)).y)
        {
            flowerBedEndPositions.Add(transform.position + new Vector3(2, 0)); // right
            flowerBedEndPositions.Add(transform.position + new Vector3(0, 2)); // up
            flowerBedEndPositions.Add(transform.position + new Vector3(-2, 0)); // left
        }
        if ((CenterFarm.bottomUnlocked ? -8 : -4) < (transform.position + new Vector3(0, -2)).y)
            flowerBedEndPositions.Add(transform.position + new Vector3(0, -2)); // down
        player.Navigate(flowerBedEndPositions, DoAction);
    }
    private void DoAction()
    {
        if (TutorialFlow.CurrentObjective.ObjectiveReferenceName == "Conditional Harvest" && state != FlowerBedState.SuperFlowers)
            return;

        Debug.Log("Doing action...");

        switch (player.InHand)
        {
            case TutorialPlayer.Items.Nothing:
                return; // Don't run code to assume objective is complete
            case TutorialPlayer.Items.Shovel:

                if (!FlowerSellPrice.TryGetValue(state, out int sellPrice))
                    break;

                DiggingSound.Play();

                UpdateFlowerbedState(FlowerBedState.Empty);
                player.Money += sellPrice;
                break;
            case TutorialPlayer.Items.Seeds:
                if (state == FlowerBedState.Empty)
                {
                    if (player.Money < SeedsPrice)
                    {
                        Debug.Log("Not enough money...");
                        PopupManager.ShowBottomPopup("Not enough money...", Color.red, goodAlert: false);
                        return;
                    }

                    PlantingSound.Play();

                    UpdateFlowerbedState(FlowerBedState.Planted);
                    player.Money -= SeedsPrice;
                }
                break;
            case TutorialPlayer.Items.WateringCan:
                if (state == FlowerBedState.Planted)
                {
                    if (player.Money < WaterPrice)
                    {
                        Debug.Log("Not enough money...");
                        PopupManager.ShowBottomPopup("Not enough money...", Color.red, goodAlert: false);
                        return;
                    }

                    WateringSound.Play();

                    UpdateFlowerbedState(FlowerBedState.Watered);
                    player.Money -= WaterPrice;
                }
                break;
        }

        // The user is forced to use a certain tool so we can safely assume the folder bed interaction caused an objection update

        Arrow.SetActive(false); // Remove arrow

        // Check if objective is complete
        SendMessageUpwards("CheckFlowerBedStates");
    }

    public GameObject Arrow;
    void ShowArrow() =>
        Arrow.SetActive(true);

    public Sprite EmptyTexture;
    public Sprite WateredTexture;
    public Sprite DrownedTexture;
    public Material NormalMaterial;
    public Material SuperFlowerMaterial;
    public void UpdateFlowerbedState(FlowerBedState flowerBedState)
    {
        state = flowerBedState;

        if (flowerBedState == FlowerBedState.Empty)
            GenerateFlowers();

        BroadcastMessage("UpdateFlower", flowerBedState);

        SpriteRenderer SpriteRenderer = GetComponent<SpriteRenderer>();
        switch (state)
        {
            // Use empty texture
            case FlowerBedState.Empty:
            case FlowerBedState.Planted:
            case FlowerBedState.DeadFlowers:
                SpriteRenderer.sprite = EmptyTexture;
                break;

            // Use watered texture
            case FlowerBedState.WeakFlowers:
            case FlowerBedState.NormalFlowers:
            case FlowerBedState.BeautifulFlowers:
            case FlowerBedState.SuperFlowers:
            case FlowerBedState.Watered:
                SpriteRenderer.sprite = WateredTexture;
                break;

            // Use drowned texture
            case FlowerBedState.DrownedFlowers:
                SpriteRenderer.sprite = DrownedTexture;
                break;
        }
        if (state == FlowerBedState.SuperFlowers)
        {
            GetComponent<Volume>().enabled = true;

            SpriteRenderer.material = SuperFlowerMaterial;
        }
        else
        {
            GetComponent<Volume>().enabled = false;

            SpriteRenderer.material = NormalMaterial;
        }
    }
}
