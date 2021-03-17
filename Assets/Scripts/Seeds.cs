using System.Linq;
using UnityEngine;

public class Seeds : MonoBehaviour
{
    FlowerBed.FlowerBedState[] SeedsEnabledState =
    {
        FlowerBed.FlowerBedState.Planted,
        FlowerBed.FlowerBedState.Watered
    };

    public SpriteRenderer SpriteRenderer;
    void UpdateFlower(FlowerBed.FlowerBedState state)
    {
        if (SeedsEnabledState.Contains(state))
            SpriteRenderer.color = Color.white;
        else
            SpriteRenderer.color = Color.clear;
    }
}
