using UnityEngine;
using System.Collections;

public class HidingSpot : MonoBehaviour
{
    public bool isOccupied { get; set; }

    void Awake()
    {
        isOccupied = false;
    }
}
