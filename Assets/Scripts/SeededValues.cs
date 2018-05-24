using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class SeededValues
    {
        private static float m_DirtHeightSpan = 1000;

        public static float DirtHeightOffset { get; private set; }
        
        public static void RandomiseValues(int randomisationSeed)
        {
            var rand = new Random(randomisationSeed);
            
            DirtHeightOffset = (float)(rand.NextDouble() * m_DirtHeightSpan + (32000 - m_DirtHeightSpan/2));
        }
    }
}
