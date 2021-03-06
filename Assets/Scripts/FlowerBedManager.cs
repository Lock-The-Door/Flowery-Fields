using UnityEngine;

public class FlowerBedManager : MonoBehaviour
{
    public Player Player;
    public PopupManager PopupManager;
    public GameObject FlowerBed;

    Vector3[] FlowerBedLocations =
    {
        new Vector3(-4, -8),
        new Vector3(0, -8),
        new Vector3(4, -8),

        new Vector3(-4, 8),
        new Vector3(0, 8),
        new Vector3(4, 8),

        new Vector3(-8, 8),
        new Vector3(-8, 4),
        new Vector3(-8, 0),
        new Vector3(-8, -4),
        new Vector3(-8, -8)
    };

    void MakeFlowerBed(int level)
    {
        var NewFlowerBed = Instantiate(FlowerBed, transform);
        NewFlowerBed.transform.position = FlowerBedLocations[level];
        NewFlowerBed.GetComponent<FlowerBed>().player = Player;
        NewFlowerBed.GetComponent<FlowerBed>().PopupManager = PopupManager;
    }
}
