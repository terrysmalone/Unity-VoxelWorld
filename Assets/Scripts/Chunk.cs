using UnityEngine;

public class Chunk
{
    public Material CubeMaterial;

    public Block[,,] ChunkData;

    public GameObject ChunkGameObject;

    public Chunk(Vector3 position, Material material)
    {
        ChunkGameObject = new GameObject(World.BuildChunkName(position));

        ChunkGameObject.transform.position = position;

        CubeMaterial = material;

        BuildChunk();
    }

    void BuildChunk()
    {
        ChunkData = new Block[World.ChunkSize, World.ChunkSize, World.ChunkSize];

        for (var z = 0; z < World.ChunkSize; z++)
        {
            for (var y = 0; y < World.ChunkSize; y++)
            {
                for (var x = 0; x < World.ChunkSize; x++)
                {
                    var pos = new Vector3(x, y, z);

                    Block.BlockType blockType;

                    blockType = Random.Range(0, 100) < 50 ? Block.BlockType.Grass
                                                          : Block.BlockType.Air;

                    //blockType = Block.BlockType.Grass;

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
    void CombineQuads()
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
