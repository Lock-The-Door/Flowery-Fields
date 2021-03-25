using System.Linq;
using UnityEngine;

public class TutorialFlowerBedManager : MonoBehaviour
{
    public TutorialFlow TutorialFlow;
    public TutorialPlayer Player;
    public TutorialShop Shop;
    public PopupManager PopupManager;
    public CenterFarm CenterFarm;
    public GameObject TutorialFlowerBed;

    readonly Vector3[] FlowerBedLocations =
    {
        new Vector3(-4, -8, -8),
        new Vector3(0, -8, -8),
        new Vector3(4, -8),

        new Vector3(-4, 8, 8),
        new Vector3(0, 8, 8),
        new Vector3(4, 8, 8),

        new Vector3(-8, 8, 8),
        new Vector3(-8, 4, 4),
        new Vector3(-8, 0, 0),
        new Vector3(-8, -4, -4),
        new Vector3(-8, -8, -8)
    };

    void MakeFlowerBed(int level)
    {
        // Create Flowerbed
        var NewFlowerBed = Instantiate(TutorialFlowerBed, transform);
        NewFlowerBed.transform.position = FlowerBedLocations[level];
        NewFlowerBed.GetComponent<TutorialFlowerBed>().player = Player;
        NewFlowerBed.GetComponent<TutorialFlowerBed>().PopupManager = PopupManager;
        NewFlowerBed.GetComponent<TutorialFlowerBed>().CenterFarm = CenterFarm;
        NewFlowerBed.GetComponent<TutorialFlowerBed>().Shop = Shop;
        NewFlowerBed.GetComponent<TutorialFlowerBed>().id = level;

        // Adjust pathfinding
        TutorialPathfindingManager.Pathfinding.GetGrid().GetXY(NewFlowerBed.transform.position, out int flowerBedX, out int flowerBedY);
        TutorialPathfindingManager.Pathfinding.GetNode(flowerBedX, flowerBedY).SetIsWalkable(false);
    }

    void RemoveFlowerBed(int level) => Destroy(GetComponentsInChildren<TutorialFlowerBed>().First(flowerBed => flowerBed.id == level).gameObject);

    void ObjectiveCheck()
    {
        string objective = TutorialFlow.CurrentObjective.ObjectiveReferenceName;

        if (objective == "Conditional Harvest") // Can't blindly show arrows
            GetComponentsInChildren<TutorialFlowerBed>().Where(flowerBed => flowerBed.state == global::TutorialFlowerBed.FlowerBedState.SuperFlowers).ToList().ForEach(flowerBed => flowerBed.SendMessage("ShowArrow"));
        else if (System.Enum.TryParse<TutorialFlowerBed.FlowerBedState>(objective, out var enumObjective)) // all flowerbeds need to match new condition since this ensures it is a flowerbed objective
            GetComponentsInChildren<TutorialFlowerBed>().Where(flowerBed => flowerBed.state != enumObjective).ToList().ForEach(flowerBed => flowerBed.SendMessage("ShowArrow"));
    }
    void CheckFlowerBedStates()
    {
        string objectiveName = TutorialFlow.CurrentObjective.ObjectiveReferenceName;

        if ((objectiveName == "Conditional Harvest" && // Conditional harvest (only super flowers or empty)
                GetComponentsInChildren<TutorialFlowerBed>().All(flowerBed => flowerBed.state == global::TutorialFlowerBed.FlowerBedState.WeakFlowers || flowerBed.state == global::TutorialFlowerBed.FlowerBedState.Empty)) ||
            GetComponentsInChildren<TutorialFlowerBed>().All(flowerBed => flowerBed.state.ToString() == objectiveName)) // Normal harvest
        {
            TutorialFlow.UpdateObjective();
        }
    }
}
