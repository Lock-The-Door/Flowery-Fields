using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Items
    {
        Nothing,
        Seeds,
        WateringCan
    }

    public Items InHand = Items.Nothing;

    // Update is called once per frame
    void Update()
    {
        
    }
}
