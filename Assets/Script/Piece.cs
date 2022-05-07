using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }
    public int RotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.Data = data;
        this.Board = board;
        this.Position = position;
        this.RotationIndex = 0;
        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        if (this.Cells == null)
            this.Cells = new Vector3Int[data.cells.Length];

        for (int i = 0; i < Cells.Length; i++)
            this.Cells[i] = (Vector3Int)data.cells[i];
    }

    private void Update()
    {
        this.Board.Clear(this);
        this.lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            Rotate(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if (Time.time >= this.stepTime)
            Step();


        this.Board.Set(this);
    }
    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay;

        Move(Vector2Int.down);

        if (this.lockTime >= this.lockDelay)
            Lock();
    }

    private void Lock()
    {
        this.Board.Set(this);
        this.Board.ClearLines();
        this.Board.SpawnPiece();
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
            continue;
        Lock();
    }
    private bool Move(Vector2Int translation) ///https://tetris.fandom.com/wiki/SRS
    {
        Vector3Int newPosition = this.Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.Board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.Position = newPosition;
            this.lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction) /// 0 , 1 , 2 , 3
    {
        int originalRotation = this.RotationIndex;
        this.RotationIndex = Wrap(this.RotationIndex + direction, 0, 4);

        RotationMatrix(direction);

        if (!TestWallKicks(this.RotationIndex, direction))
        {
            this.RotationIndex = originalRotation;
            RotationMatrix(-direction);
        }

    }

    private void RotationMatrix(int direction)
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3 cell = this.Cells[i];

            int x, y;

            switch (this.Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));

                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[0] * direction) + (cell.y * global::Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * global::Data.RotationMatrix[2] * direction) + (cell.y * global::Data.RotationMatrix[3] * direction));
                    break;
            }

            this.Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.Data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.Data.wallKicks[wallKickIndex, i];

            if (Move(translation))
                return true;
        }
        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationDirection * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }
        return Wrap(wallKickIndex, 0, this.Data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}
