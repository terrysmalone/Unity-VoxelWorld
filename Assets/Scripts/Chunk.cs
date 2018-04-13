using System.Collections;
using UnityEngine;

public class Chunk
{
    public Material cubeMaterial;

    public Block[,,] chunkData;

    public GameObject chunk;

    public Chunk(Vector3 position, Material material)
    {
        chunk = new GameObject(World.BuildChunkName(position));

        chunk.transform.position = position;

        cubeMaterial = material;

        BuildChunk();
    }

    void BuildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    Block.BlockType blockType;

                    if (Random.Range(0, 100) < 50)
                    {
                        blockType = Block.BlockType.Grass;
                    }
                    else
                    {
                        blockType = Block.BlockType.Air;
                    }

                    chunkData[x, y, z] = new Block(blockType, pos, chunk.gameObject, this);
                }
            }
        }
    }

    public void DrawChunk()
    {

        for (int z = 0; z < World.chunkSize; z++)
        {
            for (int y = 0; y < World.chunkSize; y++)
            {
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
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
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        //Create new mesh on parent object
        MeshFilter cubeMeshFilter = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));

        cubeMeshFilter.mesh = new Mesh();

        //Add combined meshes on children as parents mesh
        cubeMeshFilter.mesh.CombineMeshes(combine);

        //Create mesh renderer for cube
        MeshRenderer cubeMeshRenderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        cubeMeshRenderer.material = cubeMaterial;

        //Delete all uncombined children
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
