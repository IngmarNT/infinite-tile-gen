using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Chunk
{
    public Vector2Int pos { get; private set; }
    private int size;

    public Tile[,] tiles;

    public float[,] height;
    public float[,] rainfall;

    public Chunk(Vector2Int _pos, int _size, float[,] _height, float[,] _rainfall) {
        pos = _pos;
        size = _size;
        tiles = new Tile[size, size];
        height = _height;
        rainfall = _rainfall;
    }

    public void set(Vector2Int pos, Tile tile) {
        tiles[pos.x, pos.y] = tile;
    }
}
