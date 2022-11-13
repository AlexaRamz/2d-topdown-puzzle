using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleObject : MonoBehaviour
{
    Tilemap puzzleMap;
    SpriteRenderer spriteRenderer;
    public Sprite onSprite;
    public Sprite offSprite;
    Vector2Int cellPos;

    void Start()
    {
        puzzleMap = transform.parent.gameObject.GetComponent<Tilemap>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cellPos = (Vector2Int)puzzleMap.WorldToCell(transform.position);
    }
    public Vector2Int GetPos()
    {
        return cellPos;
    }
    public void OnSprite()
    {
        spriteRenderer.sprite = onSprite;
        if (GetComponent<TeslaCoil>())
        {
            GetComponent<TeslaCoil>().ZonesOn();
        }
    }
    public void OffSprite()
    {
        spriteRenderer.sprite = offSprite;
        if (GetComponent<TeslaCoil>())
        {
            GetComponent<TeslaCoil>().ZonesOff();
        }
    }
}
