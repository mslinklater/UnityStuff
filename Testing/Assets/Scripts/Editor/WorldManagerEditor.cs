using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

public class WorldManagerEditor : EditorWindow
{
    [MenuItem("Tools/World Manager Editor")]
    public static void ShowWOrldManagerEditor()
    {
        EditorWindow wnd = GetWindow<WorldManagerEditor>();
        wnd.titleContent = new GUIContent("World Manager Editor");
    }
}
