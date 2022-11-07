using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeslaCoil : MonoBehaviour
{
    private Tilemap puzzleMap;
    private SpriteRenderer spriteRenderer;
    public bool powered;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Tile[] poweredTiles;
    [SerializeField] private GameObject[] electricityZones;

    // Start is called before the first frame update
    void Start()
    {
        puzzleMap = GameObject.FindGameObjectWithTag("PuzzleTilemap").GetComponent<Tilemap>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (poweredTiles.Contains(puzzleMap.GetTile(puzzleMap.WorldToCell(transform.position))))
        {
            powered = true;
            spriteRenderer.sprite = onSprite;
            foreach (GameObject zone in electricityZones)
            {
                zone.SetActive(true);
            }
        }
        else
        {
            powered = false;
            spriteRenderer.sprite = offSprite;
            foreach (GameObject zone in electricityZones)
            {
                zone.SetActive(false);
            }
        }
    }
}
