using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Put one of these in every scene and when you hit Play the common entry point will be triggered

// This is responsible for the real low-level stuff... editor support.
// NO GAME SYSTEMS - that is what the RootManager handles

// Things to add...

// Global exception handling

public class Bootstrap : MonoBehaviour
{
	static Bootstrap _instance = null; 
	static bool _tempInstance = false;

	public ApplicationSettings applicationSettings;

	void Awake()
	{
		if(_instance != null)
		{
			// there is already a Bootstrap instance running so kill yourself.
			_tempInstance = true;
			DestroyImmediate( gameObject );
			return;
		}

		// OK, this is the first Bootstrap instance... let's set stuff up

		_instance = this;
		DontDestroyOnLoad( gameObject );

		RootManager.Init();

		SceneManager.LoadScene( applicationSettings.bootstrapScene );
	}

	void OnDestroy()
	{
		// if this is just a temp instance there is no clean-up to do.
		if( _tempInstance )
			return;

		// clean up all system singletons
		RootManager.Shutdown();
	}

    void Start()
    {
    }

    void Update()
    {
    }

	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, 500.0f, 20.0f), applicationSettings.watermark);
	}
}
