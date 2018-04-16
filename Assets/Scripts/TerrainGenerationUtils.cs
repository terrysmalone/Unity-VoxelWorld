using UnityEngine;

public class TerrainGenerationUtils
{
    public static float GenerateCave(float x,
                                     float y,
                                     float z,
                                     float smooth      = 0.05f,
                                     int   octaves     = 3,
                                     float persistence = 0.5f)
    {
        return FractalBrownianMotion3D(x, y, z, smooth, octaves, persistence);
    }

    public static float GenerateResource(float x,
                                         float y,
                                         float z,
                                         float smooth      = 0.01f,
                                         int   octaves     = 2,
                                         float persistence = 0.5f)
    {
        return FractalBrownianMotion3D(x, y, z, smooth, octaves, persistence);
    }


    public static int GenerateTerrain(float x,
                                      float z,
                                      int maxHeight,
                                      float smooth,
                                      int octaves,
                                      float persistence)
    {
        var height = Map(0,
                         maxHeight,
                         0,
                         1,
                         FractalBrownianMotion(x * smooth,
                                               z * smooth,
                                               octaves,
                                               persistence));

        return (int) height;
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

        const float offset = 32000f;

        for (var i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;

            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }
    
    private static float FractalBrownianMotion3D(float x, 
                                                 float y, 
                                                 float z, 
                                                 float smooth, 
                                                 int octaves, 
                                                 float persistence)
    {
        var xSmooth = x * smooth;
        var ySmooth = y * smooth;
        var zSmooth = z * smooth;

        var xy = FractalBrownianMotion(xSmooth, ySmooth, octaves, persistence);
        var yz = FractalBrownianMotion(ySmooth, zSmooth, octaves, persistence);
        var xz = FractalBrownianMotion(xSmooth, zSmooth, octaves, persistence);

        var yx = FractalBrownianMotion(xSmooth, xSmooth, octaves, persistence);
        var zy = FractalBrownianMotion(zSmooth, ySmooth, octaves, persistence);
        var zx = FractalBrownianMotion(zSmooth, xSmooth, octaves, persistence);

        return (xy + yz + xz + yx + zy + zx) / 6.0f;
    }
}
