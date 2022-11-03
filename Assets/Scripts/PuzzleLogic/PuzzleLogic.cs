using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleLogic : MonoBehaviour
{
    public Vector2Int bottomLeftCorner;
    int height = 20;
    int width = 20;
    TileState[,] puzzleArray;
    List<TileState> sources = new List<TileState>();
    public Tilemap tilemap;
    GridLayout gridLayout;

    public PuzzleTile source;
    public PuzzleTile light;
    public PuzzleTile[] wires = new PuzzleTile[6];
    public PuzzleTile[] rotatableWires = new PuzzleTile[6]; // rotatable
    // doors
    public PuzzleTile horizontalDoor;
    public PuzzleTile verticalDoor;

    public RotateTile lineRotation;
    public RotateTile curveRotation;

    [System.Serializable]
    public class ButtonToRotate
    {
        public Vector2Int buttonPos;
        public Vector2Int rotatePos;
    }
    public List<ButtonToRotate> buttons = new List<ButtonToRotate>();

    Transform plr;

    void Start()
    {
        plr = GameObject.Find("Player").transform;
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
                Vector2Int arrayPos = new Vector2Int(x, y);
                Vector2Int cellPos = GetCellPos(arrayPos);
                TileBase thisTile = tilemap.GetTile(new Vector3Int(cellPos.x, cellPos.y, 0));
                if (thisTile != null)
                {
                    PuzzleTile tile = VariableFromName(thisTile.name);
                    if (tile != null)
                    {
                        Debug.Log(tile.name);
                        TileState newState = new TileState { puzzleTile = tile };
                        if (tile.tileType == PuzzleTile.TileType.Source)
                        {
                            newState.on = true;
                            sources.Add(newState);
                        }
                        else if (tile.tileType == PuzzleTile.TileType.LineRotatable)
                        {
                            newState.SetRotations(lineRotation);
                        }
                        else if (tile.tileType == PuzzleTile.TileType.CurveRotatable)
                        {
                            newState.SetRotations(curveRotation);
                        }
                        newState.gridPos = arrayPos;
                        puzzleArray[arrayPos.x, arrayPos.y] = newState;
                    }
                }
            }
        }
        SetPower();
    }
    PuzzleTile VariableFromName(string name)
    {
        if (name == "Source") return source;
        if (name == "Horizontal") return wires[0];
        if (name == "Vertical") return wires[1];
        if (name == "UpRight") return wires[2];
        if (name == "RightDown") return wires[3];
        if (name == "DownLeft") return wires[4];
        if (name == "LeftUp") return wires[5];
        if (name == "RHorizontal") return rotatableWires[0];
        if (name == "RVertical") return rotatableWires[1];
        if (name == "RUpRight") return rotatableWires[2];
        if (name == "RRightDown") return rotatableWires[3];
        if (name == "RDownLeft") return rotatableWires[4];
        if (name == "RLeftUp") return rotatableWires[5];
        if (name == "walldoors_3") return horizontalDoor;
        if (name == "walldoors_1") return verticalDoor;
        if (name == "Light") return light;
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
                    Vector2Int cellPos = GetCellPos(new Vector2Int(x, y));
                    if (tileState.puzzleTile.tileType == PuzzleTile.TileType.Wire || tileState.puzzleTile.tileType == PuzzleTile.TileType.Light)
                    {
                        if (tileState.on)
                        {
                            tilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y, 0), tileState.puzzleTile.onSprite);
                            Debug.Log("on");
                        }
                        else
                        {
                            tilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y, 0), tileState.puzzleTile.offSprite);
                            Debug.Log("off");
                        }
                    }
                    else
                    {
                        tilemap.SetTile(new Vector3Int(cellPos.x, cellPos.y, 0), tileState.puzzleTile.onSprite);
                    }
                }
            }
        }
    }
    Vector2Int GetArrayPos(Vector2Int cellPos)
    {
        return new Vector2Int(cellPos.x - bottomLeftCorner.x, cellPos.y - bottomLeftCorner.y);
    }
    Vector2Int GetCellPos(Vector2Int arrayPos)
    {
        return new Vector2Int(arrayPos.x + bottomLeftCorner.x, arrayPos.y + bottomLeftCorner.y);
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
    void UpdateAdjacentTiles(TileState thisTile, int avoidPoint = -1)
    {
        // Given a tile, recursively update the adjacent tiles depending on its state
        Vector2Int arrayPos = thisTile.gridPos;
        List<TileState> adjacentTiles = new List<TileState> { GetAbove(arrayPos), GetRight(arrayPos), GetBelow(arrayPos), GetLeft(arrayPos) }; // Get the four adjacent tiles
        int i = 0; // Ranges from 0 to 3, determines the direction (up, right, down, left in that order)
        foreach (TileState tile in adjacentTiles)
        {
            Debug.Log(tile == null);
            // Make sure the adjacent tile is a wire or rotatable wire and is connected at one of the endpoints of the original tile
            int point = GetConnectingPoint(i);
            if (tile != null && tile.puzzleTile.isEndPoint[point] && thisTile.puzzleTile.isEndPoint[i] == true && tile.puzzleTile.tileType != PuzzleTile.TileType.Source && i != avoidPoint)
            {
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
            TileState tile = GetTileFromArray(GetArrayPos(pos));
            if (tile != null && (tile.puzzleTile.tileType == PuzzleTile.TileType.LineRotatable || tile.puzzleTile.tileType == PuzzleTile.TileType.CurveRotatable))
            {
                tile.AdvanceRotation();
                SetPower();
            }
        }
        if (Input.GetKeyDown("return"))
        {
            Vector3Int cellPos = gridLayout.WorldToCell(plr.position);
            Vector2Int pos = new Vector2Int(cellPos.x, cellPos.y);
            Debug.Log(pos);
            foreach (ButtonToRotate item in buttons)
            {
                if (item.buttonPos == pos)
                {
                    GetTileFromArray(GetArrayPos(item.rotatePos)).AdvanceRotation();
                    SetPower();
                }
            }
        }
    }
}
