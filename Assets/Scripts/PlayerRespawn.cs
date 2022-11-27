using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public LevelLoader loader;

    void OnEnable()
    {
        loader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && loader != null)
        {
            loader.ReloadLevel();
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Electricity")
        {
            GetComponent<AudioSource>().Play();
            loader.ReloadLevel();
        }
    }
}
