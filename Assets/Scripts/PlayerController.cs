using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2Int cellPosition;
    public Vector2Int chunk;
    [SerializeField]
    static ChunkHandler chunkHandler;

    void Start()
    {
        chunkHandler = FindObjectOfType<ChunkHandler>();
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int newCellPosition = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        if (newCellPosition != cellPosition) {
            
        }

        Vector2Int newChunk = chunkHandler.GetChunkFromPosition(cellPosition);
        if (newChunk != chunk) {
            chunk = newChunk;
            UpdateChunks();
        }
    }

    public void UpdateChunks() {
        int rend = chunkHandler.renderDistance;
        for (int i = -rend; i <= rend; i++) {
            for (int j = -rend; j <= rend; j++) {
                if (Mathf.Abs(i) + Mathf.Abs(j) <= rend) {
                    chunkHandler.RequestChunk(chunkHandler.OnChunkDataRecieved, new Vector2Int(chunk.x - i, chunk.y - j));
                    Debug.Log("requested chunk data");
                }
            }
        }
    }
}
