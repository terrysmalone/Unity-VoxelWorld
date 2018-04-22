using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{

    public GameObject Player;

    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static int BuildRadius = 4;

    public static ConcurrentDictionary<string, Chunk> Chunks;
    
    private bool m_FirstBuild = true;

    private Vector3 lastBuildPos;

    public static int DebugDiamondCount;
    public static int DebugGoldCount;

    private static int m_DebugChunksBuilt;
    private static int m_DebugChunksDrawn;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    private void BuildChunkAt(int xPos, int yPos, int zPos)
    {
        m_DebugChunksBuilt++;

        var chunkPosition = new Vector3((xPos) * ChunkSize,
                                        yPos * ChunkSize,
                                        (zPos) * ChunkSize);

        var chunkName = BuildChunkName(chunkPosition);

        Chunk chunk;

        if (!Chunks.TryGetValue(chunkName, out chunk))
        {
            chunk = new Chunk(chunkPosition, TextureAtlas);
            chunk.ChunkGameObject.transform.parent = transform;
            Chunks.TryAdd(chunk.ChunkGameObject.name, chunk);
        }
    }

    private IEnumerator BuildRecursiveWorld(int xPos, int yPos, int zPos, int radius)
    {
        radius--;

        if (radius <= 0) yield break;

        BuildChunkAt(xPos, yPos, zPos - 1);
        StartCoroutine(BuildRecursiveWorld(xPos, yPos, zPos - 1, radius));
        yield return null;
        BuildChunkAt(xPos, yPos, zPos + 1);
        StartCoroutine(BuildRecursiveWorld(xPos, yPos, zPos + 1, radius));
        yield return null;
        BuildChunkAt(xPos, yPos - 1, zPos);
        StartCoroutine(BuildRecursiveWorld(xPos, yPos - 1, zPos, radius));
        yield return null;
        BuildChunkAt(xPos, yPos + 1, zPos);
        StartCoroutine(BuildRecursiveWorld(xPos, yPos + 1, zPos, radius));
        yield return null;
        BuildChunkAt(xPos - 1, yPos, zPos);
        StartCoroutine(BuildRecursiveWorld(xPos - 1, yPos, zPos, radius));
        yield return null;
        BuildChunkAt(xPos + 1, yPos, zPos);
        StartCoroutine(BuildRecursiveWorld(xPos + 1, yPos, zPos, radius));

        yield return null;
    }

    private static IEnumerator DrawChunks()
    {
        foreach (var chunk in Chunks)
        {
            if (chunk.Value.status == Chunk.ChunkStatus.Draw)
            {
                m_DebugChunksDrawn++;

                chunk.Value.DrawChunk();
            }

            yield return null;
        }
    }
    
    private static void PrintDebugValues()
    {
        var numOfBlocksInChunk = ChunkSize * ChunkSize * ChunkSize;
        var numOfBlocks = BuildRadius * BuildRadius * numOfBlocksInChunk;

        Debug.Log("Number of Chunks: " + BuildRadius * BuildRadius);
        Debug.Log("Number of Blocks: " + numOfBlocks);

        Debug.Log("---------------------------------------------");
        
        Debug.Log("Diamonds: " + DebugDiamondCount);
        Debug.Log("Diamonds(per chunk): " + (double)DebugDiamondCount / (BuildRadius * BuildRadius) + "/" + numOfBlocksInChunk);
        Debug.Log("Diamonds(per block): " + (double)DebugDiamondCount / (double)numOfBlocks);

        Debug.Log("---------------------------------------------");

        Debug.Log("Gold: " + DebugGoldCount);
        Debug.Log("Gold(per chunk): " + (double)DebugGoldCount / (BuildRadius * BuildRadius) + "/" + numOfBlocksInChunk);
        Debug.Log("Gold(per block): " + (double)DebugGoldCount / (double)numOfBlocks);

    }

    private void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");

        StartCoroutine(BuildRecursiveWorld((int)(Player.transform.position.x / ChunkSize),
                                           (int)(Player.transform.position.y / ChunkSize),
                                           (int)(Player.transform.position.z / ChunkSize),
                                           BuildRadius));
    }

    // Use this for initialization
    private void Start ()
    {

        var playerPos = Player.transform.position;

        Player.transform.position = new Vector3(playerPos.x,
                                                Chunk.GetDirtHeight((int)playerPos.x, (int)playerPos.z) + 1,
                                                playerPos.z);

        lastBuildPos = Player.transform.position;

        Player.SetActive(false);

        m_FirstBuild = true;
        
        Chunks = new ConcurrentDictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Build starting chunk
        BuildChunkAt((int)(Player.transform.position.x/ChunkSize),
                     (int)(Player.transform.position.y / ChunkSize),
                     (int)(Player.transform.position.z / ChunkSize));

        //Draw it
        StartCoroutine(DrawChunks());

        //Expand world
        StartCoroutine(BuildRecursiveWorld((int) (Player.transform.position.x / ChunkSize),
                                           (int) (Player.transform.position.y / ChunkSize),
                                           (int) (Player.transform.position.z / ChunkSize),
                                           BuildRadius));

        Debug.Log("ChunksBuilt: " + m_DebugChunksBuilt);

        Debug.Log("ChunksDrawn: " + m_DebugChunksDrawn);
    }

    // Update is called once per frame
    private void Update ()
    {
        Vector3 movement = lastBuildPos - Player.transform.position;

        if(movement.magnitude > ChunkSize)
        {
            lastBuildPos = Player.transform.position;
            BuildNearPlayer();
        }

        if (!Player.activeSelf)
        {
            Player.SetActive(true);
            m_FirstBuild = false;
        }

        StartCoroutine(DrawChunks());

        Debug.Log("Player position: " + Player.transform.position);
    }
}
