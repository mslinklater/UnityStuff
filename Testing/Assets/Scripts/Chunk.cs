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
    public enum ChunkMode {
        Continuous,
        Discreet
    }

    public enum CubeCorner {
        MinX_MinY_MinZ,
        MaxX_MinY_MinZ,
        MinX_MaxY_MinZ,
        MaxX_MaxY_MinZ,
        MinX_MinY_MaxZ,
        MaxX_MinY_MaxZ,
        MinX_MaxY_MaxZ,
        MaxX_MaxY_MaxZ
    }

    public ChunkMode chunkMode = ChunkMode.Continuous;
    public bool showCage = false;
    public float cubeSize = 1.0f;

    [Range(1,39)]   // 39 max as this means 40*40*40 verts, which is 64K
    public int cubeCount = 5;
    private int numCubes = 0;
    private int vertAxisCount = 6;

    private MeshFilter meshFilter;

    private Vector3[] meshVertices;
    private Color32[] meshColors;
    private List<int> meshIndices = new List<int>();

    private bool[] cubeOccupied;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Chunk.Start()");
        // rebuild
        RebuildMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BuildDiscreetMesh()
    {

    }

    private void BuildContinuousMesh()
    {
        int numVerts = vertAxisCount * vertAxisCount * vertAxisCount;
        cubeOccupied = new bool[numCubes];
   
        meshVertices = new Vector3[numVerts];
        meshColors = new Color32[numVerts];

        meshFilter.sharedMesh.triangles = new int[0];

        // build vertices array
        int i = 0;
        for(int z = 0; z < vertAxisCount ; z++)
        {
            for(int y = 0; y < vertAxisCount; y++)
            {
                for(int x = 0; x < vertAxisCount; x++)
                {
//                    cubeOccupied[i] = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
                    meshVertices[i] = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);
                    meshColors[i] = new Color32((byte)(x * 255 / cubeCount), (byte)(y * 255 / cubeCount), (byte)(z * 255 / cubeCount), 255);
                    i++;
                }
            }
        }
        meshFilter.sharedMesh.vertices = meshVertices;
        meshFilter.sharedMesh.colors32 = meshColors;

        // occupancy
        for(int z = 0; z < cubeCount ; z++)
        {
            for(int y = 0; y < cubeCount; y++)
            {
                for(int x = 0; x < cubeCount; x++)
                {
                    cubeOccupied[x + (y * cubeCount) + (z * cubeCount * cubeCount)] = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f;
                }
            }
        }

        // create triangles
        meshIndices.Clear();

        for(int z = 0 ; z < cubeCount ; z++)
        {
            for(int y = 0 ; y < cubeCount ; y++)
            {
                for(int x = 0 ; x < cubeCount ; x++)
                {
                    if(cubeOccupied[x + (y * cubeCount) + (z * cubeCount * cubeCount)])
                    {
                        // Z min face
                        meshIndices.Add(IndexForPos(x,y,z, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y, z, CubeCorner.MinX_MinY_MinZ));

                        meshIndices.Add(IndexForPos(x, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y, z, CubeCorner.MinX_MinY_MinZ));

                        // Z max face
                        meshIndices.Add(IndexForPos(x,y,z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z+1, CubeCorner.MinX_MinY_MinZ));                    

                        meshIndices.Add(IndexForPos(x, y+1, z+1, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y+1, z+1, CubeCorner.MinX_MinY_MinZ));                    

                        // X min face
                        meshIndices.Add(IndexForPos(x,y,z, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z, CubeCorner.MinX_MinY_MinZ));                    

                        meshIndices.Add(IndexForPos(x, y, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z, CubeCorner.MinX_MinY_MinZ));                    

                        // // X max face
                        meshIndices.Add(IndexForPos(x+1,y,z, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y, z+1, CubeCorner.MinX_MinY_MinZ));

                        meshIndices.Add(IndexForPos(x+1, y, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y+1, z+1, CubeCorner.MinX_MinY_MinZ));

                        // Y min face
                        meshIndices.Add(IndexForPos(x,y,z, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x, y, z+1, CubeCorner.MinX_MinY_MinZ));

                        meshIndices.Add(IndexForPos(x+1, y, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x+1, y, z+1, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x, y, z+1, CubeCorner.MinX_MinY_MinZ));

                        // Y max face
                        meshIndices.Add(IndexForPos(x,y+1,z, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x, y+1, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y+1, z, CubeCorner.MinX_MinY_MinZ));                    

                        meshIndices.Add(IndexForPos(x+1, y+1, z, CubeCorner.MinX_MinY_MinZ));                    
                        meshIndices.Add(IndexForPos(x, y+1, z+1, CubeCorner.MinX_MinY_MinZ));
                        meshIndices.Add(IndexForPos(x+1, y+1, z+1, CubeCorner.MinX_MinY_MinZ));                    
                    }
                }                
            }
        }
        meshFilter.sharedMesh.triangles = meshIndices.ToArray();
    }

    public void RebuildMesh()
    {
        // Common mesh rebuilding
        meshFilter = GetComponent<MeshFilter>();
        if(meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        numCubes = cubeCount * cubeCount * cubeCount;
        vertAxisCount = cubeCount + 1;

        if( chunkMode == ChunkMode.Continuous)
        {
            BuildContinuousMesh();
        }
        else
        {
            BuildDiscreetMesh();
        }
    }

    private int IndexForPos(int x, int y, int z, CubeCorner corner)
    {
        if(chunkMode == ChunkMode.Continuous)
        {
            switch(corner)
            {
                case CubeCorner.MinX_MinY_MinZ:
                    break;
                case CubeCorner.MaxX_MinY_MinZ:
                    x += 1;
                    break;
                case CubeCorner.MinX_MaxY_MinZ:
                    y += 1;
                    break;
                case CubeCorner.MaxX_MaxY_MinZ:
                    x += 1;
                    y += 1;
                    break;
                case CubeCorner.MinX_MinY_MaxZ:
                    z += 1;
                    break;
                case CubeCorner.MaxX_MinY_MaxZ:
                    x += 1;
                    z += 1;
                    break;
                case CubeCorner.MinX_MaxY_MaxZ:
                    y += 1;
                    z += 1;
                    break;
                case CubeCorner.MaxX_MaxY_MaxZ:
                    x += 1;
                    y += 1;
                    z += 1;
                    break;
            }
            int ret = 0;
            ret += x;
            ret += y * vertAxisCount;
            ret += z * vertAxisCount * vertAxisCount;
            return ret;
        }
        else
        {
            Debug.LogError("TODO");
            return 0;
        }
    }

    public void OnDrawGizmosSelected()
    {
        if( showCage )
        {
            for( int i = 0 ; i < vertAxisCount ; i++ )
            {   
                // min Z
                Debug.DrawLine(meshVertices[IndexForPos(i, 0, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(i, cubeCount, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(0, i, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, i, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                // max Z
                Debug.DrawLine(meshVertices[IndexForPos(i, 0, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(i, cubeCount, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(0, i, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, i, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);

                // min X
                Debug.DrawLine(meshVertices[IndexForPos(0, i, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(0, i, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(0, 0, i, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(0, cubeCount, i, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                // max X
                Debug.DrawLine(meshVertices[IndexForPos(cubeCount, i, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, i, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(cubeCount, 0, i, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, cubeCount, i, CubeCorner.MinX_MinY_MinZ)] + transform.position);

                // min Y
                Debug.DrawLine(meshVertices[IndexForPos(i, 0, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(i, 0, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(0, 0, i, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, 0, i, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                // max Y
                Debug.DrawLine(meshVertices[IndexForPos(i, cubeCount, 0, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(i, cubeCount, cubeCount, CubeCorner.MinX_MinY_MinZ)] + transform.position);
                Debug.DrawLine(meshVertices[IndexForPos(0, cubeCount, i, CubeCorner.MinX_MinY_MinZ)] + transform.position, meshVertices[IndexForPos(cubeCount, cubeCount, i, CubeCorner.MinX_MinY_MinZ)] + transform.position);
            }
        }
    }
}
