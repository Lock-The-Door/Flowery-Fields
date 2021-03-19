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

    Dictionary<FlowerBed.FlowerBedState, float> FlowerShadeOverride = new Dictionary<FlowerBed.FlowerBedState, float>
    {
        { FlowerBed.FlowerBedState.WeakFlowers, 0.65f }
    };

    Dictionary<FlowerBed.FlowerBedState, float> FlowerSizesOverride = new Dictionary<FlowerBed.FlowerBedState, float>
    {
        { FlowerBed.FlowerBedState.DeadFlowers, 1.5f },
        { FlowerBed.FlowerBedState.WeakFlowers, 2f },
        { FlowerBed.FlowerBedState.BeautifulFlowers, 4f },
        { FlowerBed.FlowerBedState.SuperFlowers, 5f }
    };

    public Material NormalMaterial;
    public Material SuperFlowerMaterial;
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
        SpriteRenderer.color = FlowerShadeOverride.TryGetValue(state, out float color) ? new Color(color, color, color) : Color.white;
        SpriteRenderer.material = state == FlowerBed.FlowerBedState.SuperFlowers ? SuperFlowerMaterial : NormalMaterial;
    }
}
