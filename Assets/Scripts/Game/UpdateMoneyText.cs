using TMPro;
using UnityEngine;

public class UpdateMoneyText : MonoBehaviour
{
    public Player Player;
    public TextMeshProUGUI TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = GetComponent<TextMeshProUGUI>();

        TextMeshPro.text = Player.Money.ToString();
        animateGoal = Player.Money;
    }

    int animateStart = 0;
    float time = 0;
    public float speed = 1;
    int animateGoal = 100;
    // Update is called once per frame
    void Update()
    {
        // Set Goal
        if (Player.Money != animateGoal)
        {
            if (Player.Money > System.Convert.ToInt32(TextMeshPro.text.Substring(1)))
            {
                animateGoal = Player.Money;
                time = 0;
                animateStart = System.Convert.ToInt32(TextMeshPro.text.Substring(1));
            }
            else if (Player.Money < System.Convert.ToInt32(TextMeshPro.text.Substring(1)))
            {
                time = 1;
                TextMeshPro.text = "$" + Player.Money.ToString();
                animateGoal = Player.Money;
            }
        }

        // Animate
        if (time < 1)
        {
            time += Time.deltaTime * speed;
            TextMeshPro.text = "$" + Mathf.RoundToInt(Mathf.Lerp(animateStart, animateGoal, time)).ToString();
        }
    }
}
