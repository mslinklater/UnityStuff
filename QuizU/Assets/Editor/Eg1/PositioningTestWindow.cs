using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

public class PositioningTestWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/PositioningTestWindow")]
    public static void ShowExample()
    {
        PositioningTestWindow wnd = GetWindow<PositioningTestWindow>();
        wnd.titleContent = new GUIContent("PositioningTestWindow");
    }

    public void CreateGUI()
    {
        for (int i = 0; i < 2; i++)
        {
            var temp = new VisualElement();
            temp.style.width = 70;
            temp.style.height = 70;
            temp.style.marginBottom = 2;
            temp.style.backgroundColor = Color.gray;
            this.rootVisualElement.Add(temp);
        }

        // Relative positioning
        var relative = new Label("Relative\nPos\n25, 0");
        relative.style.width = 70;
        relative.style.height = 70;
        relative.style.left = 25;
        relative.style.marginBottom = 2;
        relative.style.backgroundColor = new Color(0.21f, 0, 0.254f);
        this.rootVisualElement.Add(relative);

        for (int i = 0; i < 2; i++)
        {
            var temp = new VisualElement();
            temp.style.width = 70;
            temp.style.height = 70;
            temp.style.marginBottom = 2;
            temp.style.backgroundColor = Color.gray;
            this.rootVisualElement.Add(temp);
        }

        // Absolute positioning
        var absolutePositionElement = new Label("Absolute\nPos\n25, 25");
        absolutePositionElement.style.position = Position.Absolute;
        absolutePositionElement.style.top = 25;
        absolutePositionElement.style.left = 25;
        absolutePositionElement.style.width = 70;
        absolutePositionElement.style.height = 70;
        absolutePositionElement.style.backgroundColor = Color.black;
        this.rootVisualElement.Add(absolutePositionElement);
    }
}
