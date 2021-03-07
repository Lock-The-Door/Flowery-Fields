using UnityEngine;
using UnityEngine.EventSystems;

public class OpenBorrowMoney : MonoBehaviour, IPointerClickHandler
{
    public PopupManager PopupManager;
    public GameObject BorrowMoneyUi;
    bool borrowMoney_Storyline;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!borrowMoney_Storyline) // Storyline
        {
            PopupManager.ShowWindowPopup("Borrowing money", "Your family is nice enough to let you borrow some money from the bank to expand your business, but make sure you can return it!");
            borrowMoney_Storyline = true;
        }

        BorrowMoneyUi.GetComponent<BorrowMoney>().Open();
        BorrowMoneyUi.SetActive(true);
    }
}
