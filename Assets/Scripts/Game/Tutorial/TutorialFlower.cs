using System.Collections.Generic;
using UnityEngine;

public class TutorialFlower : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public Sprite DeadFlowers;
    public Sprite WeakFlowers;
    public Sprite NormalFlowers;
    public Sprite BeautifulFlowers;
    public Sprite SuperFlowers;

    Dictionary<TutorialFlowerBed.FlowerBedState, Sprite> FlowerStates => new Dictionary<TutorialFlowerBed.FlowerBedState, Sprite>
    {
        { TutorialFlowerBed.FlowerBedState.DeadFlowers, DeadFlowers },
        { TutorialFlowerBed.FlowerBedState.DrownedFlowers, DeadFlowers },
        { TutorialFlowerBed.FlowerBedState.WeakFlowers, WeakFlowers },
        { TutorialFlowerBed.FlowerBedState.NormalFlowers, NormalFlowers },
        { TutorialFlowerBed.FlowerBedState.BeautifulFlowers, BeautifulFlowers },
        { TutorialFlowerBed.FlowerBedState.SuperFlowers, SuperFlowers }
    };

    readonly Dictionary <TutorialFlowerBed.FlowerBedState, float> FlowerShadeOverride = new Dictionary<TutorialFlowerBed.FlowerBedState, float>
    {
        { TutorialFlowerBed.FlowerBedState.WeakFlowers, 0.65f }
    };

    readonly Dictionary<TutorialFlowerBed.FlowerBedState, float> FlowerSizesOverride = new Dictionary<TutorialFlowerBed.FlowerBedState, float>
    {
        { TutorialFlowerBed.FlowerBedState.DeadFlowers, 1.5f },
        { TutorialFlowerBed.FlowerBedState.WeakFlowers, 2f },
        { TutorialFlowerBed.FlowerBedState.BeautifulFlowers, 4f },
        { TutorialFlowerBed.FlowerBedState.SuperFlowers, 5f }
    };

    public Material NormalMaterial;
    public Material SuperFlowerMaterial;
    void Start() => UpdateFlower(GetComponentInParent<TutorialFlowerBed>().state);
    void UpdateFlower(TutorialFlowerBed.FlowerBedState state)
    {
        if (!FlowerStates.TryGetValue(state, out Sprite sprite))
        {
            SpriteRenderer.color = Color.clear;
            return;
        }

        transform.localScale = FlowerSizesOverride.TryGetValue(state, out float size) ? new Vector3(size, size) : new Vector3(2.5f, 2.5f);

        SpriteRenderer.sprite = sprite;
        SpriteRenderer.color = FlowerShadeOverride.TryGetValue(state, out float color) ? new Color(color, color, color) : Color.white;
        SpriteRenderer.material = state == TutorialFlowerBed.FlowerBedState.SuperFlowers ? SuperFlowerMaterial : NormalMaterial;
    }
}
