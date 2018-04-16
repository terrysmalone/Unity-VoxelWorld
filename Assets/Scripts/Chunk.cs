using UnityEngine;

public class Chunk
{
    public Material CubeMaterial;

    public Block[,,] ChunkData;

    public GameObject ChunkGameObject;
    
    //Terrain generation values
    private readonly float m_CaveProbability = 0.45f;    //0 = no caves, 1 = all the caves
    private readonly float m_DiamondProbability = 0.3f; //0 = no diamond, 1 = all the diamonds
    private readonly float m_GoldProbability = 0.35f;    //0 = no diamond, 1 = all the diamonds

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
            for (var y = 0; y < World.ChunkSize; y++)
            {
                for (var x = 0; x < World.ChunkSize; x++)
                {
                    var pos = new Vector3(x, y, z);

                    var worldX = (int)(x + ChunkGameObject.transform.position.x);
                    var worldY = (int)(y + ChunkGameObject.transform.position.y);
                    var worldZ = (int)(z + ChunkGameObject.transform.position.z);

                    Block.BlockType blockType;

                    if (TerrainGenerationUtils.GenerateCave(worldX, 
                                                            worldY, 
                                                            worldZ,
                                                            smooth : 0.05f,
                                                            octaves: 3,
                                                            persistence : 0.5f) < m_CaveProbability)
                    {    
                        blockType = Block.BlockType.Air;               
                    }                                                   
                    else if (worldY <= TerrainGenerationUtils.GenerateStoneHeight(worldX,
                                                                                  worldZ,
                                                                                  stoneOffset: -20,
                                                                                  smooth: 0.01f,
                                                                                  octaves: 3,
                                                                                  persistence: 0.05f)) 
                    {
                        if (TerrainGenerationUtils.GenerateResource(worldX,
                                                                    worldY,
                                                                    worldZ,
                                                                    smooth: 0.35f,
                                                                    octaves: 2,
                                                                    persistence: 0.05f) < m_DiamondProbability)
                        {
                            blockType = Block.BlockType.Diamond;

                            World.DebugDiamondCount++;
                        }
                        else if (TerrainGenerationUtils.GenerateResource(worldX,
                                                                         worldY,
                                                                         worldZ,
                                                                         smooth: 0.4f,
                                                                         octaves: 2,
                                                                         persistence: 0.05f) < m_GoldProbability)
                        {
                            blockType = Block.BlockType.Gold;

                            World.DebugGoldCount++;
                        }
                        else
                        {
                            blockType = Block.BlockType.Stone;
                        }
                    }
                    else if (worldY == TerrainGenerationUtils.GenerateDirtHeight(worldX, 
                                                                                 worldZ,
                                                                                 smooth: 0.002f, 
                                                                                 octaves: 4, 
                                                                                 persistence: 0.5f))
                    {
                        blockType = Block.BlockType.Grass;
                    }
                    else if (worldY < TerrainGenerationUtils.GenerateDirtHeight(worldX,
                                                                                worldZ,
                                                                                smooth: 0.002f, 
                                                                                octaves: 4, 
                                                                                persistence: 0.5f))
                    { 
                        blockType = Block.BlockType.Dirt;                              
                    }                                                                  
                    else                                                        
                    {
                        blockType = Block.BlockType.Air;
                    }

                    ChunkData[x, y, z] = new Block(blockType, pos, ChunkGameObject.gameObject, this);
                }
            }
        }
    }

    public void DrawChunk()
    {
        for (var z = 0; z < World.ChunkSize; z++)
        {
            for (var y = 0; y < World.ChunkSize; y++)
            {
                for (var x = 0; x < World.ChunkSize; x++)
                {
                    ChunkData[x, y, z].Draw();
                }
            }
        }    

        //yield return null;

        CombineQuads();
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
