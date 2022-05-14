using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float valueOffset, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, Vector2Int trueOffset, Vector2 bounds, HeightZone[,] heightScalar = null, float exponent = 1) 
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) 
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;

            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) 
        {
            scale = 0.0001f;
        }

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) 
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float frequency = 1.0f;
                float amplitude = 1.0f;
                float noiseHeight = 0.0f;
                float superpositionCompensation = 0.0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX + (trueOffset.x / scale * frequency), sampleY + (trueOffset.y / scale * frequency));
                    //Debug.Log(perlinValue);
                    noiseHeight += perlinValue * amplitude;
                    noiseHeight -= superpositionCompensation;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;
                }
                if (exponent != 1) { noiseHeight = Mathf.Pow(noiseHeight, exponent); }

                noiseMap[x, y] = noiseHeight;
                //Debug.Log(noiseMap[x, y]);
            }
        }

        float noiseHeightRange = bounds.y - bounds.x;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (heightScalar != null) {
                    bounds.y = (float)heightScalar[x, y];
                    noiseHeightRange = bounds.y - bounds.x;
                }
                noiseMap[x, y] = Mathf.InverseLerp(bounds.x, bounds.y, noiseMap[x, y] + (valueOffset * (noiseHeightRange / 2))) * 2 - 1;
                //Debug.Log(noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
