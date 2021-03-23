using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public AudioSource _NormalAlertSound;
    static AudioSource NormalAlertSound;
    public AudioSource _BadAlertSound;
    static AudioSource BadAlertSound;

    private void Start()
    {
        NormalAlertSound = _NormalAlertSound;
        BadAlertSound = _BadAlertSound;
        Debug.Log("Set Popup Sounds");
    }

    public BottomPopupText BottomPopupTextPrefab;
    public void ShowBottomPopup(string message, Color color, bool goodAlert = true)
    {
        var popup = Instantiate(BottomPopupTextPrefab, transform);
        TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();

        popupText.text = message;
        popupText.color = color;

        // Play sounds
        if (goodAlert)
            NormalAlertSound.Play();
        else
            BadAlertSound.Play();
    }

    public static Queue<KeyValuePair<GameObject, bool>> PopupQueue = new Queue<KeyValuePair<GameObject, bool>>();
    public static void NextPopup()
    {
        if (PopupQueue.Count == 0)
            return;

        KeyValuePair<GameObject, bool> queuedPopup = PopupQueue.Peek();
        GameObject popup = queuedPopup.Key;

        popup.SetActive(true); // Show popup

        // Play sounds
        if (queuedPopup.Value)
            NormalAlertSound.Play();
        else
            BadAlertSound.Play();
    }

    public void QueuePopup(KeyValuePair<GameObject, bool> popup)
    {
        PopupQueue.Enqueue(popup);

        if (PopupQueue.Count == 1)
            NextPopup();
    }

    public WindowPopup WindowPopup;
    public void ShowWindowPopup(string title, string description, System.Action callback = null, bool goodAlert = true)
    {
        var popup = Instantiate(WindowPopup, transform);
        TextMeshProUGUI popupTitle = popup.TitleText;
        TextMeshProUGUI popupDetails = popup.DetailsText;

        popupTitle.text = title;
        popupDetails.text = description;
        popup.callbackAction = callback;

        QueuePopup(new KeyValuePair<GameObject, bool>(popup.gameObject, goodAlert));
    }

    public DecisionWindowPopup DecisionWindowPopup;
    public void ShowDecisionWindowPopup(string title, string description, System.Action<bool> callback, bool goodAlert = true)
    {
        var popup = Instantiate(DecisionWindowPopup, transform);
        TextMeshProUGUI popupTitle = popup.TitleText;
        TextMeshProUGUI popupDetails = popup.DetailsText;

        popupTitle.text = title;
        popupDetails.text = description;
        popup.callbackAction = callback;

        QueuePopup(new KeyValuePair<GameObject, bool>(popup.gameObject, goodAlert));
    }
}
