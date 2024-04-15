using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TileBoard : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField]private  Tile tilePrefab;

    [SerializeField] private TileState[] tileStates;

    private TileGrid _grid;
    private List<Tile> _tiles;

    private bool waiting;

    private void Awake()
    {
        _grid = GetComponentInChildren<TileGrid>();
        _tiles = new List<Tile>(16);
    }

    private void Update()
    {
        if (!waiting)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveTiles(Vector2Int.up, 0, 1, 1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveTiles(Vector2Int.down, 0, 1, _grid.height - 2, -1);
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveTiles(Vector2Int.left, 1, 1, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveTiles(Vector2Int.right, _grid.width - 2, -1, 0, 1);
            }
        }
    }

    public void ClearBoard()
    {
        foreach(var cell in _grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in _tiles) 
        {
            Destroy(tile.gameObject);
        }

        _tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, _grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(_grid.GetRandomEmptyCell());
        _tiles.Add(tile);
    }

    private void MoveTiles(Vector2Int direction, int startX, int incrementX, int startY, int incrementY)
    {
        bool changed = false;
        for (int x = startX; x >= 0 && x < _grid.width; x += incrementX)
        {
            for (int y = startY; y >= 0 && y < _grid.height; y += incrementY)
            {
                TileCell cell = _grid.GetCell(x, y);

                if (cell.isTaken)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }
        if (changed)
        {
            WaitIsChange();
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacent = _grid.GetAdjacentCell(tile.cell, direction);

        while (adjacent != null)
        {
            if (adjacent.isTaken)
            {
                if (IsCanMerge(tile, adjacent.tile))
                {
                    Merge(tile, adjacent.tile);
                    return true;
                }

                break;
            }

            newCell = adjacent;
            adjacent = _grid.GetAdjacentCell(adjacent, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }

        return false;
    }

    private bool IsCanMerge(Tile a, Tile b)
    {
        return a.number == b.number && !b.locked;
    }

    private void Merge(Tile a, Tile b)
    {
        _tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int number = b.number * 2;

        b.SetState(tileStates[index], number);

        gameManager.IncreaseScore(number);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }
        return -1;
    }

    private async void WaitIsChange()
    {
        waiting = true;

        await Task.Delay(100);

        waiting = false;

        foreach(Tile tile in _tiles)
        {
            tile.locked = false;
        }

        if (_tiles.Count != _grid.size)
        {
            CreateTile();
        }
        
        if(IsGameOver())
        {
            gameManager.GameOver();
        }
    }

    private bool IsGameOver()
    {
        if(_tiles.Count != _grid.size)
        {
            return false;
        }

        foreach (var tile  in _tiles)
        {
            TileCell up = _grid.GetAdjacentCell(tile.cell, Vector2Int.up);
            TileCell down = _grid.GetAdjacentCell(tile.cell, Vector2Int.down);
            TileCell left = _grid.GetAdjacentCell(tile.cell, Vector2Int.left);
            TileCell right = _grid.GetAdjacentCell(tile.cell, Vector2Int.right);

            if (up != null && IsCanMerge(tile, up.tile))
            {
                return false;
            }

            if (down != null && IsCanMerge(tile, down.tile))
            {
                return false;
            }

            if (left != null && IsCanMerge(tile, left.tile))
            {
                return false;
            }

            if (right != null && IsCanMerge(tile, right.tile))
            {
                return false;
            }
        }

        return true;
    }
}
