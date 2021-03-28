using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

public class BuildAutomation
{
	public struct BuildTargetAndGroup
	{
		public BuildTargetGroup group;
		public BuildTarget target;
		public string platformName;
		public string extension;
		public BuildTargetAndGroup(BuildTargetGroup group, BuildTarget target, string platformName = null, string extension = null)
		{
			this.group = group;
			this.target = target;
			this.platformName = platformName ?? target.ToString();
			this.extension = extension;
		}
	}

	[MenuItem("Build/Build all")]
	public static void BuildAll()
	{
		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Applications", "Builds", "");
		BuildApplication(path);
	}
	public static void BuildApplication(string path)
	{
		string appName = "Flowery Fields";
		Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Icon.png", typeof(Texture2D));
		PlayerSettings.productName = "Flowery Fields";
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.second120.floweryfields");
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.Second-120.FloweryFields");
		PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, "com.Second-120.FloweryFields");
		PlayerSettings.defaultScreenWidth = 900;
		PlayerSettings.defaultScreenHeight = 600;
		PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;
        //...

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new string[] { "Main Menu" }.Select(sceneName => path = "Assets/Scenes/" + sceneName).ToArray(), // Append the needed strings to the scene paths
            locationPathName = path,
            options = BuildOptions.None
        };

        BuildForTargetes(buildPlayerOptions, appName, icon,
			new BuildTargetAndGroup(BuildTargetGroup.WebGL, BuildTarget.WebGL),
			new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, "Windows", ".exe"),
			new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, "macOS"),
			new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64, "Linux", ".x86_64"));
	}

	public static void BuildForTargetes(BuildPlayerOptions options, string appName, Texture2D icon, params BuildTargetAndGroup[] targets)
	{
		string locationPathName = options.locationPathName;
		foreach (BuildTargetAndGroup target in targets)
		{

			// https://forum.unity.com/threads/cant-change-resolution-for-standalone-build.323931/
			PlayerSettings.SetApplicationIdentifier(target.group, PlayerSettings.applicationIdentifier);
			DeletePreference();

			options.targetGroup = target.group;
			options.target = target.target;
			options.locationPathName = locationPathName + "/" + appName + " " + target.platformName + (target.extension != null ? "/" + appName + target.extension : "");
			SetIconForTargetGroup(options.targetGroup, icon);
			//Debug.Log("building " + options.locationPathName);
			BuildPipeline.BuildPlayer(options);
		}
	}
	public static void SetIconForTargetGroup(BuildTargetGroup platform, Texture2D icon)
	{
		int[] iconSizes = PlayerSettings.GetIconSizesForTargetGroup(platform);
		Texture2D[] icons = new Texture2D[iconSizes.Length];
		for (int i = 0; i < icons.Length; i++)
		{
			if (icon.width == iconSizes[i])
			{
				icons[i] = icon;
			}
			else
			{
				Texture2D scaledIcon = new Texture2D(icon.width, icon.height, icon.format, false);
				Graphics.CopyTexture(icon, scaledIcon);
				scaledIcon.Resize(iconSizes[i], iconSizes[i], icon.format, false);
				scaledIcon.Apply();
				icons[i] = scaledIcon;
			}
		}
		PlayerSettings.SetIconsForTargetGroup(platform, icons);
	}
	public static void DeletePreference()
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			FileUtil.DeleteFileOrDirectory("~/Library/Preferences/" + PlayerSettings.applicationIdentifier + ".plist");
		}
		else
		{
			Debug.LogWarning("Not sure how to delete Prefferences for this platform!");
			// https://forum.unity.com/threads/cant-change-resolution-for-standalone-build.323931/
		}
	}
}
