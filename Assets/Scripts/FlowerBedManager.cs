using System.Linq;
using UnityEngine;

public class FlowerBedManager : MonoBehaviour
{
    public Player Player;
    public Shop Shop;
    public PopupManager PopupManager;
    public CenterFarm CenterFarm;
    public PathfindingManager PathfindingManager;
    public GameObject FlowerBed;

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
        var NewFlowerBed = Instantiate(FlowerBed, transform);
        NewFlowerBed.transform.position = FlowerBedLocations[level];
        NewFlowerBed.GetComponent<FlowerBed>().player = Player;
        NewFlowerBed.GetComponent<FlowerBed>().PopupManager = PopupManager;
        NewFlowerBed.GetComponent<FlowerBed>().CenterFarm = CenterFarm;
        NewFlowerBed.GetComponent<FlowerBed>().Shop = Shop;
        NewFlowerBed.GetComponent<FlowerBed>().id = level;

        // Adjust pathfinding
        PathfindingManager.Pathfinding.GetGrid().GetXY(NewFlowerBed.transform.position, out int flowerBedX, out int flowerBedY);
        PathfindingManager.Pathfinding.GetNode(flowerBedX, flowerBedY).SetIsWalkable(false);
    }

    void RemoveFlowerBed(int level) => Destroy(GetComponentsInChildren<FlowerBed>().First(flowerBed => flowerBed.id == level).gameObject);
}
