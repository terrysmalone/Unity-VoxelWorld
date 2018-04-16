using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{

    public GameObject Player;

    public Material TextureAtlas;
    public static int ColumnHeight = 16;
    public static int ChunkSize = 16;
    public static int buildRadius = 1;
    
    public static Dictionary<string, Chunk> Chunks;

    private bool m_FirstBuild = true;
    private bool m_Building;

    public static int DebugDiamondCount = 0;
    public static int DebugGoldCount = 0;

    //UI
    public Slider loadingBar;
    public Camera camera;
    public Button startButton;

    public static string BuildChunkName(Vector3 v)
    {
        return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
    }

    private IEnumerator BuildWorld()
    {
        m_Building = true;

        var posX = (int) Mathf.Floor(Player.transform.position.x / ChunkSize);
        var posZ = (int)Mathf.Floor(Player.transform.position.z / ChunkSize);

        var totalChunks = (Mathf.Pow(buildRadius * 2+1, 2) * ColumnHeight) *2;
        var processCount = 0;

        for (var z = -buildRadius; z <= buildRadius; z++)
        {
            for(var x = -buildRadius; x <= buildRadius; x++)
            {
                for(var y = 0; y < ColumnHeight; y++)
                {
                    var chunkPosition = new Vector3((x+posX) * ChunkSize,
                                                    y * ChunkSize,
                                                    (z + posZ) * ChunkSize);

                    var chunkName = BuildChunkName(chunkPosition);

                    Chunk chunk;

                    if (Chunks.TryGetValue(chunkName, out chunk))
                    {
                        chunk.status = Chunk.ChunkStatus.Keep;
                        break;  //Break out of this column because they will all be to keep
                    }

                    chunk = new Chunk(chunkPosition, TextureAtlas);
                    chunk.ChunkGameObject.transform.parent = transform;

                    Chunks.Add(chunk.ChunkGameObject.name, chunk);

                    if (m_FirstBuild)
                    {
                        processCount++;
                        loadingBar.value = processCount / totalChunks * 100;
                    }

                    yield return null;
                }
            }
        }

        foreach(var chunk in Chunks)
        {
            if (chunk.Value.status == Chunk.ChunkStatus.Draw)
            {
                chunk.Value.DrawChunk();

                chunk.Value.status = Chunk.ChunkStatus.Keep;
            }

            //delete old chunks here

            chunk.Value.status = Chunk.ChunkStatus.Done;

            if (m_FirstBuild)
            {
                processCount++;
                loadingBar.value = processCount / totalChunks * 100;
            }

            yield return null;
        }

        PrintDebugValues();

        if (m_FirstBuild)
        {
            Player.SetActive(true);

            loadingBar.gameObject.SetActive(false);
            camera.gameObject.SetActive(false);
            startButton.gameObject.SetActive(false);

            m_FirstBuild = false;
        }

        m_Building = false;
    }

    private static void PrintDebugValues()
    {
        var numOfBlocksInChunk = ChunkSize * ChunkSize * ChunkSize;
        var numOfBlocks = buildRadius * buildRadius * numOfBlocksInChunk;

        Debug.Log("Number of Chunks: " + buildRadius * buildRadius);
        Debug.Log("Number of Blocks: " + numOfBlocks);

        Debug.Log("---------------------------------------------");
        
        Debug.Log("Diamonds: " + DebugDiamondCount);
        Debug.Log("Diamonds(per chunk): " + (double)DebugDiamondCount / (buildRadius * buildRadius) + "/" + numOfBlocksInChunk);
        Debug.Log("Diamonds(per block): " + (double)DebugDiamondCount / (double)numOfBlocks);

        Debug.Log("---------------------------------------------");

        Debug.Log("Gold: " + DebugGoldCount);
        Debug.Log("Gold(per chunk): " + (double)DebugGoldCount / (buildRadius * buildRadius) + "/" + numOfBlocksInChunk);
        Debug.Log("Gold(per block): " + (double)DebugGoldCount / (double)numOfBlocks);

    }

    public void StartBuild()
    {
        StartCoroutine(BuildWorld());
    }

    // Use this for initialization
    private void Start ()
    {
        Player.SetActive(false);

        Chunks = new Dictionary<string, Chunk>();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    private void Update ()
    {
        if (!m_Building && !m_FirstBuild)
        {
            StartCoroutine(BuildWorld());
        }
	}
}
