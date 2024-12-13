using UnityEngine.UIElements;
using UnityEditor;

public class PlanetsMultiColumnListView : PlanetsWindow
{
    [MenuItem("Planets/Multicolumn List")]
    static void Summon()
    {
        GetWindow<PlanetsMultiColumnListView>("Multicolumn Planet List");
    }

    void CreateGUI()
    {
        uxml.CloneTree(rootVisualElement);
        var listView = rootVisualElement.Q<MultiColumnListView>();

        listView.itemsSource = planets;

        listView.columns["name"].makeCell = () => new Label();
        listView.columns["populated"].makeCell = () => new Toggle();

        listView.columns["name"].bindCell = (VisualElement element, int index) => (element as Label).text = planets[index].name;
        listView.columns["populated"].bindCell = (VisualElement element, int index) => (element as Toggle).value = planets[index].populated;
    }
}
