using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chunk))]
public class ChunkInspector : Editor
{
    Chunk chunk;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                chunk.RebuildMesh();
            }
        }
    }

    private void OnEnable()
    {
        chunk = (Chunk)target;
    }
}
