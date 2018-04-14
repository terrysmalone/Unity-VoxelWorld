using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    private static int m_MaxHeight = 150;       // Max height of terrain
    private static float m_Smooth = 0.005f;      // Increment of terrain 
    private static int m_Octaves = 4;           // Number of waves to combine
    private static float m_Persistence = 0.5f;    // How much influence each new octave has (0.5 is even)

    private static int m_StoneOffset = 20;      //How far below dirt we get stone

    /// <summary>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="stoneOffset"></param>
    /// <param name="smoothMultiplication">Difference in smoothing function from dirt layer</param>
    /// <param name="octaveDifference">Difference in octaves from dirt layer</param>
    /// <returns></returns>
    public static int GenerateStoneHeight(float x, 
                                          float z, 
                                          int stoneOffset, 
                                          float smoothMultiplication, 
                                          int octaveDifference)
    {
        var height = Map(0, 
                         m_MaxHeight - stoneOffset, 
                         0, 
                         1, 
                         FractalBrownianMotion(x * m_Smooth * smoothMultiplication, 
                                               z * m_Smooth * smoothMultiplication, 
                                               m_Octaves + octaveDifference, 
                                               m_Persistence));

        return (int)height;
    }

    public static int GenerateDirtHeight(float x, float z)
    {
        var height = Map(0, m_MaxHeight, 0, 1, FractalBrownianMotion(x * m_Smooth,
                                                                     z * m_Smooth,
                                                                     m_Octaves,
                                                                     m_Persistence));

        return (int)height;
    }

    private static float Map(float newMin, float newMax, float originalMin, float originalMax, float value)
    {
        return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(originalMin, originalMax, value));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="octaves">Number of waves to combine</param>
    /// <param name="persistence">How much influence each new octave has</param>
    /// <returns></returns>
    private static float FractalBrownianMotion(float x, float z, float octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;

        for (var i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total/maxValue;
    }
}
