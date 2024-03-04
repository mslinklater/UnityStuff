using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Assertions;

// A chunk is a group of voxels - think a chunk of cubes is a Minecraft world

public class Chunk :    MonoBehaviour, 
                        IWorldManagerMessageHandler
{
    public enum Population {
        All,
        Half
    }

    public bool showCage = false;
    public bool removeInternalFaces = false;
    public float voxelSize = 1.0f;
    public Population population = Population.All;

    [Range(1,20)]   // 20 max as this means 20*20*20*8 verts, which is 64K
    public int voxelCount = 2;
    
    private int numVoxels = 0;

    private MeshFilter meshFilter;

    private Voxel[] voxels;

    private Vector3[] meshVertices;
    private Color32[] meshColors;
    private List<int> meshIndices = new List<int>();

    private WorldManager worldManager;

    public bool debugPeek = false;

    // IWorldManagerMessageHandler

    public void SetCrack(bool crack)
    {
        Debug.Log("Chunk.SetCrack()");
    }

    // Start is called before the first frame update
    void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        worldManager.eventhandlers.Add(gameObject);

        // rebuild
        RebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildVoxelArray()
    {
        int iVoxel = 0;
        for(int z = 0; z < voxelCount; z++)
        {
            for(int y = 0; y < voxelCount; y++)
            {
                for(int x = 0; x < voxelCount; x++)
                {
                    int xp = x + 1;
                    int yp = y + 1;
                    int zp = z + 1;

                    Voxel voxel = new Voxel();
                    voxel.firstVertIndex = iVoxel * 8;
                    switch( population )
                    {
                        case Population.All:
                            voxel.opaque = true;
                            break;
                        case Population.Half:
                            voxel.opaque = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
                            break;
                    }


                    voxel.verts = new Vector3[8];
                    float crack = 0.0f;
                    if(debugPeek)
                    {
                        crack = voxelSize * 0.2f;
                    }
                    voxel.verts[0] = new Vector3(x * voxelSize, y * voxelSize, z * voxelSize);
                    voxel.verts[1] = new Vector3(xp * voxelSize - crack, y * voxelSize, z * voxelSize);
                    voxel.verts[2] = new Vector3(x * voxelSize, yp * voxelSize - crack, z * voxelSize);
                    voxel.verts[3] = new Vector3(xp * voxelSize - crack, yp * voxelSize - crack, z * voxelSize);
                    voxel.verts[4] = new Vector3(x * voxelSize, y * voxelSize, zp * voxelSize - crack);
                    voxel.verts[5] = new Vector3(xp * voxelSize - crack, y * voxelSize, zp * voxelSize - crack);
                    voxel.verts[6] = new Vector3(x * voxelSize, yp * voxelSize - crack, zp * voxelSize - crack);
                    voxel.verts[7] = new Vector3(xp * voxelSize - crack, yp * voxelSize - crack, zp * voxelSize - crack);

                    voxel.color = new Color32((byte)UnityEngine.Random.Range(0.0f, 255.0f), 
                                                (byte)UnityEngine.Random.Range(0.0f, 255.0f),
                                                (byte)UnityEngine.Random.Range(0.0f, 255.0f), 255);

                    voxels[iVoxel++] = voxel;
                }
            }
        }
    }

    private void BuildAllFaces()
    {
        int iVoxel = 0;
        for(int z = 0; z < voxelCount; z++)
        {
            for(int y = 0; y < voxelCount; y++)
            {
                for(int x = 0; x < voxelCount; x++)
                {
                    if(voxels[iVoxel].opaque)
                    {
                        int firstIndex = voxels[iVoxel].firstVertIndex;

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
                    iVoxel++;
                }
            }
        }
    }

    private void BuildVisibleFaces()
    {
        int iVoxel = 0;
        int prevX = -1;
        int nextX = 1;
        int prevY = -voxelCount;
        int nextY = voxelCount;
        int prevZ = -(voxelCount * voxelCount);
        int nextZ = voxelCount * voxelCount;

        for(int z = 0; z < voxelCount; z++)
        {
            for(int y = 0; y < voxelCount; y++)
            {
                for(int x = 0; x < voxelCount; x++)
                {
                    if(voxels[iVoxel].opaque)
                    {
                        int firstIndex = voxels[iVoxel].firstVertIndex;

                        // Z min
                        if((z == 0) || (!voxels[iVoxel + prevZ].opaque))
                        {
                            meshIndices.Add(firstIndex);
                            meshIndices.Add(firstIndex + 2);
                            meshIndices.Add(firstIndex + 1);
                            meshIndices.Add(firstIndex + 2);
                            meshIndices.Add(firstIndex + 3);
                            meshIndices.Add(firstIndex + 1);
                        }
// blah
                        // Z max
                        if((z == voxelCount-1) || (!voxels[iVoxel + nextZ].opaque))
                        {
                            meshIndices.Add(firstIndex + 4);
                            meshIndices.Add(firstIndex + 5);
                            meshIndices.Add(firstIndex + 6);
                            meshIndices.Add(firstIndex + 6);
                            meshIndices.Add(firstIndex + 5);
                            meshIndices.Add(firstIndex + 7);
                        }

                        // Y min
                        if((y == 0) || (!voxels[iVoxel + prevY].opaque))
                        {
                            meshIndices.Add(firstIndex);
                            meshIndices.Add(firstIndex + 1);
                            meshIndices.Add(firstIndex + 4);
                            meshIndices.Add(firstIndex + 1);
                            meshIndices.Add(firstIndex + 5);
                            meshIndices.Add(firstIndex + 4);
                        }

                        // Y max
                        if((y == voxelCount-1) || (!voxels[iVoxel + nextY].opaque))
                        {
                            meshIndices.Add(firstIndex + 2);
                            meshIndices.Add(firstIndex + 6);
                            meshIndices.Add(firstIndex + 3);
                            meshIndices.Add(firstIndex + 3);
                            meshIndices.Add(firstIndex + 6);
                            meshIndices.Add(firstIndex + 7);
                        }

                        // X min
                        if((x == 0) || (!voxels[iVoxel + prevX].opaque))
                        {
                            meshIndices.Add(firstIndex);
                            meshIndices.Add(firstIndex + 4);
                            meshIndices.Add(firstIndex + 2);
                            meshIndices.Add(firstIndex + 4);
                            meshIndices.Add(firstIndex + 6);
                            meshIndices.Add(firstIndex + 2);
                        }

                        // X max
                        if((x == voxelCount-1) || (!voxels[iVoxel + nextX].opaque))
                        {
                            meshIndices.Add(firstIndex + 1);
                            meshIndices.Add(firstIndex + 3);
                            meshIndices.Add(firstIndex + 5);
                            meshIndices.Add(firstIndex + 5);
                            meshIndices.Add(firstIndex + 3);
                            meshIndices.Add(firstIndex + 7);
                        }
                    }
                    iVoxel++;
                }
            }
        }
    }

    public void RebuildMesh()
    {
        // Common mesh rebuilding
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.triangles = new int[0];

        numVoxels = voxelCount * voxelCount * voxelCount;
        voxels = new Voxel[numVoxels];

        BuildVoxelArray();

        // build mesh vertex array
        int numVertices = numVoxels * 8;
        meshVertices = new Vector3[numVertices];
        meshColors = new Color32[numVertices];

        int vertNum = 0;
        for(int i = 0; i < numVoxels ; i++)
        {
            for(int iVert = 0; iVert < 8; iVert++)
            {
                meshVertices[vertNum] = voxels[i].verts[iVert];
                meshColors[vertNum] = voxels[i].color;
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

    public void Crack(bool _crack)
    {
        debugPeek = _crack;
        RebuildMesh();
    }

    public void OnDrawGizmosSelected()
    {
        if( showCage )
        {
            // TODO
        }
    }
}
