using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

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

	public static Dictionary<BuildTargetAndGroup, bool> BuildTargets = new Dictionary<BuildTargetAndGroup, bool>()
	{
		{ new BuildTargetAndGroup(BuildTargetGroup.WebGL, BuildTarget.WebGL), true },
		{ new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64, "Linux", ".x86_64"), true },
		{ new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX, "macOS"), true },
		{ new BuildTargetAndGroup(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows, "Windows", ".exe"), true }
	};

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
			scenes = EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(),
            locationPathName = path,
            options = BuildOptions.None
        };

        BuildForTargetes(buildPlayerOptions, appName, icon,
			BuildTargets.Where(target => target.Value).Select(target => target.Key).ToArray());
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

public class MultiBuildWindow : EditorWindow
{
	// Add menu named "My Window" to the Window menu
	[MenuItem("Build/Build")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		MultiBuildWindow window = (MultiBuildWindow)EditorWindow.GetWindow(typeof(MultiBuildWindow), false, "Build Menu");
		window.versionNumber = PlayerSettings.bundleVersion;
		window.Show();
	}

	string versionNumber;
	void OnGUI()
	{
		// Platforms
		GUILayout.Label("Platforms", EditorStyles.boldLabel);
		var updatedTargets = new Dictionary<BuildAutomation.BuildTargetAndGroup, bool>();
		foreach (var target in BuildAutomation.BuildTargets)
        {
			updatedTargets[target.Key] = EditorGUILayout.Toggle(target.Key.platformName, target.Value);
        }
		BuildAutomation.BuildTargets = updatedTargets;

		EditorGUILayout.Space();

		versionNumber = EditorGUILayout.TextField(new GUIContent("Version:", "The version number that will be specified in the build"), versionNumber);

		if (GUILayout.Button(new GUIContent("Build!")))
			StartBuild();
	}

	void StartBuild()
    {
		PlayerSettings.bundleVersion = versionNumber;

		Debug.Log("Starting build for version: " + PlayerSettings.bundleVersion);

		// Build
		string buildPath = EditorUtility.SaveFolderPanel("Build location", new System.IO.DirectoryInfo(Application.dataPath).Parent.FullName, "bin");
		if (buildPath.Length > 0)
		{
			// Do the building
			BuildAutomation.BuildApplication(buildPath);

			// Create version number file
			var versionFile = System.IO.File.CreateText(buildPath + "/version.txt");
			versionFile.Write("v" + versionNumber);
			versionFile.Close();

			Close();
		}
    }
}