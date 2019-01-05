using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AutomatedBuild
{
	public static void Build()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Level I.unity", "Assets/Scenes/Level II.unity", "Assets/Scenes/Level III.unity" };
		buildPlayerOptions.locationPathName = "/data/builds/PROJECT_NAME/";
		buildPlayerOptions.target = BuildTarget.WebGL;
		buildPlayerOptions.options = BuildOptions.None;
		BuildPipeline.BuildPlayer(buildPlayerOptions);

		#buildPlayerOptions.locationPathName = "/data/builds/PROJECT_NAME.apk";
		#buildPlayerOptions.target = BuildTarget.Android;
		#BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}
