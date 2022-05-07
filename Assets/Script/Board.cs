using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public TetrominoData[] tetrominos;
    public Tilemap Tilemap { get; private set; }
    public Piece ActivePiece { get; private set; }
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
        this.Tilemap = GetComponentInChildren<Tilemap>();
        this.ActivePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominos.Length; i++)
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
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        this.ActivePiece.Initialize(this, this.spawnPosition, data);

        if (IsValidPosition(this.ActivePiece, this.spawnPosition))
        {
            Set(this.ActivePiece);
        }
        else GameOver();
    }

    public void GameOver()
    {
        this.Tilemap.ClearAllTiles();
        Score.scoreValue = 0;
        SceneManager.LoadScene(2);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            Tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
                return false;

            if (this.Tilemap.HasTile(tilePosition))
                return false;

        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = this.Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else row++;
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!this.Tilemap.HasTile(position))
                return false;
        }

        return true;
    }

    private void LineClear(int row)
    {
        RectInt bounds = this.Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            this.Tilemap.SetTile(position, null);
        }

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = this.Tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                this.Tilemap.SetTile(position, above);
            }
            row++;
        }
        Score.scoreValue += 100;
    }
}
