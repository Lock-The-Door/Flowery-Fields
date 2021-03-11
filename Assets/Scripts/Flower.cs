using System.Collections;
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

    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();

        var flowerPosition = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.25f, 0.25f));
        flowerPosition.z = flowerPosition.y - 0.26f;

        transform.localPosition = flowerPosition;
    }

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
