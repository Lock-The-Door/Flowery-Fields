using System.Linq;
using UnityEngine;

public class TutorialPathfindingManager : MonoBehaviour
{
    public static Pathfinding Pathfinding = new Pathfinding(new Grid<PathNode>(10, 10, 2, new Vector3(-9, -9), (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y)));

    public GameObject Table;
    public GameObject Flowerbeds;

    void Start()
    {
        // reposition objects based on screen ratio
        if (Screen.height / Screen.width > 0.75)
        {
            Table.transform.position += new Vector3(-2, 0);
        }

        // Generate anti-pathfind squares
        // Table
        Pathfinding.GetGrid().GetXY(Table.transform.position, out int tableX, out int tableY);
        for (int tableYPos = tableY - 1; tableYPos > tableY + 1; tableYPos++)
            Pathfinding.GetNode(tableX, tableYPos).SetIsWalkable(false);

        // Flowerbeds
        Flowerbeds.GetComponentsInChildren<TutorialFlowerBed>().ToList().ForEach(flowerBed =>
        {
            Pathfinding.GetGrid().GetXY(flowerBed.transform.position, out int flowerBedX, out int flowerBedY);
            Pathfinding.GetNode(flowerBedX, flowerBedY).SetIsWalkable(false);
        });
    }
}
