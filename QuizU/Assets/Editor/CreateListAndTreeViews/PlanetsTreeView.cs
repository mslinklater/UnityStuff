using UnityEngine.UIElements;
using UnityEditor;

public class PlanetsTreeView : PlanetsWindow
{
    [MenuItem("Planets/Standard Tree")]
    static void Summon()
    {
        GetWindow<PlanetsTreeView>("Standard Planet Tree");
    }

    void CreateGUI()
    {
        uxml.CloneTree(rootVisualElement);
        var treeView = rootVisualElement.Q<TreeView>();

        treeView.SetRootItems(treeRoots);

        treeView.makeItem = () => new Label();

        treeView.bindItem = (VisualElement element, int index) => (element as Label).text = treeView.GetItemDataForIndex<IPlanetOrGroup>(index).name;
    }
}
