using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class HidingSpot : NetworkBehaviour
{
    [SyncVar]
    public bool isOccupied;

    void Awake()
    {
        isOccupied = false;
    }


    
    public void SetOccupiedStatus(bool _isOccupied)
    {
        isOccupied = _isOccupied;
    }
}
