﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block {

    public enum BlockType { Grass, Dirt, Stone, Air };

    enum CubeSide { Bottom, Top, Left, Right, Front, Back };

    public bool isSolid;

    BlockType m_BlockType;
    GameObject m_Parent;
    Chunk m_Owner;
    Vector3 m_Position;
    //Material m_CubeMaterial;
    
    Vector2[,] blockUVs =
    {
        /* Grass top */ { new Vector2(0.125f, 0.375f),
                          new Vector2(0.1875f, 0.375f),
                          new Vector2(0.125f, 0.4375f),
                          new Vector2(0.1875f, 0.4375f) },

        /* Grass side */ { new Vector2(0.1875f, 0.9375f),
                           new Vector2(0.25f, 0.9375f),
                           new Vector2(0.1875f, 1.0f),
                           new Vector2(0.25f, 1.0f) },

        /* Dirt */ { new Vector2(0.125f, 0.9375f),
                     new Vector2(0.1875f, 0.9375f),
                     new Vector2(0.125f, 1.0f),
                     new Vector2(0.1875f, 1.0f) },

        /* Stone */ { new Vector2(0, 0.875f),
                      new Vector2(0.0625f, 0.875f),
                      new Vector2(0, 0.9375f),
                      new Vector2(0.0625f, 0.9375f) }
    };

    public Block(BlockType blockType, Vector3 position, GameObject parent, Chunk owner)
    {
        m_BlockType = blockType;
        m_Parent = parent;
        m_Position = position;
        m_Owner = owner;
        
        if (m_BlockType == BlockType.Air)
        {
            isSolid = false;
        }
        else
        {
            isSolid = true;
        }
    }

    public void Draw()
    {
        if (m_BlockType == BlockType.Air) return;
        
        if(!HasSolidNeighbour((int)m_Position.x, (int)m_Position.y, (int)m_Position.z+1))
            CreateQuad(CubeSide.Front);

        if (!HasSolidNeighbour((int)m_Position.x, (int)m_Position.y, (int)m_Position.z-1))
            CreateQuad(CubeSide.Back);

        if (!HasSolidNeighbour((int)m_Position.x, (int)m_Position.y+1, (int)m_Position.z))
            CreateQuad(CubeSide.Top);

        if (!HasSolidNeighbour((int)m_Position.x, (int)m_Position.y-1, (int)m_Position.z))
            CreateQuad(CubeSide.Bottom);

        if (!HasSolidNeighbour((int)m_Position.x-1, (int)m_Position.y, (int)m_Position.z))
            CreateQuad(CubeSide.Left);

        if (!HasSolidNeighbour((int)m_Position.x+1, (int)m_Position.y, (int)m_Position.z))
            CreateQuad(CubeSide.Right);
    }

    void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh
        {
            name = "ScriptedMesh"
        };

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //Set UV's
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        SetUVs(side, out uv00, out uv10, out uv01, out uv11);

        //All possible vertices
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);

        switch (side)
        {
            case CubeSide.Bottom:
                vertices = new Vector3[] { p0, p1, p2, p3 };

                normals = new Vector3[] { Vector3.down,
                                          Vector3.down,
                                          Vector3.down,
                                          Vector3.down };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Top:
                vertices = new Vector3[] { p7, p6, p5, p4 };

                normals = new Vector3[] { Vector3.up,
                                          Vector3.up,
                                          Vector3.up,
                                          Vector3.up };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Left:
                vertices = new Vector3[] { p7, p4, p0, p3 };

                normals = new Vector3[] { Vector3.left,
                                          Vector3.left,
                                          Vector3.left,
                                          Vector3.left };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Right:
                vertices = new Vector3[] { p5, p6, p2, p1 };

                normals = new Vector3[] { Vector3.right,
                                          Vector3.right,
                                          Vector3.right,
                                          Vector3.right };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Front:
                vertices = new Vector3[] { p4, p5, p1, p0 };

                normals = new Vector3[] { Vector3.forward,
                                          Vector3.forward,
                                          Vector3.forward,
                                          Vector3.forward };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;

            case CubeSide.Back:
                vertices = new Vector3[] { p6, p7, p3, p2 };

                normals = new Vector3[] { Vector3.back,
                                          Vector3.back,
                                          Vector3.back,
                                          Vector3.back };

                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };

                triangles = new int[] { 3, 1, 0, 3, 2, 1 };

                break;
        };

        // Put them into mesh
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("Quad");
        quad.transform.position = m_Position;
        quad.transform.parent = m_Parent.transform;

        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;

        //MeshRenderer renderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        //renderer.material = m_CubeMaterial;
    }

    private void SetUVs(CubeSide cubeSide, out Vector2 uv00, out Vector2 uv10, out Vector2 uv01, out Vector2 uv11)
    {
        if (m_BlockType == BlockType.Grass && cubeSide == CubeSide.Top)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }        
        else if (m_BlockType == BlockType.Grass && cubeSide == CubeSide.Bottom)
        {
            uv00 = blockUVs[2, 0];
            uv10 = blockUVs[2, 1];
            uv01 = blockUVs[2, 2];
            uv11 = blockUVs[2, 3];
        }
        else if (m_BlockType == BlockType.Grass)
        {
            uv00 = blockUVs[1, 0];
            uv10 = blockUVs[1, 1];
            uv01 = blockUVs[1, 2];
            uv11 = blockUVs[1, 3];
        }
        else
        {
            uv00 = blockUVs[(int)(m_BlockType + 1), 0];
            uv10 = blockUVs[(int)(m_BlockType + 1), 1];
            uv01 = blockUVs[(int)(m_BlockType + 1), 2];
            uv11 = blockUVs[(int)(m_BlockType + 1), 3];
        }
    }

    public bool HasSolidNeighbour(int x, int y, int z)
    {
        Block[,,] chunks = m_Owner.chunkData;

        try
        {
            return chunks[x, y, z].isSolid;
        }
        catch (System.IndexOutOfRangeException ex) { }

        return false;
    }
}

struct OffsetPoint
{
    public Block.BlockType BlockType;

    public int X, Y;
}