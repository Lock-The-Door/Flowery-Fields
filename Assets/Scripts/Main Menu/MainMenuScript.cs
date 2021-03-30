using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour
{
    public PopupManager PopupManager;
    public AudioMixer AudioMixer;

    // Start is called before the first frame update
    void Start()
    {
        // Reset game statics
        GameStatics.NewGame = true;
        GameStatics.LoadedGame = new GameData();
        GameStatics.GameGuid = Guid.Empty.ToString();

        // Load & Apply Settings
        // Audio
        float MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(MasterVolume) * 20);
        float MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.35f);
        AudioMixer.SetFloat("MusicVolume", Mathf.Log10(MusicVolume) * 20);
        float SoundEffectVolume = PlayerPrefs.GetFloat("SoundEffectVolume", 0.5f);
        AudioMixer.SetFloat("SoundEffectVolume", Mathf.Log10(SoundEffectVolume) * 20);

        GameUpdates.CheckForUpdates(PopupManager); // Check for game updates
    }
}

public static class GameUpdates
{
    static bool updateReminderTriggered = false;
    static Dictionary<RuntimePlatform, string> PlatformUpdateChannelName = new Dictionary<RuntimePlatform, string>()
    {
        { RuntimePlatform.WindowsPlayer, "windows" },
        { RuntimePlatform.OSXPlayer, "mac" },
        { RuntimePlatform.LinuxPlayer, "linux-x86_64" }
    };

    public static async Task CheckForUpdates(PopupManager popupManager)
    {
        // Check for game updates
        if (!updateReminderTriggered)
        {
            // Get channel name
            if (!PlatformUpdateChannelName.TryGetValue(Application.platform, out string channelName))
            {
                Debug.Log("Platform is not eligible for update detection");
                updateReminderTriggered = true;
                return;
            }

            // Make web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://itch.io/api/1/x/wharf/latest?game_id=964088&channel_name={0}", channelName));
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                Debug.LogWarning("Failed to fetch latest version of Flowery Fields...");
                return;
            }

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = await reader.ReadToEndAsync();
            Debug.Log(jsonResponse);
            var responseDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
            
            // only one item on response
            if (responseDictionary.TryGetValue("latest", out string latestVersion)) // returned latest version
            {
                Debug.Log("Current version: " + Application.version);
                Debug.Log("Latest version: " + latestVersion);
                var result = Application.version.CompareTo(latestVersion.Trim('v'));
                if (result > 0)
                    Debug.LogWarning("Running higher game version than released version");
                else if (result < 0)
                {
                    updateReminderTriggered = true;

                    Debug.LogWarning("New update available on itch!");
                    popupManager.ShowDecisionWindowPopup("We found another flowery patch for you!", "A new version of Flowery Fields is available! Would you like to go download it now?", willDownload =>
                    {
                        if (!willDownload)
                            return;

                        // Go to my itch page and then close the game
                        Application.OpenURL("https://second-120.itch.io/flowery-fields");
                        Application.Quit();
                    }, true);
                }
                else
                    Debug.Log("Flowery Fields is up to date! :)");
                return;
            }
            else if (responseDictionary.TryGetValue("errors", out string error))
            {
                Debug.LogWarning("Error trying to fetch lates game version: " + error);
            }
            else // No values or other error
            {
                if (responseDictionary.Count == 0)
                {
                    Debug.Log("Looks like the latest version isn't avalible for you yet. :(");
                }
                else
                {
                    Debug.LogWarning("Something weird happened while fetching the latest version of the game");
                }
            }
        }
    }
}