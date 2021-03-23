using System.Linq;
using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public AudioSource NormalAlertSound;
    public AudioSource BadAlertSound;

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
    
    public WindowPopup WindowPopup;
    public void ShowWindowPopup(string title, string description, bool goodAlert = true)
    {
        var popup = Instantiate(WindowPopup, transform);
        TextMeshProUGUI popupTitle = popup.TitleText;
        TextMeshProUGUI popupDetails = popup.DetailsText;

        popupTitle.text = title;
        popupDetails.text = description;

        // Play sounds
        if (goodAlert)
            NormalAlertSound.Play();
        else
            BadAlertSound.Play();
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

        // Play sounds
        if (goodAlert)
            NormalAlertSound.Play();
        else
            BadAlertSound.Play();
    }
}
