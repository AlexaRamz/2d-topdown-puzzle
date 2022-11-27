using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeslaCoil : MonoBehaviour
{
    [SerializeField] private GameObject[] electricityZones;
    bool on = false;

    public void ZonesOn()
    {
        if (!on)
        {
            transform.Find("OnAudio").GetComponent<AudioSource>().Play();
            foreach (GameObject zone in electricityZones)
            {
                zone.SetActive(true);
            }
        }
        on = true;
    }
    public void ZonesOff()
    {
        if (on)
        {
            transform.Find("OffAudio").GetComponent<AudioSource>().Play();
            foreach (GameObject zone in electricityZones)
            {
                zone.SetActive(false);
            }
        }
        on = false;
    }
}
