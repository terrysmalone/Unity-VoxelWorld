using UnityEngine;

namespace Assets.Scripts.BlockTypes
{
    public class DiamondBlock : Block
    {
        private readonly Vector2[,] myUVs =
        { 
            /* Top */
            {
                new Vector2(0, 0.875f),
                new Vector2(0.0625f, 0.875f),
                new Vector2(0, 0.9375f),
                new Vector2(0.0625f, 0.9375f)
            },
            /* Side */   
            {
                new Vector2(0, 0.875f),
                new Vector2(0.0625f, 0.875f),
                new Vector2(0, 0.9375f),
                new Vector2(0.0625f, 0.9375f)
            },
            /* Bottom */ 
            {
                new Vector2(0, 0.875f),
                new Vector2(0.0625f, 0.875f),
                new Vector2(0, 0.9375f),
                new Vector2(0.0625f, 0.9375f)
            }
        };

        public DiamondBlock(Vector3 pos, GameObject parent, Chunk owner, Material material)
        {
            m_BlockType = BlockType.Dirt;
            m_Parent = parent;
            m_Owner = owner;
            m_Position = pos;

            CubeMaterial = material;
            IsSolid = true;

            m_BlockUVs = myUVs;
        }
    }

    public class GoldBlock : Block
    {
        private readonly Vector2[,] myUVs =
        { 
            /* Top */
            {
                new Vector2(0.0625f, 0.875f),
                new Vector2(0.125f, 0.875f),
                new Vector2(0.0625f, 0.9375f),
                new Vector2(0.125f, 0.9375f)
            },
            /* Side */   
            {
                new Vector2(0.0625f, 0.875f),
                new Vector2(0.125f, 0.875f),
                new Vector2(0.0625f, 0.9375f),
                new Vector2(0.125f, 0.9375f)
            },
            /* Bottom */ 
            {
                new Vector2(0.0625f, 0.875f),
                new Vector2(0.125f, 0.875f),
                new Vector2(0.0625f, 0.9375f),
                new Vector2(0.125f, 0.9375f)
            }
        };

        public GoldBlock(Vector3 pos, GameObject parent, Chunk owner, Material material)
        {
            m_BlockType = BlockType.Dirt;
            m_Parent = parent;
            m_Owner = owner;
            m_Position = pos;

            CubeMaterial = material;
            IsSolid = true;

            m_BlockUVs = myUVs;
        }
    }
}