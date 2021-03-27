using UnityEngine;

public class BottomPopupText : MonoBehaviour
{
    Vector2 startPos;
    float time = 0;
    public float speed = 1;
    public float endY = 20;
    Vector2 endPos;

    public float stationaryTime = 3;

    bool reversing = false;

    private void Start()
    {
        startPos = GetComponent<RectTransform>().anchoredPosition;
        endPos = new Vector2(startPos.x, endY);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;

        if (time < 1)
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPos, endPos, time);
        else if (time >= stationaryTime + 1 && !reversing)
        {
            endPos = startPos;
            startPos = transform.position;
            time = 0;
            reversing = true;
        }
        else if (time >= 1 && reversing)
        {
            Destroy(gameObject);
        }
    }
}
