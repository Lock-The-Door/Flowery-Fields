using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BorrowedMoneyInfo
{
    public int DailyCost;
    public int DaysLeft;
    public int MoneyBorrowed;

    public BorrowedMoneyInfo(int _dailyCost, int _daysLeft, int _moneyBorrowed)
    {
        DailyCost = _dailyCost;
        DaysLeft = _daysLeft;
        MoneyBorrowed = _moneyBorrowed;
    }
}

public class BorrowMoney : MonoBehaviour
{
    public Slider moneySlider;
    public TMP_InputField moneyInput;
    public TMP_Dropdown moneyPlan;
    public TextMeshProUGUI borrowLimit;
    public TextMeshProUGUI toReturnText;
    public TextMeshProUGUI dailyPaymentText;
    public TextMeshProUGUI totalDailyPaymentText;

    public int maxMoneyAvalibleToBorrow = 1000;
    int moneyReadyToBorrow => maxMoneyAvalibleToBorrow - moneyBorrowed;
    int moneyBorrowed => dailyPayments.Select(payment => payment.MoneyBorrowed).Sum();

    int[] interestRates = { 3, 10, 15 }; // 3%, 10%, 15%

    List<BorrowedMoneyInfo> dailyPayments = new List<BorrowedMoneyInfo>();
    public int totalDailyPayment => dailyPayments.Select(payment => payment.DailyCost).Sum();
    public void UpdateDailyPayments()
    {
        List<BorrowedMoneyInfo> toRemove = new List<BorrowedMoneyInfo>();

        foreach(BorrowedMoneyInfo payment in dailyPayments)
            if (--payment.DaysLeft == 0)
                toRemove.Add(payment);
        dailyPayments.RemoveAll(payment => toRemove.Contains(payment));
    }

    // current values
    int moneyBorrowing = 0;
    int moneyToReturn => Mathf.RoundToInt(moneyBorrowing * ((float)interestRates[moneyPlan.value] / 100 + 1));
    int dailyPayment => moneyToReturn/((moneyPlan.value+1)*7);

    void Start() => Open();
    

    public void Open()
    {
        moneyBorrowing = 0;
        lastInput = "0";

        // Set default values
        borrowLimit.text = $"You can borrow up to ${moneyReadyToBorrow}";

        moneySlider.value = 0;
        moneySlider.maxValue = moneyReadyToBorrow; // limit slider

        moneyInput.text = "0";

        moneyPlan.value = 0;

        UpdatePrices();
    }

    void UpdatePrices()
    {
        toReturnText.text = $"To be returned ({interestRates[moneyPlan.value]}% interest): ${moneyToReturn}";
        dailyPaymentText.text = $"Daily payment: ${dailyPayment}";
        totalDailyPaymentText.text = $"Total daily payment: ${totalDailyPayment + dailyPayment}";
    }

    public void MoneySliderUpdate(float newValue)
    {
        moneyInput.text = newValue.ToString();
        lastInput = moneyInput.text;

        moneyBorrowing = (int)newValue;
        UpdatePrices();
    }
    string lastInput = "0";
    public void MoneyFieldInputUpdate(string newStringValue)
    {
        // Error correction
        if (!int.TryParse(newStringValue, out int newValue) || newValue < 0)
        {
            moneyInput.text = lastInput;
            return;
        }

        // See if in range
        if (newValue > moneyReadyToBorrow)
            newValue = moneyReadyToBorrow;


        moneyBorrowing = newValue;
        moneyInput.text = newValue.ToString();
        moneySlider.value = newValue;
        lastInput = newValue.ToString();
        UpdatePrices();
    }


    public Player Player;

    void Close() => gameObject.SetActive(false);
    void Confirm()
    {
        Player.money += moneyBorrowing;
        dailyPayments.Add(new BorrowedMoneyInfo(dailyPayment, (moneyPlan.value+1)*7, moneyBorrowing));

        Close();
    }
}
