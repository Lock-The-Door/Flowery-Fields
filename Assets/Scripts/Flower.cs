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
        { FlowerBed.FlowerBedState.DrownedFlowers, DeadFlowers },
        { FlowerBed.FlowerBedState.WeakFlowers, WeakFlowers },
        { FlowerBed.FlowerBedState.NormalFlowers, NormalFlowers },
        { FlowerBed.FlowerBedState.BeautifulFlowers, BeautifulFlowers },
        { FlowerBed.FlowerBedState.SuperFlowers, SuperFlowers }
    };

    Dictionary<FlowerBed.FlowerBedState, float> FlowerSizesOverride => new Dictionary<FlowerBed.FlowerBedState, float>
    {
        { FlowerBed.FlowerBedState.BeautifulFlowers, 3f },
        { FlowerBed.FlowerBedState.SuperFlowers, 4f }
    };

    void Start() => UpdateFlower(FlowerBed.FlowerBedState.Empty);
    void UpdateFlower(FlowerBed.FlowerBedState state)
    {
        if (!FlowerStates.TryGetValue(state, out Sprite sprite))
        {
            SpriteRenderer.color = Color.clear;
            return;
        }

        transform.localScale = FlowerSizesOverride.TryGetValue(state, out float size) ? new Vector3(size, size) : new Vector3(2.5f, 2.5f);

        SpriteRenderer.sprite = sprite;

        SpriteRenderer.color = Color.white;
    }
}
