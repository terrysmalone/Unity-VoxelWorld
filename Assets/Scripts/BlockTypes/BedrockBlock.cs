using UnityEngine;

namespace Assets.Scripts.BlockTypes
{
    public class BedrockBlock : Block
    {
        private readonly Vector2[,] myUVs =
        { 
            /* Top */
            {
                new Vector2(0.25f, 0.9375f),
                new Vector2(0.3125f, 0.9375f),
                new Vector2(0.25f, 1f),
                new Vector2(0.3125f, 1f)
            },
            /* Side */   
            {
                new Vector2(0.25f, 0.9375f),
                new Vector2(0.3125f, 0.9375f),
                new Vector2(0.25f, 1f),
                new Vector2(0.3125f, 1f)
            },
            /* Bottom */ 
            {
                new Vector2(0.25f, 0.9375f),
                new Vector2(0.3125f, 0.9375f),
                new Vector2(0.25f, 1f),
                new Vector2(0.3125f, 1f)
            },
        };

        public BedrockBlock(Vector3 pos, GameObject parent, Chunk owner, Material material)
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