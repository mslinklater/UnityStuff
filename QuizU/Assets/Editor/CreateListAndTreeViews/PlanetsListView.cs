using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlanetsListView : PlanetsWindow
{
    [MenuItem("Planets/Standard List")]
    static void Summon()
    {
        GetWindow<PlanetsListView>("Standard Planet List");
    }

    void CreateGUI()
    {
        uxml.CloneTree(rootVisualElement);
        var listView = rootVisualElement.Q<ListView>();

        listView.itemsSource = planets;

        listView.makeItem = () => new Label();

        listView.bindItem = (VisualElement element, int index) =>
            (element as Label).text = planets[index].name;
    }
}
