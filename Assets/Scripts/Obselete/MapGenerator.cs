using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, SpriteMap};
    public DrawMode drawMode;

    public GameObject tilePrefab;
    public static Dictionary<Vector2Int, Chunk> chunks;

    public int chunkSize;
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public Vector2Int trueOffset;
    [Range(-1, 1)]
    public float valueOffset;
    public Vector2 bounds;

    public bool autoUpdate;

    public TerrainType[] regions;

    [SerializeField]
    private Biome[] biomesT;
    public static Biome[] biomes;
    
    public void GenerateMap() 
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, valueOffset, noiseScale, octaves, persistance, lacunarity, offset, trueOffset, bounds);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        int[] valueMap = new int[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++) 
        {
            for (int x = 0; x < mapWidth; x++) 
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height) 
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        valueMap[y * mapWidth + x] = i;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if (drawMode == DrawMode.SpriteMap) 
        {
            
        }
    }

    private void OnValidate()
    {
        biomes = biomesT;
        if (mapWidth < 1) 
        {
            mapWidth = 1;
        }
        if (mapHeight < 1) 
        {
            mapHeight = 1;
        }
        if (lacunarity < 1) 
        {
            lacunarity = 1;
        }
        if (octaves < 0) 
        {
            octaves = 0;
        }
        if (drawMode == DrawMode.SpriteMap)
        {
            autoUpdate = false;
        }
    }
}

[System.Serializable]
public struct TerrainType 
{
    public string name;
    public float height;
    public Color color;
}

