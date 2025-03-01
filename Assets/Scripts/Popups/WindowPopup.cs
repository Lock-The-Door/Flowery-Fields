using TMPro;
using UnityEngine;

public class WindowPopup : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI DetailsText;

    RectTransform RectTransform;
    Vector2 startSize = new Vector2(0, 0);
    float time = 0;
    public float speed = 3; 
    Vector2 endSize;
    bool closing = false;
    public bool queuedPopup = true;

    public System.Action callbackAction;

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
            if (callbackAction != null)
                callbackAction.Invoke();

            Destroy(gameObject);
        }
    }

    void Close()
    {
        if (closing)
            return;

        Debug.Log("Closing window popup");

        startSize = endSize;
        endSize = new Vector2(0, 0);
        time = 0;
        closing = true;

        if (queuedPopup)
        {
            Debug.Log("Showing next popup");
            PopupManager.PopupQueue.Dequeue();
            PopupManager.NextPopup();
        }
    }
}
