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

    // Main chunk storage

    public List<GameObject> eventhandlers;
    private Dictionary<Vector3,Chunk> chunkDictionary = new Dictionary<Vector3, Chunk>();

    // global chunk resolution
    private int chunkSize = 10;

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
        if(crack != crackInInspector)
        {
            crack = crackInInspector;
        }
    }
#endregion

#region World block API
    public void SetVoxel(int x, int y, int z)
    {
        Vector3 chunkID = new Vector3((x / chunkSize) * chunkSize, (x / chunkSize) * chunkSize, (x / chunkSize) * chunkSize);
        if(!chunkDictionary.ContainsKey(chunkID))
        {
            Chunk newChunk = new Chunk();
            newChunk.voxelCount = chunkSize;
            newChunk.voxelSize = 1.0f;
            chunkDictionary[chunkID] = newChunk;
        }
        // set the voxel on the chunk

        Chunk chunk = chunkDictionary[chunkID];
    }

    public void ClearVoxel(int x, int y, int z)
    {

    }

    public void RebuildAll()
    {

    }
#endregion
}
