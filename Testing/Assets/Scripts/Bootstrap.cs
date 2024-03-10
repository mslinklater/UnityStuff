using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UI;
using UnityEngine;

[InitializeOnLoad]
public class Bootstrap
{
    void Awake()
    {
        Debug.Log("Bootstrap.Awake()");

        // lock this in... don't delete while game is running
        DontDestroyOnLoad(gameObject);

        // look for other Bootstrap objects - if you find them, delete them
        Bootstrap[] bootstrapsInScene = FindObjectsByType<Bootstrap>(FindObjectsSortMode.None);
        if(bootstrapsInScene.Count() > 1)
        {
            Debug.LogError("Found multiple Bootstrap objects in scene. This is not allowed.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
