using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeslaCoil : MonoBehaviour
{
    [SerializeField] private GameObject[] electricityZones;

    public void ZonesOn()
    {
        foreach (GameObject zone in electricityZones)
        {
            zone.SetActive(true);
        }
    }
    public void ZonesOff()
    {
        foreach (GameObject zone in electricityZones)
        {
            zone.SetActive(false);
        }
    }
}
