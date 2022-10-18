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
    public Tilemap tilemap;

    public PuzzleTile source;
    public PuzzleTile horizontal;
    public PuzzleTile vertical;
    public PuzzleTile upRight;
    public PuzzleTile rightDown;
    public PuzzleTile downLeft;
    public PuzzleTile leftUp;

    void Start()
    {
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
                if (thisTile != null)
                {
                    PuzzleTile tile = VariableFromName(thisTile.name);
                    TileState newState = new TileState { puzzleTile = tile };
                    if (tile.tileType == PuzzleTile.TileType.Source)
                    {
                        newState.on = true;
                        sources.Add(newState);
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
                    for (int j = 0; j < tile.isPowered.Length; j++)
                    {
                        if (tile.puzzleTile.isEndPoint[j] && tile.isPowered[j])
                        {
                            tile.on = true;
                            break;
                        }
                    }
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
    void SetPower()
    {
        // Begin at each power source tile and recursively update the adjacent tiles
        foreach (TileState source in sources)
        {
            UpdateAdjacentTiles(source);
        }
        UpdateTileSprites();
    }
}
