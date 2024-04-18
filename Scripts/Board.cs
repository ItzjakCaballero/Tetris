using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Tilemap tilemap { get; private set; }
    public Peice activePeice { get; private set; }
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    
    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePeice = GetComponentInChildren<Peice>();
        for(int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];
        this.activePeice.Initialize(this, this.spawnPosition, data);
        if(isValidPosition(this.activePeice, this.spawnPosition))
        {
            Set(this.activePeice);
        } else
        {
            GameOver();
        }
        Set(this.activePeice);
    }
    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
    }

    public void Set(Peice peice)
    {
        for(int i = 0; i < peice.cells.Length; i++)
        {
            Vector3Int tilePosition = peice.cells[i] + peice.position;
            this.tilemap.SetTile(tilePosition, peice.data.tile);
        }
    }

    public void Clear(Peice peice)
    {
        for (int i = 0; i < peice.cells.Length; i++)
        {
            Vector3Int tilePosition = peice.cells[i] + peice.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool isValidPosition(Peice peice, Vector3Int position)
    {
        RectInt bounds = this.Bounds;
        for(int i = 0; i < peice.cells.Length; i++)
        {
            Vector3Int tilePosition = peice.cells[i] + position;
            if(!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if(this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;
        while(row < bounds.yMax)
        {
            if(isLineFull(row))
            {
                LineClear(row);
            } else
            {
                row++;
            }
        }
    }

    private bool isLineFull(int row)
    {
        RectInt bounds = this.Bounds;
        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if(!this.tilemap.HasTile(position))
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;
        for(int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.tilemap.SetTile(position, null);
        }
        while(row < bounds.yMax)
        {
            for(int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.tilemap.GetTile(position);
                position = new Vector3Int(col, row, 0);
                this.tilemap.SetTile(position, above);
            }
            row++;
        }
    }
}
