using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

// A chunk is a group of voxels - think a chunk of cubes is a Minecraft world

public class Chunk : MonoBehaviour
{
    public float cubeSize = 1.0f;

    [Range(1,39)]   // 39 max as this means 40*40*40 verts, which is 64K
    public int cubeCount = 5;
    private int vertAxisCount = 6;

    private MeshFilter meshFilter;
    private Vector3[] meshVertices;
    private bool[] cubeOccupied;
    private List<int> triangleIndexes;

    // to do connect to the mesh

    // Start is called before the first frame update
    void Start()
    {
        // Add MeshFilter if one isn't already there

        // rebuild
        RebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RebuildMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        vertAxisCount = cubeCount + 1;
        int numVerts = vertAxisCount * vertAxisCount * vertAxisCount;
        cubeOccupied = new bool[numVerts];
        meshVertices = new Vector3[numVerts];

        meshFilter.sharedMesh.triangles = new int[0];

        // build vertices array
        int i = 0;
        for(int z = 0; z < vertAxisCount ; z++)
        {
            for(int y = 0; y < vertAxisCount; y++)
            {
                for(int x = 0; x < vertAxisCount; x++)
                {
                    cubeOccupied[i] = true;
                    meshVertices[i++] = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);
                }
            }
        }
        meshFilter.sharedMesh.vertices = meshVertices;

        // create triangles
        triangleIndexes = new List<int>();
        for(int z = 0 ; z < cubeCount ; z++)
        {
            for(int y = 0 ; y < cubeCount ; y++)
            {
                for(int x = 0 ; x < cubeCount ; x++)
                {
                    triangleIndexes.Add(IndexForPos(x,y,z));
                    triangleIndexes.Add(IndexForPos(x, y+1, z));                    
                    triangleIndexes.Add(IndexForPos(x+1, y, z));
                }                
            }
        }
        meshFilter.sharedMesh.triangles = triangleIndexes.ToArray();
    }

    private int IndexForPos(int x, int y, int z)
    {
        int ret = 0;
        ret += x;
        ret += y * vertAxisCount;
        ret += z * vertAxisCount * vertAxisCount;
        return ret;
    }

    public void OnDrawGizmos()
    {
        for( int i = 0 ; i < vertAxisCount ; i++ )
        {   
            // min Z
            Debug.DrawLine(meshVertices[IndexForPos(i, 0, 0)], meshVertices[IndexForPos(i, cubeCount, 0)]);
            Debug.DrawLine(meshVertices[IndexForPos(0, i, 0)], meshVertices[IndexForPos(cubeCount, i, 0)]);
            // max Z
            Debug.DrawLine(meshVertices[IndexForPos(i, 0, cubeCount)], meshVertices[IndexForPos(i, cubeCount, cubeCount)]);
            Debug.DrawLine(meshVertices[IndexForPos(0, i, cubeCount)], meshVertices[IndexForPos(cubeCount, i, cubeCount)]);

            // min X
            Debug.DrawLine(meshVertices[IndexForPos(0, i, 0)], meshVertices[IndexForPos(0, i, cubeCount)]);
            Debug.DrawLine(meshVertices[IndexForPos(0, 0, i)], meshVertices[IndexForPos(0, cubeCount, i)]);
            // max X
            Debug.DrawLine(meshVertices[IndexForPos(cubeCount, i, 0)], meshVertices[IndexForPos(cubeCount, i, cubeCount)]);
            Debug.DrawLine(meshVertices[IndexForPos(cubeCount, 0, i)], meshVertices[IndexForPos(cubeCount, cubeCount, i)]);

            // min Y
            Debug.DrawLine(meshVertices[IndexForPos(i, 0, 0)], meshVertices[IndexForPos(i, 0, cubeCount)]);
            Debug.DrawLine(meshVertices[IndexForPos(0, 0, i)], meshVertices[IndexForPos(cubeCount, 0, i)]);
            // max Y
            Debug.DrawLine(meshVertices[IndexForPos(i, cubeCount, 0)], meshVertices[IndexForPos(i, cubeCount, cubeCount)]);
            Debug.DrawLine(meshVertices[IndexForPos(0, cubeCount, i)], meshVertices[IndexForPos(cubeCount, cubeCount, i)]);
        }
    }
}
