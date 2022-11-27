using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool on = false;
    void Start()
    {
        
    }
    public void Open()
    {
        if (!on)
        {
            transform.Find("OnAudio").GetComponent<AudioSource>().Play();
            GetComponent<BoxCollider2D>().enabled = false;
        }
        on = true;
    }
    public void Close()
    {
        if (on)
        {
            transform.Find("OffAudio").GetComponent<AudioSource>().Play();
            Debug.Log("off");
        }
        GetComponent<BoxCollider2D>().enabled = true;
        on = false;
    }
    void Update()
    {
        
    }
}
