using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


class ProjectBuilder {
	static string[] SCENES = FindEnabledEditorScenes();
	static string APP_NAME = "TT";
	
	[MenuItem ("Custom/CI/Build Android")]
	static void PerformAndroidBuild ()
	{
		var androidSDKRoot = Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT");
		if (androidSDKRoot == null) 
		{
			androidSDKRoot = "C:\\Program Files (x86)\\Android\\android-sdk";
		}

		EditorPrefs.SetString("AndroidSdkRoot", androidSDKRoot);

		string target_filename = APP_NAME + ".apk";

        var buildPath = Environment.GetEnvironmentVariable("BUILD_PATH");
        if (buildPath != null)
        {
            target_filename = buildPath + "/" + target_filename;
        }

		GenericBuild(SCENES, target_filename, BuildTarget.Android ,BuildOptions.None);
	}
	
	private static string[] FindEnabledEditorScenes() { 
		List<string> EditorScenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}
	
	static void GenericBuild(string[] scenes, string target_filename, BuildTarget build_target, BuildOptions build_options)
	{
		BuildTargetGroup targetGroup = BuildTargetGroup.Unknown;

		switch(build_target)
		{
			case BuildTarget.StandaloneWindows:
			case BuildTarget.StandaloneOSX:

				targetGroup = BuildTargetGroup.Standalone;
				break;

			case BuildTarget.Android:

				targetGroup = BuildTargetGroup.Android;
				break;

			case BuildTarget.iOS:

				targetGroup = BuildTargetGroup.iOS;
				break;
		}
    

		EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, build_target);
        
		BuildPipeline.BuildPlayer(scenes, target_filename, build_target, build_options);
	}
}
