using System.Collections.Generic;
using UnityEngine;

public class Storyline
{
    public string title;
    public string details;

    public Storyline(string _title, string _details)
    {
        title = _title;
        details = _details;
    }
}

public class StorylineManager : MonoBehaviour
{
    public Player Player;
    public PopupManager PopupManager;

    List<string> storylinesSeen = new List<string>();
    Dictionary<string, Storyline[]> storylines => new Dictionary<string, Storyline[]>
    {
        {
            "The Adventure Begins",

            new Storyline[]
            {
                new Storyline("Supporting your family!",
                    "Knowing your family needs some help, you'll start by giving $20 to your family everyday."),
                new Storyline("The adventure begins!",
                    $"You are a young {Player.PlayerGender} who's with a failing middle class family. " +
                    "As a last resort, your parents let you use your creativity to make some money. " +
                    "And as someone who loves flowers, you decided to start a flower farm to sell some flowers!")
            }
        },
        {
            "Being Generous",

            new Storyline[]
            {
                new Storyline("Being generous!", 
                    "As you make more money, you decide to become more generous give more money to your family.")
            }
        },
        {
            "Borrowing Money",

            new Storyline[]
            {
                new Storyline("Borrowing money", 
                    "Your family is nice enough to let you borrow some money from the bank to expand your business, but make sure you can return it!")
            }
        },
        {
            "The End",

            new Storyline[]
            {
                new Storyline("You did it!", 
                    "You've made a lot of money, your family is proud of you. The end! :)")
            }
        },
    };

    public void ShowStoryline(string storylineName)
    {
        if (storylinesSeen.Contains(storylineName) || !storylines.TryGetValue(storylineName, out var storyline))
            return;

        storylinesSeen.Add(storylineName);

        foreach (Storyline storylineLine in storyline)
            PopupManager.ShowWindowPopup(storylineLine.title, storylineLine.details);
    }
}
