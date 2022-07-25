using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public static RoomTemplates instance;

    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    [Header("References for rooms")]
    public GameObject pickupPrefab;

    public int roomCap = 50;
    
    private void Awake()
    {
        instance = this;
    }
}
