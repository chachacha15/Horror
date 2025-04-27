using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Switcher))]
public class SwitcherEditor : Editor
{
    void OnSceneGUI()
    {
        Switcher switcher = (Switcher)target;
        float sceneViewHeight = SceneView.lastActiveSceneView.position.height;

        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(10, sceneViewHeight - 75, 150, 60));

        if (GUILayout.Button("Previous Painting")) switcher.PreviousPainting();
        if (GUILayout.Button("Next Painting")) switcher.NextPainting();

        GUILayout.EndArea();
        Handles.EndGUI();
    }
}