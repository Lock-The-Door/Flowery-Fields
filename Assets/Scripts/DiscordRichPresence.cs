using UnityEngine;
using Discord;
using System.Collections.Generic;

public static class DiscordRichPresenceManager
{
    private static Discord.Discord discord;
    private static Discord.Discord GetOrCreateDiscord()
    {
        if (discord == null)
        {
            try
            {
                discord = new Discord.Discord(836697022471471154, (ulong)CreateFlags.NoRequireDiscord);
            }
            catch (ResultException e)
            {
                if (e.Result == Result.NotRunning)
                {
                    return null;
                }
            }
        }

        return discord;
    }

    public static void RunCallbacks()
    {
        try
        {
            GetOrCreateDiscord().RunCallbacks();
        }
        catch
        {
            discord = null;
            return;
        }
    }

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
        if (GetOrCreateDiscord() == null)
        {
            Debug.Log("Discord unavailable");
            return;
        }

        Debug.Log("Updating discord rich presence for " + activityName);

        ActivityManager activityManager = GetOrCreateDiscord().GetActivityManager();

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