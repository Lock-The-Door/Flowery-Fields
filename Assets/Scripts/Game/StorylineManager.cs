using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Storyline
{
    public string Name;
    public int DayOfTrigger;
    public StorylinePopup[] Popups;
    public System.Action Action = null;

    public void RunStoryline(PopupManager PopupManager)
    {
        if (!StorylineManager.StorylinesSeen.Contains(Name))
        {
            foreach (StorylinePopup popup in Popups)
                PopupManager.ShowWindowPopup(popup.title, popup.details);
        }

        if (Action != null)
            Action.Invoke();
    }
}
public class StorylinePopup
{
    public string title;
    public string details;

    public StorylinePopup(string _title, string _details)
    {
        title = _title;
        details = _details;
    }
}

public class StorylineManager : MonoBehaviour
{
    public Player Player;
    public PopupManager PopupManager;

    // Objects interacted with
    public GameObject BorrowMoneySign;

    public static List<string> StorylinesSeen = new List<string>();
    public List<Storyline> Storylines => new List<Storyline>()
    {
        // MAIN STORYLINE
        new Storyline() {
            Name = "The Adventure Begins",
            DayOfTrigger = 1,
            Popups = new StorylinePopup[]
            {
                new StorylinePopup("The adventure begins!",
                    $"You are a young {Player.PlayerGender} who's with a failing middle class family. " +
                    "As a last resort, your parents let you use your creativity to make some money. " +
                    "And as someone who loves flowers, you decided to start a flower farm to sell some flowers!"),
                new StorylinePopup("Supporting your family!",
                    "Knowing your family needs some help, you start by giving $10 to your family everyday.")
            }
        },
        new Storyline() {
            Name = "Borrowing Money",
            DayOfTrigger = 7,
            Popups = new StorylinePopup[]
            {
                new StorylinePopup("Borrowing money",
                    "Your family is nice enough to let you borrow some money from the bank to expand your business, but make sure you can return it!")
            },
            Action = new System.Action(() =>
            {
                BorrowMoneySign.SetActive(true); // Enable the borrow money sign
            })
        },
        new Storyline() {
            Name = "The End",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("You did it!",
                    "You've made a lot of money, you've upgraded your flower farm to the best form it can be. Your family is proud of you. The end! :)")
            }
        },
        // WEATHER
        new Storyline() {
            Name = "Sunny",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("It's a sunny day!",
                    "Let's plant some flowers! Click on the flower seeds on the table and then click on a flowerbed to plant some flowers. Don't forget to water them! When you are done, press finish day!")
            }
        },
        new Storyline() {
            Name = "Rainy",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("It's raining!",
                    "It's going to get wet! Remember to not water flowers today or your all flowers will drown!")
            }
        },
        new Storyline() {
            Name = "Superstorm",

            Popups = new StorylinePopup[] {
                new StorylinePopup("The superstorm!", 
                    "Superstorms are rare. Some flowers may not make it but many will still survive, stronger and better than ever!")  
            }
        },
        new Storyline() {
            Name = "Natural Disaster",
            
            Popups =  new StorylinePopup[]
            {
                new StorylinePopup("Uh oh! Incoming disaster!",
                    "Natural disasters are quite dangerous and you won't get much out of it unless you're lucky. You're better off leaving some flowers unharvested today and hope they don't die.")
            }
        },
        // FLOWER DISCOVERIES
        new Storyline() {
            Name = "Beautiful Flowers",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("Some nice flowers",
                    "Seems like you got some beautiful flowers! These might sell for more.")
            }
        },
        new Storyline() {
            Name = "Superflowers",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("Superflowers!!!",
                    "The rumours were true these ultimate beautiful flowers do exist. You'll definitely get a lot of money for that.")
            }
        },
        // INTERACTIONS
        new Storyline() {
            Name = "Being Generous",

            Popups = new StorylinePopup[]
            {
                new StorylinePopup("Being generous!", 
                    "As you make more money, you decide to become more generous and give more money to your family.")
            }
        },
    };

    public void ShowStoryline(string storylineName)
    {
        var storyline = Storylines.FirstOrDefault(storyline => storyline.Name == storylineName);

        if (StorylinesSeen.Contains(storylineName) || storyline == null)
            return;

        storyline.RunStoryline(PopupManager);

        StorylinesSeen.Add(storylineName);
    }

    public void CheckForNewStoryline(int day) => 
        Storylines.FindAll(storyline => storyline.DayOfTrigger == day).ForEach(storyline => ShowStoryline(storyline.Name)); // Call storyline actions for the day specified
}
