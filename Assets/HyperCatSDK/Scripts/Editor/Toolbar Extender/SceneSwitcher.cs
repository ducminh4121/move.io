using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

[InitializeOnLoad]
public static class SceneSwitcher
{
    static SceneSwitcher()
    {
        ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        foreach (SceneIndex scene in Enum.GetValues(typeof(SceneIndex)))
        {
            if (scene != SceneIndex.None)
            {
                var sceneName = scene.ToString();
                if (GUILayout.Button(new GUIContent(sceneName, $"Open {sceneName} Scene")))
                {
                    SceneHelper.OpenScene(scene);
                }
            }
        }
    }

    static void OnRightToolbarGUI()
    {
        // Test scene here
    }
}