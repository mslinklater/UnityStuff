using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Systems to bring up

// Logging / Debug assist
// Messages
// Debug UI overlay
// Input (if there is any...)

public static class RootManager
{
	public static BuildManager buildManager = null;

    public static void Init()
    {
		buildManager = new BuildManager();
		buildManager.Init();
	}

    public static void Update()
    {
        
    }

	public static void Shutdown()
	{
		buildManager.Shutdown();
		buildManager = null;
	}
}
