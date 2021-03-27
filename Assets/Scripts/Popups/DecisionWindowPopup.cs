using TMPro;
using UnityEngine;

public class DecisionWindowPopup : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DetailsText;

    RectTransform RectTransform;
    Vector2 startSize = new Vector2(0, 0);
    float time = 0;
    public float speed = 3; 
    Vector2 endSize;
    bool closing = false;

    public System.Action<bool> callbackAction;
    bool answer;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        endSize = RectTransform.localScale;
        RectTransform.localScale = startSize;
        Debug.Log("Ready to open");
    }

    // Update is called once per frame
    void Update()
    {
        if (time < 1)
        {
            time += Time.deltaTime;
            RectTransform.localScale = Vector2.Lerp(startSize, endSize, time);
        }
        else if (closing)
        {
            callbackAction.Invoke(answer);

            Destroy(gameObject);
        }
    }

    public void Close(bool _answer)
    {
        if (closing)
            return;

        Debug.Log("Closing window popup");
        answer = _answer;

        startSize = endSize;
        endSize = new Vector2(0, 0);
        time = 0;
        closing = true;

        Debug.Log("Showing next popup");
        PopupManager.PopupQueue.Dequeue();
        PopupManager.NextPopup();
    }
}
