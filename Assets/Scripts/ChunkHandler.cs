using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ChunkHandler : MonoBehaviour
{
    private Dictionary<Vector2Int, Chunk> chunks;
    private List<Chunk> activeChunks;

    public static ObjectPool<PoolObject> tilePool;

    public GameObject tilePrefab;

    public int chunkSize;
    public float noiseScale;

    public int renderDistance;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public float heightExponent;
    public int heightDivisions;

    public int seed;
    public Vector2 offset;
    public Vector2Int trueOffset;
    [Range(-1, 1)]
    public float valueOffset;
    public Vector2 bounds;

    public bool autoUpdate;

    [SerializeField]
    private Biome[] biomesT;
    public static Biome[] biomes;

    Queue<ChunkThreadInfo> chunkThreadInfoQueue = new Queue<ChunkThreadInfo>();

    void Awake()
    {
        chunks = new Dictionary<Vector2Int, Chunk>();
        activeChunks = new List<Chunk>();

        int numTiles = chunkSize * ((2 * renderDistance * renderDistance) + 2 * renderDistance + 1);
        tilePool = new ObjectPool<PoolObject>(tilePrefab, numTiles);
    }

    public void RequestChunk(Action<Chunk> callback, Vector2Int pos) {
        ThreadStart threadStart = delegate
        {
            ChunkThread(callback, pos);
        };
        Debug.Log("starting new thread");
        new Thread(threadStart).Start();
    }

    void ChunkThread(Action<Chunk> callback, Vector2Int pos) {
        Chunk chunk = GenerateChunk(pos);
        lock (chunkThreadInfoQueue) {
            chunkThreadInfoQueue.Enqueue(new ChunkThreadInfo(callback, chunk));
        }
    }

    public void OnChunkDataRecieved(Chunk chunk)
    {
        Debug.Log("recieved new chunk data");

        if (chunk.height == null) { return; }

        float[,] height = chunk.height;
        Vector2Int pos = chunk.pos;

        chunks.Add(pos, chunk);
        activeChunks.Add(chunk);
        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(pos.x * chunkSize + i, Mathf.Ceil(height[i, j] * heightDivisions * 8) / heightDivisions, pos.y * chunkSize + j), Quaternion.Euler(90, 0, 0));
                chunk.set(new Vector2Int(i, j), newTile.GetComponent<Tile>());
            }
        }
    }

    void Update()
    {
        if (chunkThreadInfoQueue.Count > 0) {
            for (int i = 0; i < chunkThreadInfoQueue.Count; i++) {
                ChunkThreadInfo threadInfo = chunkThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.param);
            }
        }
    }

    Chunk GenerateChunk(Vector2Int pos)
    {
        if (chunks.ContainsKey(pos)) { return default(Chunk); }

        System.Random r = new System.Random(seed);

        float[,] height = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed + r.Next(1000, 9999), valueOffset, noiseScale, octaves, persistance, lacunarity, offset, pos * chunkSize, bounds, exponent: heightExponent);

        HeightZone[,] zones = new HeightZone[chunkSize, chunkSize];

        for (int i = 0; i < chunkSize; i++) {
            for (int j = 0; j < chunkSize; j++)
            {
                zones[i, j] = Tile.CalculateHeightZone(height[i, j]);
            }
        }

        float[,] rainfall = Noise.GenerateNoiseMap(chunkSize, chunkSize, seed + r.Next(1000, 9999), valueOffset, noiseScale, octaves, persistance, lacunarity, offset, pos * chunkSize, new Vector2(62.5f, 16000.0f), zones);
        
        Chunk chunk = new Chunk(pos, chunkSize, height, rainfall);

        return chunk;

        //chunks.Add(pos, chunk);

        /*for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                GameObject newTile = Instantiate(tilePrefab, new Vector3(pos.x * chunkSize + i, Mathf.Ceil(height[i, j] * heightDivisions * 8) / heightDivisions, pos.y * chunkSize + j), Quaternion.Euler(90, 0, 0));
                chunk.set(new Vector2Int(i, j), newTile.GetComponent<Tile>());
            }
        } execute on main thread*/
    }

    public Vector2Int GetChunkFromPosition(Vector2Int pos) {
        Vector2Int cPos = new Vector2Int(Mathf.FloorToInt(pos.x / (float)chunkSize), Mathf.FloorToInt(pos.y / (float)chunkSize));
        return cPos;
    }

    private void OnValidate()
    {
        biomes = biomesT;
    }

    struct ChunkThreadInfo {
        public readonly Action<Chunk> callback;
        public readonly Chunk param;

        public ChunkThreadInfo(Action<Chunk> _callback, Chunk _param) {
            callback = _callback;
            param = _param;
        }
    }
}

[System.Serializable]
public struct Biome
{
    public string name;
    public HeightZone height;
    public float maxRainfall;
    public Sprite[] vegetation;
}