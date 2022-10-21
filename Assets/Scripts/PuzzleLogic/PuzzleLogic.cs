using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleLogic : MonoBehaviour
{
    int height = 10;
    int width = 10;
    TileState[,] puzzleArray;
    List<TileState> sources = new List<TileState>();
    List<TileState> rotatables = new List<TileState>();
    public Tilemap tilemap;
    GridLayout gridLayout;

    public PuzzleTile source;
    public PuzzleTile horizontal;
    public PuzzleTile vertical;
    public PuzzleTile upRight;
    public PuzzleTile rightDown;
    public PuzzleTile downLeft;
    public PuzzleTile leftUp;
    //rotatable
    public PuzzleTile rHorizontal;
    public PuzzleTile rVertical;
    public PuzzleTile rUpRight;
    public PuzzleTile rRightDown;
    public PuzzleTile rDownLeft;
    public PuzzleTile rLeftUp;
    //doors
    public PuzzleTile horizontalDoor;
    public PuzzleTile verticalDoor;


    public RotateTile lineRotation;
    public RotateTile curveRotation;

    void Start()
    {
        gridLayout = tilemap.transform.parent.GetComponent<GridLayout>();
        puzzleArray = new TileState[height, width];
        GetPuzzleTiles();
    }
    void GetPuzzleTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileBase thisTile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (thisTile != null && thisTile.name != "Wall")
                {
                    PuzzleTile tile = VariableFromName(thisTile.name);
                    TileState newState = new TileState { puzzleTile = tile };
                    if (tile.tileType == PuzzleTile.TileType.Source)
                    {
                        newState.on = true;
                        sources.Add(newState);
                    }
                    else if (tile.tileType == PuzzleTile.TileType.Rotatable)
                    {
                        newState.rotateTile = curveRotation;
                        newState.SetRotation(0);
                        rotatables.Add(newState);
                    }
                    newState.gridPos = new Vector2Int(x, y);
                    puzzleArray[x, y] = newState;
                    Debug.Log("tile");
                }
                Debug.Log("any");
            }
        }
        SetPower();
    }
    PuzzleTile VariableFromName(string name)
    {
        if (name == "Source") return source;
        if (name == "Horizontal") return horizontal;
        if (name == "Vertical") return vertical;
        if (name == "UpRight") return upRight;
        if (name == "RightDown") return rightDown;
        if (name == "DownLeft") return downLeft;
        if (name == "LeftUp") return leftUp;
        if (name == "RHorizontal") return rHorizontal;
        if (name == "RVertical") return rVertical;
        if (name == "RUpRight") return rUpRight;
        if (name == "RRightDown") return rRightDown;
        if (name == "RDownLeft") return rDownLeft;
        if (name == "RLeftUp") return rLeftUp;
        if (name == "walldoors_3") return horizontalDoor;
        if (name == "walldoors_1") return verticalDoor;
        return null;
    }
    void UpdateTileSprites()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileState tileState = puzzleArray[x, y];
                if (tileState != null)
                {
                    if (tileState.puzzleTile.tileType == PuzzleTile.TileType.Wire)
                    {
                        if (tileState.on)
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), tileState.puzzleTile.onSprite);
                            Debug.Log("on");
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(x, y, 0), tileState.puzzleTile.offSprite);
                            Debug.Log("off");
                        }
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(x, y, 0), tileState.puzzleTile.onSprite);
                    }
                }
            }
        }
    }
    Vector2Int GetArrayPos(Vector2Int worldPos)
    {
        return worldPos;
    }
    bool CheckArrayPos(Vector2Int arrayPos)
    {
        return arrayPos.x >= 0 && arrayPos.x < width && arrayPos.y >= 0 && arrayPos.y < height;
    }

    TileState GetTileFromArray(Vector2Int arrayPos)
    {
        if (CheckArrayPos(arrayPos))
        {
            return puzzleArray[arrayPos.x, arrayPos.y];
        }
        return null;
    }

    // Functions for getting the adjacent tiles
    TileState GetAbove(Vector2Int arrayPos)
    {
        Vector2Int newPos = new Vector2Int(arrayPos.x, arrayPos.y + 1);
        return GetTileFromArray(newPos);
    }
    TileState GetRight(Vector2Int arrayPos)
    {
        Vector2Int newPos = new Vector2Int(arrayPos.x + 1, arrayPos.y);
        return GetTileFromArray(newPos);
    }
    TileState GetBelow(Vector2Int arrayPos)
    {
        Vector2Int newPos = new Vector2Int(arrayPos.x, arrayPos.y - 1);
        return GetTileFromArray(newPos);
    }
    TileState GetLeft(Vector2Int arrayPos)
    {
        Vector2Int newPos = new Vector2Int(arrayPos.x - 1, arrayPos.y);
        return GetTileFromArray(newPos);
    }

    int GetConnectingPoint(int i)
    {
        // Get the connecting endpoint for the adjacent tile. Right gives left, up gives down, and so on.
        int point = i + 2;
        if (point == 4) point = 0;
        if (point == 5) point = 1;
        return point;
    }
    int tries = 0;
    void UpdateAdjacentTiles(TileState thisTile, int avoidPoint = -1)
    {
        // Given a tile, recursively update the adjacent tiles depending on its state
        if (tries < 10)
        {
            Vector2Int arrayPos = thisTile.gridPos;
            List<TileState> adjacentTiles = new List<TileState> { GetAbove(arrayPos), GetRight(arrayPos), GetBelow(arrayPos), GetLeft(arrayPos) }; // Get the four adjacent tiles
            int i = 0; // Ranges from 0 to 3, determines the direction (up, right, down, left in that order)
            foreach (TileState tile in adjacentTiles)
            {
                Debug.Log("1");
                Debug.Log(tile == null);
                // Make sure the adjacent tile is a wire or rotatable wire and is connected at one of the endpoints of the original tile
                if (tile != null && thisTile.puzzleTile.isEndPoint[i] == true && tile.puzzleTile.tileType != PuzzleTile.TileType.Source && i != avoidPoint)
                {
                    int point = GetConnectingPoint(i);
                    tile.isPowered[point] = thisTile.on;
                    bool originalState = tile.on;
                    tile.on = false;

                    // Set wire tile on or off depending on whether any endpoints are powered
                    tile.UpdatePower();
                    // If wire on/off state was changed, update the next adjacent tiles not including the original tile
                    if (originalState != tile.on)
                    {
                        UpdateAdjacentTiles(tile, point);
                    }
                }
                i++;
            }
        }
    }
    void ClearPower()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileState tileState = puzzleArray[x, y];
                if (tileState != null && tileState.puzzleTile.tileType != PuzzleTile.TileType.Source)
                {
                    tileState.ClearPower();
                }
            }
        }
    }
    void SetPower()
    {
        // Begin at each power source tile and recursively update the adjacent tiles
        ClearPower();
        foreach (TileState tile in sources)
        {
            UpdateAdjacentTiles(tile);
        }
        UpdateTileSprites();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int cellPos = gridLayout.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            Vector2Int pos = new Vector2Int(cellPos.x, cellPos.y);
            TileState tile = GetTileFromArray(pos);
            if (tile != null && tile.puzzleTile.tileType == PuzzleTile.TileType.Rotatable)
            {
                tile.AdvanceRotation();
                SetPower();
            }
        }
    }
}
