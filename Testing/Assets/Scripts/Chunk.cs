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
        Half,
        None,
        LeaveAlone
    }

    private struct DrawBuffers {
        public Vector3[] meshVertices;
        public Color32[] meshColors;
        public List<int> meshIndices;   // = new List<int>();
    }

    public bool showCage = false;
    public bool removeInternalFaces = true;
    public float voxelSize = 1.0f;
    public Population population = Population.LeaveAlone;

    static public int voxelCount = 10;
    
    private int numVoxels = 0;
    private int numVertices = 0;

    private MeshFilter meshFilter;

    private Voxel[] voxels = new Voxel[voxelCount * voxelCount * voxelCount];

    private DrawBuffers drawBuffers = new DrawBuffers();

    private WorldManager worldManager;

    public bool debugPeek = false;

    // IWorldManagerMessageHandler

    public void SetCrack(bool crack)
    {
        Debug.Log("Chunk.SetCrack()");
    }

    void Awake()
    {
        drawBuffers.meshIndices = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        worldManager = FindObjectOfType<WorldManager>();
        worldManager.eventhandlers.Add(gameObject);

        EnsureMeshExists();

        // rebuild
        RebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void EnsureMeshExists()
    {
        // Add required components
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if(meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.triangles = new int[0];
        }
        if(drawBuffers.meshColors == null)
        {
            numVoxels = voxelCount * voxelCount * voxelCount;
            numVertices = numVoxels * 8;
            drawBuffers.meshColors = new Color32[numVertices];
        }
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

                    Voxel voxel = voxels[iVoxel];

                    voxel.firstVertIndex = iVoxel * 8;
                    switch( population )
                    {
                        case Population.All:
                            voxel.material = Voxel.Material.Ground;
                            break;
                        case Population.Half:
                            if(UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
                            {
                                voxel.material = Voxel.Material.Ground;
                            }
                            else
                            {
                                voxel.material = Voxel.Material.Empty;
                            }
                            break;
                        case Population.None:
                            voxel.material = Voxel.Material.Empty;
                            break;
                        case Population.LeaveAlone:
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
                    if(voxels[iVoxel].IsOpaque())
                    {
                        int firstIndex = voxels[iVoxel].firstVertIndex;

                        // Z min
                        drawBuffers.meshIndices.Add(firstIndex);
                        drawBuffers.meshIndices.Add(firstIndex + 2);
                        drawBuffers.meshIndices.Add(firstIndex + 1);
                        drawBuffers.meshIndices.Add(firstIndex + 2);
                        drawBuffers.meshIndices.Add(firstIndex + 3);
                        drawBuffers.meshIndices.Add(firstIndex + 1);

                        // Z max
                        drawBuffers.meshIndices.Add(firstIndex + 4);
                        drawBuffers.meshIndices.Add(firstIndex + 5);
                        drawBuffers.meshIndices.Add(firstIndex + 6);
                        drawBuffers.meshIndices.Add(firstIndex + 6);
                        drawBuffers.meshIndices.Add(firstIndex + 5);
                        drawBuffers.meshIndices.Add(firstIndex + 7);

                        // Y min
                        drawBuffers.meshIndices.Add(firstIndex);
                        drawBuffers.meshIndices.Add(firstIndex + 1);
                        drawBuffers.meshIndices.Add(firstIndex + 4);
                        drawBuffers.meshIndices.Add(firstIndex + 1);
                        drawBuffers.meshIndices.Add(firstIndex + 5);
                        drawBuffers.meshIndices.Add(firstIndex + 4);

                        // Y max
                        drawBuffers.meshIndices.Add(firstIndex + 2);
                        drawBuffers.meshIndices.Add(firstIndex + 6);
                        drawBuffers.meshIndices.Add(firstIndex + 3);
                        drawBuffers.meshIndices.Add(firstIndex + 3);
                        drawBuffers.meshIndices.Add(firstIndex + 6);
                        drawBuffers.meshIndices.Add(firstIndex + 7);

                        // X min
                        drawBuffers.meshIndices.Add(firstIndex);
                        drawBuffers.meshIndices.Add(firstIndex + 4);
                        drawBuffers.meshIndices.Add(firstIndex + 2);
                        drawBuffers.meshIndices.Add(firstIndex + 4);
                        drawBuffers.meshIndices.Add(firstIndex + 6);
                        drawBuffers.meshIndices.Add(firstIndex + 2);

                        // X max
                        drawBuffers.meshIndices.Add(firstIndex + 1);
                        drawBuffers.meshIndices.Add(firstIndex + 3);
                        drawBuffers.meshIndices.Add(firstIndex + 5);
                        drawBuffers.meshIndices.Add(firstIndex + 5);
                        drawBuffers.meshIndices.Add(firstIndex + 3);
                        drawBuffers.meshIndices.Add(firstIndex + 7);
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
                    if(voxels[iVoxel].IsOpaque())
                    {
                        int firstIndex = voxels[iVoxel].firstVertIndex;

                        // Z min
                        if((z == 0) || (!voxels[iVoxel + prevZ].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex);
                            drawBuffers.meshIndices.Add(firstIndex + 2);
                            drawBuffers.meshIndices.Add(firstIndex + 1);
                            drawBuffers.meshIndices.Add(firstIndex + 2);
                            drawBuffers.meshIndices.Add(firstIndex + 3);
                            drawBuffers.meshIndices.Add(firstIndex + 1);
                        }

                        // Z max
                        if((z == voxelCount-1) || (!voxels[iVoxel + nextZ].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex + 4);
                            drawBuffers.meshIndices.Add(firstIndex + 5);
                            drawBuffers.meshIndices.Add(firstIndex + 6);
                            drawBuffers.meshIndices.Add(firstIndex + 6);
                            drawBuffers.meshIndices.Add(firstIndex + 5);
                            drawBuffers.meshIndices.Add(firstIndex + 7);
                        }

                        // Y min
                        if((y == 0) || (!voxels[iVoxel + prevY].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex);
                            drawBuffers.meshIndices.Add(firstIndex + 1);
                            drawBuffers.meshIndices.Add(firstIndex + 4);
                            drawBuffers.meshIndices.Add(firstIndex + 1);
                            drawBuffers.meshIndices.Add(firstIndex + 5);
                            drawBuffers.meshIndices.Add(firstIndex + 4);
                        }

                        // Y max
                        if((y == voxelCount-1) || (!voxels[iVoxel + nextY].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex + 2);
                            drawBuffers.meshIndices.Add(firstIndex + 6);
                            drawBuffers.meshIndices.Add(firstIndex + 3);
                            drawBuffers.meshIndices.Add(firstIndex + 3);
                            drawBuffers.meshIndices.Add(firstIndex + 6);
                            drawBuffers.meshIndices.Add(firstIndex + 7);
                        }

                        // X min
                        if((x == 0) || (!voxels[iVoxel + prevX].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex);
                            drawBuffers.meshIndices.Add(firstIndex + 4);
                            drawBuffers.meshIndices.Add(firstIndex + 2);
                            drawBuffers.meshIndices.Add(firstIndex + 4);
                            drawBuffers.meshIndices.Add(firstIndex + 6);
                            drawBuffers.meshIndices.Add(firstIndex + 2);
                        }

                        // X max
                        if((x == voxelCount-1) || (!voxels[iVoxel + nextX].IsOpaque()))
                        {
                            drawBuffers.meshIndices.Add(firstIndex + 1);
                            drawBuffers.meshIndices.Add(firstIndex + 3);
                            drawBuffers.meshIndices.Add(firstIndex + 5);
                            drawBuffers.meshIndices.Add(firstIndex + 5);
                            drawBuffers.meshIndices.Add(firstIndex + 3);
                            drawBuffers.meshIndices.Add(firstIndex + 7);
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
        BuildVoxelArray();

        // build mesh vertex array
        drawBuffers.meshVertices = new Vector3[numVertices];

        int vertNum = 0;
        for(int i = 0; i < numVoxels ; i++)
        {
            for(int iVert = 0; iVert < 8; iVert++)
            {
                drawBuffers.meshVertices[vertNum] = voxels[i].verts[iVert];
                drawBuffers.meshColors[vertNum] = voxels[i].color;
                vertNum++;
            }
        }

        EnsureMeshExists();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.vertices = drawBuffers.meshVertices;
        meshFilter.sharedMesh.colors32 = drawBuffers.meshColors;

        // build mesh indices
        Assert.IsNotNull(drawBuffers.meshIndices, "meshIndices");
        drawBuffers.meshIndices.Clear();

        if(removeInternalFaces)
        {
            BuildVisibleFaces();
        }
        else
        {
            BuildAllFaces();
        }

        meshFilter.sharedMesh.triangles = drawBuffers.meshIndices.ToArray();
    }

    public void SetVoxel(int x, int y, int z, Voxel.Material material, Color32 color)
    {
        EnsureMeshExists();

        int index = x + (voxelCount * y) + (voxelCount * voxelCount * z);
        Voxel v = voxels[index];
        v.material = material;
        voxels[index] = v;
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
