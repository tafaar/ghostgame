using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateScript : MonoBehaviour
{
    [Header("Direction Game Objects")]

    [SerializeField]
    GameObject downCheck;
    [SerializeField]
    GameObject leftCheck;
    [SerializeField]
    GameObject rightCheck;
    [SerializeField]
    GameObject upCheck;

    [SerializeField]
    GameObject playerPrefab;


    public GameObject grid;

    [Header("Rooms")]

    public GameObject[] DLRU;

    public GameObject[] DLR;
    public GameObject[] LRU;
    public GameObject[] DLU;
    public GameObject[] DRU;

    public GameObject[] LR;
    public GameObject[] DU;

    public GameObject[] DR;
    public GameObject[] DL;
    public GameObject[] LU;
    public GameObject[] RU;

    public GameObject[] D;
    public GameObject[] L;
    public GameObject[] R;
    public GameObject[] U;

    string roomsNeeded;

    public enum RoomType { SPAWN, BOSS, ITEM, SHOP, DEFAULT }
    public RoomType roomType = RoomType.DEFAULT;

    void Start()
    {
        grid = FindObjectOfType<FloorGenerator>().gameObject;

        StartCoroutine(CheckDirections());
    }

    IEnumerator CheckDirections()
    {
        yield return new WaitForSeconds(0.2f);

        roomsNeeded += downCheck.GetComponent<SpawnPoint>().CheckRoom();
        roomsNeeded += leftCheck.GetComponent<SpawnPoint>().CheckRoom();
        roomsNeeded += rightCheck.GetComponent<SpawnPoint>().CheckRoom();
        roomsNeeded += upCheck.GetComponent<SpawnPoint>().CheckRoom();


        int randSelection = 0;

        yield return new WaitForSeconds(0.2f);

        GameObject newRoom = null;

        if(roomsNeeded == "DLRU")
        {
            randSelection = Random.Range(0, DLRU.Length - 1);
            newRoom = Instantiate(DLRU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }else if (roomsNeeded == "DLR")
        {
            randSelection = Random.Range(0, DLR.Length - 1);
            newRoom = Instantiate(DLR[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "LRU")
        {
            randSelection = Random.Range(0, LRU.Length - 1);
            newRoom = Instantiate(LRU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "DLU")
        {
            randSelection = Random.Range(0, DLU.Length - 1);
            newRoom = Instantiate(DLU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "DRU")
        {
            randSelection = Random.Range(0, DRU.Length - 1);
            newRoom = Instantiate(DRU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "LR")
        {
            randSelection = Random.Range(0, LR.Length - 1);
            newRoom = Instantiate(LR[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "DU")
        {
            randSelection = Random.Range(0, DU.Length - 1);
            newRoom = Instantiate(DU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "DR")
        {
            randSelection = Random.Range(0, DR.Length - 1);
            newRoom = Instantiate(DR[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "DL")
        {
            randSelection = Random.Range(0, DL.Length - 1);
            newRoom = Instantiate(DL[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "LU")
        {
            randSelection = Random.Range(0, LU.Length - 1);
            newRoom = Instantiate(LU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "RU")
        {
            randSelection = Random.Range(0, RU.Length - 1);
            newRoom = Instantiate(RU[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "D")
        {
            randSelection = Random.Range(0, D.Length - 1);
            newRoom = Instantiate(D[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "L")
        {
            randSelection = Random.Range(0, L.Length - 1);
            newRoom = Instantiate(L[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "R")
        {
            randSelection = Random.Range(0, D.Length - 1);
            newRoom = Instantiate(R[randSelection], transform.position, Quaternion.identity, grid.transform);
        }
        else if (roomsNeeded == "U")
        {
            randSelection = Random.Range(0, U.Length - 1);
            newRoom = Instantiate(U[randSelection], transform.position, Quaternion.identity, grid.transform);
        }

        newRoom.GetComponent<Room>().doorsNeeded = roomsNeeded;

        if(roomType != RoomType.DEFAULT)
        {
            Destroy(newRoom.GetComponent<Room>().enemyContents);
            Destroy(newRoom.GetComponent<Room>().persistentContents);
            Destroy(newRoom.GetComponent<Room>().nonPersistentContents);
        }

        if (roomType == RoomType.SPAWN)
        {
            newRoom.GetComponent<Room>().roomType = Room.RoomType.SPAWN;
            FloorGenerator.instance.spawnRoom = newRoom;

        }

        if (roomType == RoomType.BOSS)
        {
            newRoom.GetComponent<Room>().roomType = Room.RoomType.BOSS;
            newRoom.GetComponent<Room>().SetBoss();
        }

        if (roomType == RoomType.ITEM)
        {
            newRoom.GetComponent<Room>().roomType = Room.RoomType.ITEM;
            newRoom.GetComponent<Room>().SetItemRoom();
        }

        if (roomType == RoomType.SHOP)
        {
            newRoom.GetComponent<Room>().roomType = Room.RoomType.SHOP;
            newRoom.GetComponent<Room>().SetShop();
        }

        yield return new WaitForSeconds(1f);

        if (roomType == RoomType.SPAWN)
        {
            PlayerMove.instance.transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}
