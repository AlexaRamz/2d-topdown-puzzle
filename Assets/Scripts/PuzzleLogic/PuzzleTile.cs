using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "new PuzzleTile", menuName = "ScriptableObjects/PuzzleTile", order = 1)]
public class PuzzleTile : ScriptableObject
{
    public enum TileType {
        Wire,
        Source,
        LineRotatable,
        CurveRotatable,
        Light,
        Other,
    }

    public TileType tileType;
    //Up, Right, Down, Left
    public bool[] isEndPoint = new bool[4];

    public TileBase onSprite;
    public TileBase offSprite;
}
public class TileState
{
    public PuzzleTile puzzleTile;

    public bool[] isPowered = new bool[4];
    public bool on = false;
    public Vector2Int gridPos;

    public void UpdatePower()
    {
        for (int i = 0; i < 4; i++)
        {
            if (puzzleTile.isEndPoint[i] && isPowered[i])
            {
                on = true;
                break;
            }
        }
    }
    public void ClearPower()
    {
        on = false;
        for (int i = 0; i < 4; i++)
        {
            isPowered[i] = false;
        }
    }

    RotateTile rotateTile;
    int rotIndex = 0;
    public void AdvanceRotation()
    {
        rotIndex += 1;
        if (rotIndex >= rotateTile.tiles.Count)
        {
            rotIndex = 0;
        }
        SetRotation(rotIndex);
    }
    public void SetRotation(int index)
    {
        rotIndex = index;
        puzzleTile = rotateTile.tiles[rotIndex];
    }
    public void SetRotations(RotateTile rotations)
    {
        rotateTile = rotations;
        int index = rotateTile.GetIndex(puzzleTile);
        if (index != -1)
        {
            rotIndex = index;
        }
    }
}
