using UnityEngine;
using Discord;
using System.Collections.Generic;

public static class DiscordRichPresenceManager
{
    private static Discord.Discord discord = new Discord.Discord(836697022471471154, (ulong)CreateFlags.NoRequireDiscord);

    public static void RunCallbacks() => discord.RunCallbacks();

    private static readonly ActivityManager activityManager = discord.GetActivityManager();
    private static readonly Dictionary<string, Activity> activities = new Dictionary<string, Activity>()
    {
        {"Main Menu", new Activity()
            {
                Details = "Main Menu",
                Assets =
                {
                    LargeImage = "icon"
                },
                Instance = true
            }
        },
        {"Playing", new Activity()
            {
                Details = "Growing Flowers",
                Assets =
                {
                    LargeImage = "icon"
                },
                Instance = true
            }
        },
        {"Tutorial", new Activity()
            {
                Details = "Learning About Flowers",
                Assets =
                {
                    LargeImage = "icon"
                },
                Instance = true
            }
        }
    };

    private static string LastActivity = null;
    private static long LastTimestamp = 0;
    public static void UpdateActivity(string activityName, int day = 0)
    {
        Debug.Log("Updating discord rich presence for " + activityName);

        var activity = activities[activityName];

        // Don't override timestamp if only updating day #
        long timestamp = System.DateTimeOffset.Now.ToUnixTimeSeconds();
        activity.Timestamps.Start = LastActivity != activityName ? timestamp : LastTimestamp;

        if (day > 0)
            activity.State = "Day " + day;

        if (Application.isEditor)
        {
            Debug.Log("Not updating in editor");
            return;
        }

        activityManager.UpdateActivity(activity, AfterUpdate);
        LastActivity = activityName;
        LastTimestamp = timestamp;
    }

    static void AfterUpdate(Result result)
    {
        if (result == Result.Ok)
        {
            Debug.Log("Sucessfully updated discord rich presence");
        }
        else
        {
            Debug.LogWarning("Failed to update discord rich presence. Failed with error: " + result);
        }
    }
}