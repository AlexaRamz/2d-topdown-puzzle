using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleObject : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    public Sprite onSprite;
    public Sprite offSprite;
    public Sprite brokenSprite;
    public bool broken = false;
    Vector2Int cellPos;
    PuzzleLogic puzzleMng;

    void Start()
    {
        puzzleMng = GameObject.Find("PuzzleManager").GetComponent<PuzzleLogic>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cellPos = (Vector2Int)puzzleMng.tilemap.WorldToCell(transform.position);
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
    public void BrokenSprite()
    {
        spriteRenderer.sprite = brokenSprite;
    }
    void Update()
    {
        if (broken && Input.GetKeyDown("return")) // Fix when near
        {
            if (puzzleMng.PlayerIsNear(cellPos))
            {
                broken = false;
                puzzleMng.UpdateTileSprites();
            }
        }
    }
}
