using UnityEngine;

public class CenterFarm : MonoBehaviour
{
    public bool topUnlocked = false;
    public bool bottomUnlocked = false;
    public bool leftSideUnlocked = false;

    public const float FullOrthographicSize = 10;
    public const float HalfOrthographicSize = 9;
    public const float SmallestOrthographicSize = 8;

    public const float LockedModeOffset = 2;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(LockedModeOffset, 0, transform.position.z);
        GetComponent<Camera>().orthographicSize = SmallestOrthographicSize;

        if (topUnlocked && bottomUnlocked)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        else if (topUnlocked)
            transform.position = new Vector3(transform.position.x, LockedModeOffset, transform.position.z);
        else if (bottomUnlocked)
            transform.position = new Vector3(transform.position.x, -LockedModeOffset, transform.position.z);

        if (leftSideUnlocked)
        {
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            if (Screen.height/Screen.width > 0.75)
                GetComponent<Camera>().orthographicSize = FullOrthographicSize;
        }
        if (topUnlocked && bottomUnlocked)
            GetComponent<Camera>().orthographicSize = FullOrthographicSize;
    }
}
