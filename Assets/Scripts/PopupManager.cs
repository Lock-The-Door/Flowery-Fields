using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public BottomPopupText BottomPopupTextPrefab;

    public void ShowBottomPopup(string message, Color color)
    {
        var popup = Instantiate(BottomPopupTextPrefab, transform);
        TextMeshProUGUI popupText = popup.GetComponent<TextMeshProUGUI>();

        popupText.text = message;
        popupText.color = color;
    }
    
    public WindowPopup WindowPopup;

    public void ShowWindowPopup(string title, string description)
    {
        var popup = Instantiate(WindowPopup, transform);
        TextMeshProUGUI popupTitle = popup.TitleText;
        TextMeshProUGUI popupDetails = popup.DetailsText;

        popupTitle.text = title;
        popupDetails.text = description;
    }
}
