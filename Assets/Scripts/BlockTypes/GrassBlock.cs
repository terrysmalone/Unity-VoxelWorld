using UnityEngine;

namespace Assets.Scripts.BlockTypes
{
    public class GrassBlock : Block
    {
        private readonly Vector2[,] myUVs =
        { 
            /* Top */
            {
                new Vector2(0.125f, 0.9375f),
                new Vector2(0.1875f, 0.9375f),
                new Vector2(0.125f, 1f),
                new Vector2(0.1875f, 1f)
            },
            /* Side */   
            {
                new Vector2(0.0625f, 0.9375f),
                new Vector2(0.125f, 0.9375f),
                new Vector2(0.0625f, 1f),
                new Vector2(0.125f, 1f)
            },
            /* Bottom */ 
            {
                new Vector2(0, 0.9375f),
                new Vector2(0.0625f, 0.9375f),
                new Vector2(0, 1f),
                new Vector2(0.0625f, 1f)
            }
        };

        public GrassBlock(Vector3 pos, GameObject parent, Chunk owner, Material material)
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