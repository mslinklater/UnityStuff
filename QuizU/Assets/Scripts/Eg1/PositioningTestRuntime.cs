using UnityEngine;
using UnityEngine.UIElements;

public class PositionTestRuntime : MonoBehaviour {
    void OnEnable()
    {
        GetComponent<UIDocument>();
    }
}