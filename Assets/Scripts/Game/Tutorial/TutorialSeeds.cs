using System.Linq;
using UnityEngine;

public class TutorialSeeds : MonoBehaviour
{
    TutorialFlowerBed.FlowerBedState[] SeedsEnabledState =
    {
        TutorialFlowerBed.FlowerBedState.Planted,
        TutorialFlowerBed.FlowerBedState.Watered
    };

    public SpriteRenderer SpriteRenderer;
    void UpdateFlower(TutorialFlowerBed.FlowerBedState state)
    {
        if (SeedsEnabledState.Contains(state))
            SpriteRenderer.color = Color.white;
        else
            SpriteRenderer.color = Color.clear;
    }
}
