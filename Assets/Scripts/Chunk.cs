using System;
using UnityEngine;

public class Chunk
{
    public Material CubeMaterial;

    public Block[,,] ChunkData;

    public GameObject ChunkGameObject;

    public enum ChunkStatus { Draw, Done, Keep }

    public ChunkStatus status;

    //Terrain generation values
    private readonly float m_CaveProbability = 0.4f;    // 0 = less caves, 1 = more caves
    
    private readonly float m_DiamondProbability = 0.32f; // 0 = less diamond, 1 = more diamonds
    private readonly float m_DiamondMaxHeight = 20;    // Maximum height diamond can spawn

    private readonly float m_GoldProbability = 0.32f;    // 0 = less gold, 1 = more gold
    private readonly float m_GoldMaxHeight = 40; // Maximum height diamond can spawn


    public Chunk(Vector3 position, Material material)
    {
        ChunkGameObject = new GameObject(World.BuildChunkName(position));

        ChunkGameObject.transform.position = position;

        CubeMaterial = material;

        BuildChunk();
    }

    private void BuildChunk()
    {
        ChunkData = new Block[World.ChunkSize, World.ChunkSize, World.ChunkSize];

        for (var z = 0; z < World.ChunkSize; z++)
        {
            var worldZ = (int)(z + ChunkGameObject.transform.position.z);

            for (var y = 0; y < World.ChunkSize; y++)
            {
                var worldY = (int)(y + ChunkGameObject.transform.position.y);

                if (worldY >= 0)
                {

                    for (var x = 0; x < World.ChunkSize; x++)
                    {
                        var pos = new Vector3(x, y, z);

                        var worldX = (int)(x + ChunkGameObject.transform.position.x);

                        var blockType = GenerateBlockType(worldX, worldY, worldZ);

                        ChunkData[x, y, z] = new Block(blockType, pos, ChunkGameObject.gameObject, this);

                        status = ChunkStatus.Draw;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Generally we want to start from the bottom up for terrain 
    /// and from least rare to more rare for resources
    /// </summary>
    /// <param name="worldX"></param>
    /// <param name="worldY"></param>
    /// <param name="worldZ"></param>
    /// <returns></returns>
    private Block.BlockType GenerateBlockType(int worldX, int worldY, int worldZ)
    {
        //if (CheckForChasm(worldX, worldY, worldZ))
        //{
        //    return Block.BlockType.Air;
        //}

        if (worldY == 0)
        {
            return Block.BlockType.Bedrock;
        }

        if (CheckForCave(worldX, worldY, worldZ))
        {
            return Block.BlockType.Air;
        }

        if (CheckForStone(worldX, worldY, worldZ))
        {
            //if (CheckForCave(worldX, worldY, worldZ))
            //{
            //    return Block.BlockType.Air;
            //}

            //Check for anything within  stone
            var blockType = GenerateResources(worldX, worldY, worldZ);

            //If there's nothing in the stone, return stone
            if (blockType == Block.BlockType.None)
            {
                blockType = Block.BlockType.CobbleStoneRaw;
            }

            return blockType;
        }

        //We check for dirt or grasss
        var dirtType = CheckForDirt(worldX, worldY, worldZ);

        //If it's none it's not dirt or grass
        if(dirtType != Block.BlockType.None)
        {
            return dirtType;
        }
        
        //If it's nothing else, make it air
        return Block.BlockType.Air;
    }
    
    private bool CheckForCave(int worldX, int worldY, int worldZ)
    {
        return TerrainGenerationUtils.GenerateCave(worldX,
                                                   worldY,
                                                   worldZ,
                                                   smooth: 0.05f,
                                                   octaves: 3,
                                                   persistence: 0.5f) < m_CaveProbability;
    }

    private bool CheckForChasm(int worldX, int worldY, int worldZ)
    {
        return worldY <= TerrainGenerationUtils.GenerateTerrain(
            worldX,
            worldZ,
            maxHeight: 200,
            smooth: 0.06f,
            octaves: 3,
            persistence: 0.5f);
    }

    private static bool CheckForStone(int worldX, int worldY, int worldZ)
    {
        return worldY <= TerrainGenerationUtils.GenerateTerrain(worldX,
                                                                worldZ,
                                                                maxHeight: 130,
                                                                smooth: 0.014f,
                                                                octaves: 3,
                                                                persistence: 0.05f);
    }

    private static Block.BlockType CheckForDirt(int worldX, int worldY, int worldZ)
    {
        var terrain = GetDirtHeight(worldX, worldZ);
        
        if (worldY == terrain)
        {
            return Block.BlockType.Grass;
        }

        if (worldY < terrain)
        {
            return Block.BlockType.Dirt;
        }

        return Block.BlockType.None;
    }

    internal static int GetDirtHeight(int worldX, int worldZ)
    {
        return TerrainGenerationUtils.GenerateTerrain(worldX,
                                                      worldZ,
                                                      maxHeight: 150,
                                                      smooth: 0.005f,
                                                      octaves: 4,
                                                      persistence: 0.05f);
    }

    private Block.BlockType GenerateResources(int worldX, int worldY, int worldZ)
    {
        //Diamonds
        if (TerrainGenerationUtils.GenerateResource(worldX,
                                                    worldY,
                                                    worldZ,
                                                    smooth: 0.35f,
                                                    octaves: 2,
                                                    persistence: 0.05f) < m_DiamondProbability 
            && worldY <= m_DiamondMaxHeight)
        {
            World.DebugDiamondCount++;

            return Block.BlockType.Diamond;
        }

        //Gold
        if (TerrainGenerationUtils.GenerateResource(worldX,
                                                    worldY,
                                                    worldZ,
                                                    smooth: 0.4f,
                                                    octaves: 2,
                                                    persistence: 0.05f) < m_GoldProbability
            && worldY <= m_GoldMaxHeight)
        {
            World.DebugGoldCount++;

            return Block.BlockType.Gold;
        }

        return Block.BlockType.None;
    }

    public void DrawChunk()
    {
        for (var z = 0; z < World.ChunkSize; z++)
        {
            for (var y = 0; y < World.ChunkSize; y++)
            {
                for (var x = 0; x < World.ChunkSize; x++)
                {
                    if (ChunkData[x, y, z] != null)
                    {
                        ChunkData[x, y, z].Draw();
                    }
                }
            }
        }    
        
        CombineQuads();

        var collider = ChunkGameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = ChunkGameObject.transform.GetComponent<MeshFilter>().mesh;
        status = ChunkStatus.Done;
    }

    /// <summary>
    /// Combine the quads into a cube
    /// </summary>
    private void CombineQuads()
    {
        //Combine children meshes
        var meshFilters = ChunkGameObject.GetComponentsInChildren<MeshFilter>();

        var combine = new CombineInstance[meshFilters.Length];

        var i = 0;

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //Create new mesh on parent object
        var cubeMeshFilter = (MeshFilter)ChunkGameObject.gameObject.AddComponent(typeof(MeshFilter));

        cubeMeshFilter.mesh = new Mesh();

        //Add combined meshes on children as parents mesh
        cubeMeshFilter.mesh.CombineMeshes(combine);

        //Create mesh renderer for cube
        var cubeMeshRenderer = ChunkGameObject.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        cubeMeshRenderer.material = CubeMaterial;

        //Delete all uncombined children
        foreach (Transform quad in ChunkGameObject.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
