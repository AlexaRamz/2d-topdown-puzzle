using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Button : MonoBehaviour
{
    PuzzleLogic puzzleMng;
    Vector2Int buttonPos;
    public Vector2Int rotatePos; // tile cell position of rotatable wire

    void Start()
    {
        puzzleMng = GameObject.Find("PuzzleManager").GetComponent<PuzzleLogic>();
        buttonPos = (Vector2Int)puzzleMng.tilemap.WorldToCell(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("return"))
        {
            if (puzzleMng.PlayerIsOn(buttonPos))
            {
                puzzleMng.RotateTileAt(rotatePos);
            }
        }
    }
}
