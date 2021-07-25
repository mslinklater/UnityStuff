using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ApplicationSettings", order = 1)]
public class ApplicationSettings : ScriptableObject
{
	public string bootstrapScene;
	public string watermark = "(c) Martin Linklater 2021";
}
