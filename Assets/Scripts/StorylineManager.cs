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

    public List<string> StorylinesSeen = new List<string>();
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
            "Sunny",

            new Storyline[]
            {
                new Storyline("It's a sunny day!",
                    "Let's plant some flowers! Click on the flower seeds on the table and then click on a flowerbed to plant some flowers. Don't forget to water them! When you are done, press finish day!")
            }
        },
        {
            "Rainy",

            new Storyline[]
            {
                new Storyline("It's raining!",
                    "It's going to get wet! Remember to not water flowers today or your all flowers will drown!")
            }
        },
        {
            "Superstorm",

            new Storyline[]
            {
                new Storyline("The superstorm!",
                    "Superstorms are rare. Some flowers may not make it but many will still survive, stronger and better than ever!")
            }
        },
        {
            "Natural Disaster",
            
            new Storyline[]
            {
                new Storyline("Uh oh! Incoming disaster!",
                    "Natural disasters are quite dangerous and you won't get much out of it unless you're lucky. You're better off leaving some flowers unharvested today and hope they don't die.")
            }
        },
        {
            "Beautiful flowers",

            new Storyline[]
            {
                new Storyline("Some nice flowers",
                    "Seems like you got some beautiful flowers! These might sell for more.")
            }
        },
        {
            "Superflowers",

            new Storyline[]
            {
                new Storyline("Superflowers!!!",
                    "The rumours were true these ultimate beautiful flowers do exist. You'll definitely get a lot of money for that.")
            }
        },
        {
            "Being Generous",

            new Storyline[]
            {
                new Storyline("Being generous!", 
                    "As you make more money, you decide to become more generous and give more money to your family.")
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
        if (StorylinesSeen.Contains(storylineName) || !storylines.TryGetValue(storylineName, out var storyline))
            return;

        StorylinesSeen.Add(storylineName);

        foreach (Storyline storylineLine in storyline)
            PopupManager.ShowWindowPopup(storylineLine.title, storylineLine.details);
    }
}
