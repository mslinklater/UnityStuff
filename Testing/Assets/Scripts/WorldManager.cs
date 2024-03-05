using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IWorldManagerMessageHandler : IEventSystemHandler
{
    void SetCrack(bool crack);
}

public class WorldManager : MonoBehaviour
{
    public bool crackInInspector = false;
    public bool crack { get { return _crack; } 
                        set { 
                                _crack = value; 
                                foreach(GameObject obj in eventhandlers)
                                {
                                    obj.SendMessage("Crack", _crack, SendMessageOptions.DontRequireReceiver);
                                }
                            } }
    private bool _crack = false;

    public GameObject chunkPrefab;

    // Main chunk storage

    public List<GameObject> eventhandlers;
    private Dictionary<Vector3,GameObject> chunkDictionary = new Dictionary<Vector3, GameObject>();

    // global chunk resolution
//    private int chunkSize = 10;

#region Lifecycle
    // Start is called before the first frame update
    void Start()
    {
        // remove all existing chunks
        Chunk[] chunks = FindObjectsByType<Chunk>(FindObjectsSortMode.None);
        foreach( Chunk chunk in chunks )
        {
            Destroy(chunk.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        SetVoxel((int)UnityEngine.Random.Range(0, 50),
                (int)UnityEngine.Random.Range(0, 50),
                (int)UnityEngine.Random.Range(0, 50), 
                true,
                new Color32((byte)UnityEngine.Random.Range(0.0f, 255.0f),
                        (byte)UnityEngine.Random.Range(0.0f, 255.0f),
                        (byte)UnityEngine.Random.Range(0.0f, 255.0f),
                        255));

        if(crack != crackInInspector)
        {
            crack = crackInInspector;
        }
    }
#endregion

#region World block API
    public void SetVoxel(int x, int y, int z, bool opaque, Color32 color)
    {
        int c = Chunk.voxelCount;
        Vector3 chunkPos = new Vector3((x / c) * c, (y / c) * c, (z / c) * c);
        if(!chunkDictionary.ContainsKey(chunkPos))
        {
            GameObject newChunkObj = Instantiate(chunkPrefab);
            Chunk chunkComponent = newChunkObj.GetComponent<Chunk>();
            chunkComponent.voxelSize = 1.0f;
            chunkDictionary[chunkPos] = newChunkObj;
            newChunkObj.transform.position = chunkPos;  // a bit strange this
        }
        // set the voxel on the chunk

        GameObject chunkObj = chunkDictionary[chunkPos];
        Chunk chunkComp = chunkObj.GetComponent<Chunk>();

        int localX = x % Chunk.voxelCount;
        int localY = y % Chunk.voxelCount;
        int localZ = z % Chunk.voxelCount;

        chunkComp.SetVoxel(localX, localY, localZ, Voxel.Material.Ground, color);
        chunkComp.RebuildMesh();
    }

    public void RebuildAll()
    {

    }
#endregion
}
