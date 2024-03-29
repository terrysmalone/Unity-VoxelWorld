﻿using System;
using UnityEngine;

public class Block
{

    public enum BlockType { None, Dirt, Grass, CobbleStoneRaw, Bedrock, Diamond, Gold, CobbleStone, Air }

    private enum CubeSide { Bottom, Top, Left, Right, Front, Back }

    public bool IsSolid;

    public Material CubeMaterial;

    internal BlockType m_BlockType;
    internal GameObject m_Parent;
    internal Chunk m_Owner;

    internal Vector3 m_Position;

    internal Vector2[,] m_BlockUVs =
    {
        /*TOP*/			{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
            new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*SIDE*/		{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
            new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*BOTTOM*/		{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
            new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )}
    };
    
    public Block() { }

    public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk owner, Material material)
    {
        m_BlockType = blockType;
        m_Parent = parent;
        m_Position = position;
        m_Owner = owner;

        CubeMaterial = material;

        IsSolid = m_BlockType != BlockType.Air;
    }

    public void Draw()
    {
        if (m_BlockType == BlockType.Air) return;

        if(!HasSolidNeighbour((int) m_Position.x, (int) m_Position.y, (int) m_Position.z + 1))
        {
            CreateQuad(CubeSide.Front);
        }

        if(!HasSolidNeighbour((int) m_Position.x, (int) m_Position.y, (int) m_Position.z - 1))
        {
            CreateQuad(CubeSide.Back);
        }

        if(!HasSolidNeighbour((int) m_Position.x, (int) m_Position.y + 1, (int) m_Position.z))
        {
            CreateQuad(CubeSide.Top);
        }

        if(!HasSolidNeighbour((int) m_Position.x, (int) m_Position.y - 1, (int) m_Position.z))
        {
            CreateQuad(CubeSide.Bottom);
        }

        if (!HasSolidNeighbour((int)m_Position.x-1, (int)m_Position.y, (int)m_Position.z))
        {
            CreateQuad(CubeSide.Left);
        }

        if(!HasSolidNeighbour((int) m_Position.x + 1, (int) m_Position.y, (int) m_Position.z))
        {
            CreateQuad(CubeSide.Right);
        }
    }

    private void CreateQuad(CubeSide side)
    {
        var mesh = new Mesh
        {
            name = "ScriptedMesh"
        };

        var vertices = new Vector3[4];
        var normals = new Vector3[4];
        var uvs = new Vector2[4];
        var triangles = new int[6];

        //Set UV's
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (side == CubeSide.Top)
        {
            uv00 = m_BlockUVs[0, 0];
            uv10 = m_BlockUVs[0, 1];
            uv01 = m_BlockUVs[0, 2];
            uv11 = m_BlockUVs[0, 3];
        }
        else if (side == CubeSide.Bottom)
        {
            uv00 = m_BlockUVs[2, 0];
            uv10 = m_BlockUVs[2, 1];
            uv01 = m_BlockUVs[2, 2];
            uv11 = m_BlockUVs[2, 3];
        }
        else
        {
            uv00 = m_BlockUVs[1, 0];
            uv10 = m_BlockUVs[1, 1];
            uv01 = m_BlockUVs[1, 2];
            uv11 = m_BlockUVs[1, 3];
        }

        //SetUVs(side, out uv00, out uv10, out uv01, out uv11);

        //All possible vertices
        var p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        var p1 = new Vector3(0.5f, -0.5f, 0.5f);
        var p2 = new Vector3(0.5f, -0.5f, -0.5f);
        var p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        var p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        var p5 = new Vector3(0.5f, 0.5f, 0.5f);
        var p6 = new Vector3(0.5f, 0.5f, -0.5f);
        var p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side)
        {
            case CubeSide.Bottom:
                vertices = new[] { p0, p1, p2, p3 };

                normals = new[] { Vector3.down,
                                  Vector3.down,
                                  Vector3.down,
                                  Vector3.down };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Top:
                vertices = new[] { p7, p6, p5, p4 };

                normals = new[] { Vector3.up,
                                  Vector3.up,
                                  Vector3.up,
                                  Vector3.up };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Left:
                vertices = new[] { p7, p4, p0, p3 };

                normals = new[] { Vector3.left,
                                  Vector3.left,
                                  Vector3.left,
                                  Vector3.left };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Right:
                vertices = new[] { p5, p6, p2, p1 };

                normals = new[] { Vector3.right,
                                  Vector3.right,
                                  Vector3.right,
                                  Vector3.right };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Front:
                vertices = new[] { p4, p5, p1, p0 };

                normals = new[] { Vector3.forward,
                                  Vector3.forward,
                                  Vector3.forward,
                                  Vector3.forward };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Back:
                vertices = new[] { p6, p7, p3, p2 };

                normals = new[] { Vector3.back,
                                  Vector3.back,
                                  Vector3.back,
                                  Vector3.back };

                uvs = new[] { uv11, uv01, uv00, uv10 };

                triangles = new[] { 3, 1, 0, 3, 2, 1 };

                break;
        }

        // Put them into mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        var quad = new GameObject("Quad");
        quad.transform.position = m_Position;
        quad.transform.parent = m_Parent.transform;

        var meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
    }

    private void SetUVs(CubeSide cubeSide, out Vector2 uv00, out Vector2 uv10, out Vector2 uv01, out Vector2 uv11)
    {
        if (m_BlockType == BlockType.Grass && cubeSide == CubeSide.Top)
        {
            uv00 = m_BlockUVs[2, 0];
            uv10 = m_BlockUVs[2, 1];
            uv01 = m_BlockUVs[2, 2];
            uv11 = m_BlockUVs[2, 3];
        }        
        else if (m_BlockType == BlockType.Grass && cubeSide == CubeSide.Bottom)
        {
            uv00 = m_BlockUVs[0, 0];
            uv10 = m_BlockUVs[0, 1];
            uv01 = m_BlockUVs[0, 2];
            uv11 = m_BlockUVs[0, 3];
        }
        else if (m_BlockType == BlockType.Grass) 
        {
            uv00 = m_BlockUVs[1, 0];
            uv10 = m_BlockUVs[1, 1];
            uv01 = m_BlockUVs[1, 2];
            uv11 = m_BlockUVs[1, 3];
        }
        else if (m_BlockType == BlockType.Dirt)
        {
            uv00 = m_BlockUVs[0, 0];
            uv10 = m_BlockUVs[0, 1];
            uv01 = m_BlockUVs[0, 2];
            uv11 = m_BlockUVs[0, 3];
        }
        else
        {
            uv00 = m_BlockUVs[(int)(m_BlockType), 0];
            uv10 = m_BlockUVs[(int)(m_BlockType), 1];
            uv01 = m_BlockUVs[(int)(m_BlockType), 2];
            uv11 = m_BlockUVs[(int)(m_BlockType), 3];
        }
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunks;
        
        if (x < 0 || x >= World.ChunkSize 
         || y < 0 || y >= World.ChunkSize 
         || z < 0 || z >= World.ChunkSize)
        {
            //Neighbour is in neighbouring chunk
            
            var neighbouringChunkPos = m_Parent.transform.position +
                                       new Vector3((x - (int) m_Position.x) * World.ChunkSize,
                                                   (y - (int) m_Position.y) * World.ChunkSize,
                                                   (z - (int) m_Position.z) * World.ChunkSize);

            var name = World.BuildChunkName(neighbouringChunkPos);

            x = ConvertBlockIndexToLocal(x);
            y = ConvertBlockIndexToLocal(y);
            z = ConvertBlockIndexToLocal(z);

            Chunk chunk;

            if(World.Chunks.TryGetValue(name, out chunk))
            {
                chunks = chunk.ChunkData;
            }
            else
            {
                return false;
            }
        }
        else 
        {
            //Neighbour is in this chunk
            chunks = m_Owner.ChunkData;
        }
        
        try
        {
            return chunks[x, y, z].IsSolid;
        }
        catch (System.IndexOutOfRangeException ioore)
        {
            Debug.Log("Unknown error when checking for neighbour " + ioore);
        }
        catch (Exception exc)
        {
            Debug.Log($"Unknown error when checking for neighbour at ({x}, {y}, {z})." + exc);
        }

        return false;
    }

    private static int ConvertBlockIndexToLocal(int index)
    {
        if(index == -1)
        {
            return World.ChunkSize - 1;
        }
        else if(index == World.ChunkSize)
        {
            return 0;
        }

        return index;
    }
}
