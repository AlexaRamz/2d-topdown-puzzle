using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PuzzleObject : MonoBehaviour
{
    public Sprite onSprite;
    public Sprite offSprite;
    public Sprite brokenSprite;
    public bool broken = false;
    public bool source = false;
    public bool lightbulb = false;

    void Start()
    {

    }
    public Vector2Int GetPos()
    {
        return (Vector2Int)GameObject.Find("PuzzleManager").GetComponent<PuzzleLogic>().tilemap.WorldToCell(transform.position);
    }
    bool on = true;
    public bool lightOn = false;
    public void OnSprite()
    {
        GetComponent<SpriteRenderer>().sprite = onSprite;
        if (GetComponent<TeslaCoil>())
        {
            GetComponent<TeslaCoil>().ZonesOn();
        }
        if (!on && GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().Play();
        }
        if (GetComponent<Door>())
        {
            GetComponent<Door>().Open();
        }
        on = true;
        lightOn = true;
    }
    public void OffSprite()
    {
        if (offSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = offSprite;
        }
        if (GetComponent<TeslaCoil>())
        {
            GetComponent<TeslaCoil>().ZonesOff();
        }
        if (GetComponent<Door>())
        {
            GetComponent<Door>().Close();
        }
        on = false;
        lightOn = false;
    }
    public void BrokenSprite()
    {
        GetComponent<SpriteRenderer>().sprite = brokenSprite;
    }
    void Update()
    {
        if (broken && Input.GetKeyDown("return")) // Fix when near
        {
            PuzzleLogic puzzleMng = GameObject.Find("PuzzleManager").GetComponent<PuzzleLogic>();
            Vector2Int cellPos = (Vector2Int)puzzleMng.tilemap.WorldToCell(transform.position);
            if (puzzleMng.PlayerIsNear(cellPos))
            {
                broken = false;
                if (source)
                {
                    puzzleMng.FixSourceAt(cellPos);
                }
                else
                {
                    puzzleMng.UpdateTileSprites();
                }
                StartCoroutine(puzzleMng.WinCanvas());
            }
        }
    }
}
