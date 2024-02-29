using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Assertions;

// A chunk is a group of voxels - think a chunk of cubes is a Minecraft world

public class Chunk : MonoBehaviour
{
    public bool showCage = false;
    public bool removeInternalFaces = false;
    public float cubeSize = 1.0f;

    [Range(1,20)]   // 20 max as this means 20*20*20*8 verts, which is 64K
    public int cubeCount = 2;
    private int numCubes = 0;

    private MeshFilter meshFilter;

    private Cube[] cubes;

    private Vector3[] meshVertices;
    private Color32[] meshColors;
    private List<int> meshIndices = new List<int>();
    private bool[] cubeOccupied;

    // Start is called before the first frame update
    void Start()
    {
        // rebuild
        RebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildCubeArray()
    {
        int iCube = 0;
        for(int z = 0; z < cubeCount; z++)
        {
            for(int y = 0; y < cubeCount; y++)
            {
                for(int x = 0; x < cubeCount; x++)
                {
                    int xp = x + 1;
                    int yp = y + 1;
                    int zp = z + 1;

                    Cube cube = new Cube();
                    cube.firstVertIndex = iCube * 8;
//                    cube.occupied = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
                    cube.occupied = true;
//                    cube.numVerts = 8;
                    cube.verts = new Vector3[8];
                    cube.verts[0] = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);
                    cube.verts[1] = new Vector3(xp * cubeSize, y * cubeSize, z * cubeSize);
                    cube.verts[2] = new Vector3(x * cubeSize, yp * cubeSize, z * cubeSize);
                    cube.verts[3] = new Vector3(xp * cubeSize, yp * cubeSize, z * cubeSize);
                    cube.verts[4] = new Vector3(x * cubeSize, y * cubeSize, zp * cubeSize);
                    cube.verts[5] = new Vector3(xp * cubeSize, y * cubeSize, zp * cubeSize);
                    cube.verts[6] = new Vector3(x * cubeSize, yp * cubeSize, zp * cubeSize);
                    cube.verts[7] = new Vector3(xp * cubeSize, yp * cubeSize, zp * cubeSize);

                    cube.color = new Color32((byte)UnityEngine.Random.Range(0.0f, 255.0f), 
                                                (byte)UnityEngine.Random.Range(0.0f, 255.0f),
                                                (byte)UnityEngine.Random.Range(0.0f, 255.0f), 255);

                    cubes[iCube++] = cube;
                }
            }
        }
    }

    private void BuildAllFaces()
    {
        int iCube = 0;
        for(int z = 0; z < cubeCount; z++)
        {
            for(int y = 0; y < cubeCount; y++)
            {
                for(int x = 0; x < cubeCount; x++)
                {
                    if(cubes[iCube].occupied)
                    {
                        int firstIndex = cubes[iCube].firstVertIndex;

                        // Z min
                        meshIndices.Add(firstIndex);
                        meshIndices.Add(firstIndex + 2);
                        meshIndices.Add(firstIndex + 1);
                        meshIndices.Add(firstIndex + 2);
                        meshIndices.Add(firstIndex + 3);
                        meshIndices.Add(firstIndex + 1);

                        // Z max
                        meshIndices.Add(firstIndex + 4);
                        meshIndices.Add(firstIndex + 5);
                        meshIndices.Add(firstIndex + 6);
                        meshIndices.Add(firstIndex + 6);
                        meshIndices.Add(firstIndex + 5);
                        meshIndices.Add(firstIndex + 7);

                        // Y min
                        meshIndices.Add(firstIndex);
                        meshIndices.Add(firstIndex + 1);
                        meshIndices.Add(firstIndex + 4);
                        meshIndices.Add(firstIndex + 1);
                        meshIndices.Add(firstIndex + 5);
                        meshIndices.Add(firstIndex + 4);

                        // Y max
                        meshIndices.Add(firstIndex + 2);
                        meshIndices.Add(firstIndex + 6);
                        meshIndices.Add(firstIndex + 3);
                        meshIndices.Add(firstIndex + 3);
                        meshIndices.Add(firstIndex + 6);
                        meshIndices.Add(firstIndex + 7);

                        // X min
                        meshIndices.Add(firstIndex);
                        meshIndices.Add(firstIndex + 4);
                        meshIndices.Add(firstIndex + 2);
                        meshIndices.Add(firstIndex + 4);
                        meshIndices.Add(firstIndex + 6);
                        meshIndices.Add(firstIndex + 2);

                        // X max
                        meshIndices.Add(firstIndex + 1);
                        meshIndices.Add(firstIndex + 3);
                        meshIndices.Add(firstIndex + 5);
                        meshIndices.Add(firstIndex + 5);
                        meshIndices.Add(firstIndex + 3);
                        meshIndices.Add(firstIndex + 7);
                    }
                    iCube++;
                }
            }
        }
    }

    private void BuildVisibleFaces()
    {

    }

    public void RebuildMesh()
    {
        // Common mesh rebuilding
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.sharedMesh.triangles = new int[0];

        numCubes = cubeCount * cubeCount * cubeCount;
        cubes = new Cube[numCubes];

        BuildCubeArray();

        // build mesh vertex array
        int numVertices = numCubes * 8;
        meshVertices = new Vector3[numVertices];
        meshColors = new Color32[numVertices];

        int vertNum = 0;
        for(int i = 0; i < numCubes ; i++)
        {
            for(int iVert = 0; iVert < 8; iVert++)
            {
                meshVertices[vertNum] = cubes[i].verts[iVert];
                meshColors[vertNum] = cubes[i].color;
                vertNum++;
            }
        }
        meshFilter.sharedMesh.vertices = meshVertices;
        meshFilter.sharedMesh.colors32 = meshColors;

        // build mesh indices
        meshIndices.Clear();

        if(removeInternalFaces)
        {
            BuildVisibleFaces();
        }
        else
        {
            BuildAllFaces();
        }

        Debug.Log("Triangles size: " + meshIndices.Count());
        meshFilter.sharedMesh.triangles = meshIndices.ToArray();
    }

    public void OnDrawGizmosSelected()
    {
        if( showCage )
        {
        }
    }
}
