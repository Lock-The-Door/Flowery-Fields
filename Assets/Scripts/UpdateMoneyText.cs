using System.Collections;
using System.Collections.Generic;
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

        TextMeshPro.text = Player.money.ToString();
        animateGoal = Player.money;
    }

    int animateStart = 0;
    float time = 0;
    public float speed = 1;
    int animateGoal = 100;
    // Update is called once per frame
    void Update()
    {
        // Set Goal
        if (Player.money != animateGoal)
        {
            if (Player.money > System.Convert.ToInt32(TextMeshPro.text))
            {
                animateGoal = Player.money;
                time = 0;
                animateStart = System.Convert.ToInt32(TextMeshPro.text);
            }
            else if (Player.money < System.Convert.ToInt32(TextMeshPro.text))
            {
                time = 1;
                TextMeshPro.text = Player.money.ToString();
                animateGoal = Player.money;
            }
        }

        // Animate
        if (time < 1)
        {
            time += Time.deltaTime * speed;
            TextMeshPro.text = Mathf.RoundToInt(Mathf.Lerp(animateStart, animateGoal, time)).ToString();
        }
    }
}
