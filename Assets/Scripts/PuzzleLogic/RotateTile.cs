using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "new RotateTile", menuName = "ScriptableObjects/RotateTile", order = 1)]
public class RotateTile : ScriptableObject
{
    public List<PuzzleTile> tiles = new List<PuzzleTile>();
}