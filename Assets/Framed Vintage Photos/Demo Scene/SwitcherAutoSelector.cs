using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class SwitcherAutoSelector
{
    static SwitcherAutoSelector()
    {
        EditorApplication.delayCall += SelectSwitcherObject;
    }

    static void SelectSwitcherObject()
    {
        Switcher switcher = GameObject.FindObjectOfType<Switcher>();
        if (switcher != null) Selection.activeObject = switcher.gameObject;
    }
}
