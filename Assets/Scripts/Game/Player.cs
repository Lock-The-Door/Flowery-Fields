using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Items
    {
        Nothing,
        Shovel,
        Seeds,
        WateringCan
    }

    public enum Gender
    {
        boy,
        girl
    }

    public Gender PlayerGender = Gender.girl;

    public int Money = 100;
    public Items InHand = Items.Nothing;

    public PathfindingManager PathfindingManager;
    public Shop Shop;

    public AudioSource SelectSound;

    Coroutine NavigationCoroutine;
    Vector3 nextNode;
    public float startWalkspeed;
    private float Walkspeed => startWalkspeed + .5f * Shop.ShopItems.Find(shopItem => shopItem.Name == "Shoes").Level;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Navigate(List<Vector3> targetLocs, System.Action callback = null, bool playSelectSound = true)
    {
        if (playSelectSound)
            SelectSound.Play();

        if (NavigationCoroutine != null)
            StopCoroutine(NavigationCoroutine); // Cancel navigation

        // Find closest end pos
        Vector3 targetLoc = Vector3.zero;
        float closestDistance = float.MaxValue;
        foreach (Vector3 possibleTargetLoc in targetLocs)
        {
            float distance = Vector3.Distance(transform.position, possibleTargetLoc);
            if (distance > closestDistance)
                continue;
            targetLoc = possibleTargetLoc;
            closestDistance = distance;
        }

        PathfindingManager.Pathfinding.GetGrid().GetXY(targetLoc, out int targetx, out int targety);
        Debug.Log(targetx + ", " + targety);

        Debug.Log(targetLoc);

        var path = PathfindingManager.Pathfinding.FindPath(transform.position, targetLoc);

        if (path == null || path.Count == 0)
        {
            Debug.LogError("Not moving");
            return;
        }

        // prevent diags
        if (path.Count > 1)
        {
            if ((Vector2)transform.position != (Vector2)path[0])
            {
                // compare distance from second node to the last next node and the current positional node
                // Current node pos
                PathfindingManager.Pathfinding.GetGrid().GetXY(transform.position, out int currentGridX, out int currentGridY);
                Vector3 currentWorldGridPos = new Vector3(currentGridX, currentGridY) * PathfindingManager.Pathfinding.GetGrid().GetCellSize() + PathfindingManager.Pathfinding.GetGrid().originPosition + Vector3.one * PathfindingManager.Pathfinding.GetGrid().GetCellSize() * .5f;

                Debug.Log("Currently at " + currentWorldGridPos);


                path[0] = Vector3.Distance(path[1], currentWorldGridPos) < Vector3.Distance(path[1], nextNode) ? currentWorldGridPos : nextNode;

                Debug.Log("Going to: " + path[0]);
            }
            else
            {
                // Remove start pos
                path.RemoveAt(0);
            }
        }

        Debug.Log("Following path:");
        string pathInString = "";
        foreach (Vector3 pathNode in path)
            pathInString += pathNode;
        Debug.Log(pathInString);

        NavigationCoroutine = StartCoroutine(NavigatePath(path, callback));
    }

    Animator Animator;
    SpriteRenderer SpriteRenderer;
    Vector3 start;
    IEnumerator NavigatePath(List<Vector3> path, System.Action callback)
    {
        Animator.SetBool("Walking", true);

        foreach (Vector3 pathNode in path)
        {
            nextNode = pathNode;

            if (start == default)
                start = transform.position;
            Vector3 currentPos = transform.position;
            Vector3 end = new Vector3(pathNode.x, pathNode.y, pathNode.y - 1);
            float time = path[0] == pathNode ? 1-(Vector2.Distance(currentPos, end)/2) : 0;

            Vector3 direction = end - currentPos;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            switch (angle - 90)
            {
                case 0: // Up
                    Animator.SetBool("Facing Away", true);
                    break;
                case -180: // Down
                    Animator.SetBool("Facing Away", false);
                    break;
                case 90: // Forward
                    Animator.SetBool("Facing Away", false);
                    SpriteRenderer.flipX = false;
                    break;
                case -90: // Back
                    Animator.SetBool("Facing Away", false);
                    SpriteRenderer.flipX = true;
                    break;
                default:
                    Debug.LogWarning("Player travelling at angle " + angle);
                    break;
            }

            while (time < 1)
            {
                time += Time.deltaTime * Walkspeed;
                transform.position = Vector3.Lerp(start, end, time);

                yield return new WaitForFixedUpdate();
            }

            start = end;
        }

        Animator.SetBool("Walking", false); // Stop animations

        if (callback != null)
        {
            // Finished, do callback
            callback.Invoke();
        }

        start = default;
    }
}
