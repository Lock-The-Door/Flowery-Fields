using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Sprite DeadFlowers;
    public Sprite WeakFlowers;
    public Sprite NormalFlowers;
    public Sprite BeautifulFlowers;
    public Sprite SuperFlowers;

    Dictionary<FlowerBed.FlowerBedState, Sprite> FlowerStates => new Dictionary<FlowerBed.FlowerBedState, Sprite>
    {
        { FlowerBed.FlowerBedState.DeadFlowers, DeadFlowers },
        { FlowerBed.FlowerBedState.WeakFlowers, WeakFlowers },
        { FlowerBed.FlowerBedState.NormalFlowers, NormalFlowers },
        { FlowerBed.FlowerBedState.BeautifulFlowers, BeautifulFlowers },
        { FlowerBed.FlowerBedState.SuperFlowers, SuperFlowers },
    };

    void UpdateFlower(FlowerBed.FlowerBedState state)
    {
        if (!FlowerStates.TryGetValue(state, out Sprite sprite))
        {
            SpriteRenderer.color = Color.clear;
            return;
        }

        SpriteRenderer.sprite = sprite;

        SpriteRenderer.color = Color.white;
    }
}
