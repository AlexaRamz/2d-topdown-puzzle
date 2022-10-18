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
        Rotatable
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
    public bool on;
    public Vector2Int gridPos;
}
