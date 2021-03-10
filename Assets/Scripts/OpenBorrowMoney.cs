using UnityEngine;
using UnityEngine.EventSystems;

public class OpenBorrowMoney : MonoBehaviour, IPointerClickHandler
{
    public StorylineManager StorylineManager;
    public PopupManager PopupManager;
    public GameObject BorrowMoneyUi;

    public void OnPointerClick(PointerEventData eventData)
    {
        StorylineManager.ShowStoryline("Borrowing Money");

        BorrowMoneyUi.GetComponent<BorrowMoney>().Open();
        BorrowMoneyUi.SetActive(true);
    }
}
