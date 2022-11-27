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

    public List<PuzzleTile> wires = new List<PuzzleTile>();
    public List<PuzzleTile> rotatableWires = new List<PuzzleTile>();
    // doors
    public PuzzleTile horizontalDoor;
    public PuzzleTile verticalDoor;
    public PuzzleTile source;
    // rotatable tile collections
    public RotateTile lineRotation;
    public RotateTile curveRotation;
    public RotateTile TRotation;

    Transform plr;
    public Canvas winCanvas;
    List<PuzzleObject> lights = new List<PuzzleObject>();

    void Start()
    {
        puzzleArray = new TileState[height, width];
        plr = GameObject.Find("Player").transform;
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
                    PuzzleTile tile = GetPuzzleTile(thisTile, tilemap.GetTransformMatrix(new Vector3Int(cellPos.x, cellPos.y, 0)).rotation.eulerAngles);
                    if (tile != null)
                    {
                        TileState newState = new TileState { puzzleTile = tile };
                        if (tile.tileType == PuzzleTile.TileType.LineRotatable)
                        {
                            newState.SetRotations(lineRotation);
                        }
                        else if (tile.tileType == PuzzleTile.TileType.CurveRotatable)
                        {
                            newState.SetRotations(curveRotation);
                        }
                        else if (tile.tileType == PuzzleTile.TileType.TRotatable)
                        {
                            newState.SetRotations(TRotation);
                        }
                        newState.gridPos = arrayPos;
                        puzzleArray[arrayPos.x, arrayPos.y] = newState;
                    }
                }
            }
        }
        foreach (Transform obj in tilemap.transform)
        {
            if (obj.GetComponent<PuzzleObject>() != null)
            {
                PuzzleObject puzzleObj = obj.GetComponent<PuzzleObject>();
                if (puzzleObj.source)
                {
                    TileState newState = new TileState { puzzleTile = source };
                    Vector2Int arrayPos = GetArrayPos((Vector2Int)tilemap.WorldToCell(puzzleObj.transform.position));
                    if (!puzzleObj.broken)
                    {
                        newState.on = true;
                    }
                    else
                    {
                        newState.on = false;
                    }
                    sources.Add(newState);
                    newState.gridPos = arrayPos;
                    puzzleArray[arrayPos.x, arrayPos.y] = newState;
                }
                else if (puzzleObj.lightbulb)
                {
                    lights.Add(puzzleObj);
                }
            }
        }
        SetPower();
    }
    PuzzleTile GetPuzzleTile(TileBase tile, Vector3 rotation)
    {
        if (horizontalDoor.onTile == tile && horizontalDoor.rotation == rotation)
        {
            return horizontalDoor;
        }
        else if (verticalDoor.onTile == tile && verticalDoor.rotation == rotation)
        {
            return verticalDoor;
        }
        foreach (PuzzleTile puzzleTile in wires)
        {
            if (puzzleTile.onTile == tile && puzzleTile.rotation == rotation)
            {
                return puzzleTile;
            }
        }
        foreach (PuzzleTile puzzleTile in rotatableWires)
        {
            if (puzzleTile.onTile == tile && puzzleTile.rotation == rotation)
            {
                return puzzleTile;
            }
        }
        return null;
    }
    public void UpdateTileSprites()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                TileState tileState = puzzleArray[x, y];
                if (tileState != null)
                {
                    Vector3Int cellPos = (Vector3Int)GetCellPos(new Vector2Int(x, y));
                    if (tileState.puzzleTile.tileType == PuzzleTile.TileType.Wire)
                    {
                        if (tileState.on)
                        {
                            Vector3 rot = tileState.puzzleTile.rotation;
                            tilemap.SetTile(cellPos, tileState.puzzleTile.onTile);
                            tilemap.SetTransformMatrix(cellPos, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rot.x, rot.y, rot.z), Vector3.one));
                        }
                        else
                        {
                            Vector3 rot = tileState.puzzleTile.rotation;
                            tilemap.SetTile(cellPos, tileState.puzzleTile.offTile);
                            tilemap.SetTransformMatrix(cellPos, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rot.x, rot.y, rot.z), Vector3.one));
                        }
                    }
                    else
                    {
                        Vector3 rot = tileState.puzzleTile.rotation;
                        tilemap.SetTile(cellPos, tileState.puzzleTile.onTile);
                        tilemap.SetTransformMatrix(cellPos, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rot.x, rot.y, rot.z), Vector3.one));
                    }
                }
            }
        }
        foreach (Transform obj in tilemap.transform)
        {
            if (obj.GetComponent<PuzzleObject>() != null)
            {
                PuzzleObject puzzleObj = obj.GetComponent<PuzzleObject>();
                if (puzzleObj.broken)
                {
                    puzzleObj.BrokenSprite();
                }
                else if (IsPowered(puzzleObj.GetPos()))
                {
                    puzzleObj.OnSprite();
                }
                else
                {
                    puzzleObj.OffSprite();
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
    public void RotateTileAt(Vector2Int cellPos)
    {
        TileState tile = GetTileFromArray(GetArrayPos(cellPos));
        if (tile != null && tile.IsRotatable())
        {
            tile.AdvanceRotation();
            SetPower();
        }
    }
    public void FixSourceAt(Vector2Int cellPos)
    {
        TileState tile = GetTileFromArray(GetArrayPos(cellPos));
        if (tile != null && tile.puzzleTile.tileType == PuzzleTile.TileType.Source)
        {
            tile.on = true;
            SetPower();
            Debug.Log("fix");
        }
    }
    public bool PlayerIsOn(Vector2Int cellPos)
    {
        Vector2Int plrPos = (Vector2Int)tilemap.WorldToCell(plr.position);
        return plrPos == cellPos;
    }
    public bool PlayerIsNear(Vector2Int cellPos)
    {
        Vector2Int plrPos = (Vector2Int)tilemap.WorldToCell(plr.position);
        return Vector2.Distance(plrPos, cellPos) == 1;
    }
    public bool IsPowered(Vector2Int cellPos)
    {
        TileState tile = GetTileFromArray(GetArrayPos(cellPos));
        Debug.Log(cellPos);
        return tile != null && tile.on;
    }
    public IEnumerator WinCanvas()
    {
        yield return new WaitForSeconds(1);
        ThisCanvas();
    }
    void ThisCanvas()
    {
        foreach (PuzzleObject tile in lights)
        {
            if (!tile.lightOn || tile.broken)
            {
                return;
            }
        }
        winCanvas.enabled = true;
    }
    void SetPower()
    {
        // Begin at each power source tile and recursively update the adjacent tiles
        ClearPower();
        foreach (TileState tile in sources)
        {
            if (tile.on)
            {
                UpdateAdjacentTiles(tile);
            }
        }
        UpdateTileSprites();
        StartCoroutine(WinCanvas());
    }
    void Update()
    {
        //if (Input.GetMouseButtonDown(0)) // Rotate on click
        //{
        //    Vector2Int mousePos = (Vector2Int)tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //    RotateTileAt(mousePos);
        //}
    }
}
