using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : MonoBehaviour
{
    public enum Items
    {
        Nothing,
        Seeds,
        WateringCan
    }

    public int money = 100;
    public Items InHand = Items.Nothing;

    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Navigate(object[] arguments)
    {
        Vector3 targetLoc = (Vector3)arguments[0];
        GameObject callback = (GameObject)arguments[1];

        agent.ResetPath();
        agent.SetDestination(targetLoc);

        if (Vector3.Distance(transform.position, targetLoc) > agent.stoppingDistance)
            StartCoroutine("ArrivalDetection", arguments);
        else
            Debug.Log("Arrived!");
    }

    IEnumerable ArrivalDetection(object[] arguments)
    {
        Vector3 targetLoc = (Vector3)arguments[0];
        GameObject callback = (GameObject)arguments[1];

        yield return new WaitUntil(() => Vector3.Distance(transform.position, targetLoc) <= agent.stoppingDistance);

        Debug.Log("Arrived!");
    }
}
