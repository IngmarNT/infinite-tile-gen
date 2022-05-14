using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tile
{
    private Vector2Int pos;
    public float height { get; private set; }
    public float rainfall { get; private set; }
    public Biome biome { get; private set; }
    public HeightZone heightZone { get; private set; }

    public static HeightZone CalculateHeightZone(float height) {
        if (height <= -0.6f) {
            return HeightZone.DEEP_OCEAN;
        }
        if (height <= -0.4f)
        {
            return HeightZone.SHALLOW_OCEAN;
        }
        if (height <= -0.1f)
        {
            return HeightZone.COASTAL;
        }
        if (height <= 0.0f)
        {
            return HeightZone.MARSH;
        }
        if (height <= 0.1f)
        {
            return HeightZone.LOWLANDS;
        }
        if (height <= 0.3f)
        {
            return HeightZone.LOWER_MONTANE;
        }
        if (height <= 0.5f)
        {
            return HeightZone.MONTANE;
        }
        if (height <= 0.7f)
        {
            return HeightZone.SUBALPINE;
        }
        if (height <= 0.9f)
        {
            return HeightZone.ALPINE;
        }
        if (height <= 1.0f)
        {
            return HeightZone.ALVAR;
        }
        return HeightZone.NULL;
    }

    private void CalculateBiome()
    {
        foreach (Biome b in MapGenerator.biomes) {
            if (heightZone == b.height) {
                if (rainfall <= b.maxRainfall) {
                    biome = b;
                    Debug.Log(biome.name);
                }
            }
        }
    }
}

public enum HeightZone
{
    ALVAR = 500,
    ALPINE = 1000,
    SUBALPINE = 2000,
    MONTANE = 4000,
    LOWER_MONTANE = 8000,
    LOWLANDS = 16000,
    MARSH = 16000,
    COASTAL = 16000,
    SHALLOW_OCEAN = 16000,
    DEEP_OCEAN = 16000,
    NULL,
}