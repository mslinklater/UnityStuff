using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugUI : MonoBehaviour
{
	private bool isVisible = true;

    // Start is called before the first frame update
    void Start()
    {
		// set to be permenant - TODO: Only if debug
        DontDestroyOnLoad( gameObject );
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnGUI()
	{
		if(isVisible)
		{
			if(GUI.Button( new Rect(0.0f, 20.0f, 200.0f, 20.0f), "Scratch"))
			{
				SceneManager.LoadScene("Scratch");
			}
		}
	}
}
