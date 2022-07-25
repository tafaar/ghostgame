using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public class FloorGenerator : MonoBehaviour
{
    public static FloorGenerator instance;

    [Space(50)]
    [Header("Debug")]
    public GameObject marker;
    public GameObject roomArt;

    public GameObject realEndMarker;
    public GameObject possibleEndMarker;
    public GameObject emptyCellMarker;
    public GameObject newEndMarker;

    public bool debugMode = true;

    int roomCheck;
    int iterations = 0;

   

    public class Cell
    {
        public string neighbors;
        public int rank;
        public int row;
        public int column;
        public bool isRoom;
        public bool isDeadEnd;
        public int distanceToSpawn;

        public enum RoomType { SPAWN, BOSS, ITEM, SHOP, DEFAULT}
        public RoomType roomType;


        public Cell(string neighborCells, int arrayRank, int arrayRow, int arrayColumn, bool room = false,  bool deadEnd = false, int distanceSpawn = 0, RoomType type = RoomType.DEFAULT)
        {
            neighbors = neighborCells;
            rank = arrayRank;
            row = arrayRow;
            column = arrayColumn;
            isRoom = room;
            isDeadEnd = deadEnd;
            distanceToSpawn = distanceSpawn;
            roomType = type;
        }
    }

    [Space(50)]
    [Header("Generation Settings")]
    [SerializeField] GameObject grid;
    [SerializeField] GameObject roomTemplate;

    [Range(6, 50)]
    public int desiredRooms = 8;
    [Range(4, 10)]
    public int desiredDeadEnds = 4;

    [Space(50)]
    [Header("Other")]
    public bool spawnCreated;
    public bool bossCreated;
    public GameObject spawnRoom;
    bool print = false;

    public List<GameObject> generatedRooms;
    public List<GameObject> cleanUp;
    List<GameObject> rooms;
    List<GameObject> removeEnemies;

    [Space(50)]
    [Header("Prefabs")]
    public GameObject[] bosses;
    public GameObject[] shops;
    public GameObject[] itemrooms;
    public GameObject obscurer;
    public GameObject doorPrefab;
    public GameObject defaultBullet;

    int[,,] a = new int[2, 10, 10];

    List<Cell> cells;

    int[] spawnCoordinates = { 0, 0 };

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple floor generator instances were found");
            Destroy(gameObject);
        }

        instance = this;

        removeEnemies = new List<GameObject>();
        cleanUp = new List<GameObject>();
        generatedRooms = new List<GameObject>();
        rooms = new List<GameObject>();
    }

    private void Start()
    {
        cells = new();
        StartCoroutine(Initialize(a));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("CREATING NEW LAYOUT");
            cells.Clear();
            ResetFloor();
            
        }
    }

    IEnumerator Initialize(int[,,] a)
    {
        bool failedGeneration = false;

        a[1, Mathf.RoundToInt(a.GetLength(1) / 2), Mathf.RoundToInt(a.GetLength(2) / 2)] = 2;

        spawnCreated = false;

        roomCheck = desiredRooms;
        iterations = 0;

        Iterate(a);

        yield return new WaitForEndOfFrame();

        //Check neighbors

        CheckNeighbors(a);

        yield return new WaitForEndOfFrame();

        List<Cell> deadEndReal = new List<Cell>();

        foreach (Cell cell in cells)
        {
            if (cell.neighbors == "D" || cell.neighbors == "L" || cell.neighbors == "R" || cell.neighbors == "U")
            {
                cell.isDeadEnd = true;

                if (cell.isRoom)
                {
                    DebugRoom(cell, realEndMarker);
                    deadEndReal.Add(cell);
                    Debug.Log("FOUND DEAD END AT " + cell.row + ", " + cell.column);
                }
            }
        }

        if(deadEndReal.Count < desiredDeadEnds)
        {
            Debug.Log("Not enough deadends, adding");

            List<Cell> potentialDeadends = new();

            foreach (Cell cell in cells)
            {
                if(cell.isDeadEnd == true && cell.isRoom == false)
                {
                    DebugRoom(cell, possibleEndMarker);
                    potentialDeadends.Add(cell);
                }
            }

            Debug.Log("There are " + potentialDeadends.Count + " potential dead ends.");

            int attempts = 0;
            

            while (deadEndReal.Count < desiredDeadEnds)
            {
                attempts += 1;

                if(attempts >= 2)
                {
                    Debug.LogWarning("Unable to generate desired room at deadend generation");
                    failedGeneration = true;
                    break;
                }
                
                foreach (Cell potential in potentialDeadends)
                {
                    bool roomAdded = false;

                    if (potential.isRoom == false)
                    {

                        bool adjacentDE = false;

                        for (int i = 0; i < 2; i++)
                        {

                            if (deadEndReal.Count >= desiredDeadEnds || roomAdded) break;

                            if (adjacentDE == false)
                            {
                                //DOWN
                                if (potential.column + 1 <= a.GetLength(2) - 1)
                                {
                                    foreach (Cell cellCheck in cells)
                                    {
                                        if (cellCheck.column == potential.column + 1 && cellCheck.row == potential.row)
                                        {

                                            if (cellCheck.isDeadEnd && cellCheck.isRoom) adjacentDE = true;

                                            if (cellCheck.neighbors == "" && i == 1)
                                            {
                                                Debug.Log("CREATING NEW U ROOM");
                                                cellCheck.neighbors += "U";
                                                deadEndReal.Add(potential);
                                                a[0, potential.row, potential.column] = 1;
                                                potential.isRoom = true;

                                                DebugRoom(potential, newEndMarker);

                                                roomAdded = true;
                                            }
                                        }
                                    }
                                }

                                if (deadEndReal.Count >= desiredDeadEnds || roomAdded) break;

                                //UP
                                if (potential.column - 1 >= 0)
                                {
                                    foreach (Cell cellCheck in cells)
                                    {
                                        if (cellCheck.column == potential.column - 1 && cellCheck.row == potential.row)
                                        {

                                            if (cellCheck.isDeadEnd && cellCheck.isRoom) adjacentDE = true;

                                            if (cellCheck.neighbors == "" && i == 1)
                                            {
                                                Debug.Log("CREATING NEW D ROOM");
                                                cellCheck.neighbors += "D";
                                                deadEndReal.Add(potential);
                                                a[0, potential.row, potential.column] = 1;
                                                potential.isRoom = true;

                                                DebugRoom(potential, newEndMarker);

                                                roomAdded = true;
                                            }
                                        }
                                    }
                                }

                                if (deadEndReal.Count >= desiredDeadEnds || roomAdded) break;

                                //LEFT
                                if (potential.row - 1 >= 0)
                                {
                                    foreach (Cell cellCheck in cells)
                                    {
                                        if (cellCheck.column == potential.column && cellCheck.row == potential.row - 1)
                                        {

                                            if (cellCheck.isDeadEnd && cellCheck.isRoom) adjacentDE = true;

                                            if (cellCheck.neighbors == "" && i == 1)
                                            {
                                                Debug.Log("CREATING NEW R ROOM");
                                                cellCheck.neighbors += "R";
                                                deadEndReal.Add(potential);
                                                a[0, potential.row, potential.column] = 1;
                                                potential.isRoom = true;

                                                DebugRoom(potential, newEndMarker);

                                                roomAdded = true;
                                            }
                                        }
                                    }
                                }

                                if (deadEndReal.Count >= desiredDeadEnds || roomAdded) break;

                                //RIGHT
                                if (potential.row + 1 <= a.GetLength(1) - 1)
                                {
                                    foreach (Cell cellCheck in cells)
                                    {
                                        if (cellCheck.column == potential.column && cellCheck.row == potential.row + 1)
                                        {

                                            if (cellCheck.isDeadEnd && cellCheck.isRoom) adjacentDE = true;

                                            if (cellCheck.neighbors == "" && i == 1)
                                            {
                                                Debug.Log("CREATING NEW L ROOM");
                                                cellCheck.neighbors += "L";
                                                deadEndReal.Add(potential);
                                                a[0, potential.row, potential.column] = 1;
                                                potential.isRoom = true;

                                                DebugRoom(potential, newEndMarker);

                                                roomAdded = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (roomAdded) continue;
                    }
                }
            }

            Debug.Log("There are now " + deadEndReal.Count + " dead ends.");

            foreach(Cell realEnd in deadEndReal)
            {
                DebugRoom(realEnd, realEndMarker);
            }

        }

        int realRooms = 0;

        foreach(Cell room in cells)
        {
            if (room.isRoom)
            {
                realRooms += 1;
            }
        }

        #region DeadEndAssignment

        foreach (Cell deadEnd in deadEndReal.ToList())
        {
            deadEnd.distanceToSpawn = Mathf.Abs(deadEnd.row - spawnCoordinates[0]) + Mathf.Abs(deadEnd.column - spawnCoordinates[1]);
            if (deadEnd.roomType == Cell.RoomType.SPAWN) deadEndReal.Remove(deadEnd);
        }

        List<Cell> deadEndByDistance = deadEndReal.ToList().OrderBy(cell => cell.distanceToSpawn).ToList();

        deadEndByDistance[^1].roomType = Cell.RoomType.BOSS;
        deadEndReal.Remove(deadEndByDistance[^1]);

        int treasureIndex = UnityEngine.Random.Range(0, deadEndByDistance.Count - 1);

        deadEndByDistance[treasureIndex].roomType = Cell.RoomType.ITEM;
        deadEndReal.Remove(deadEndByDistance[treasureIndex]);

        int shopIndex = UnityEngine.Random.Range(0, deadEndByDistance.Count - 1);

        deadEndByDistance[shopIndex].roomType = Cell.RoomType.SHOP;
        deadEndReal.Remove(deadEndByDistance[shopIndex]);

        #endregion DeadEndAssignment

        if (debugMode) Debug.Log("There are " + realRooms + " rooms");

        if (failedGeneration)
        {
            yield return new WaitForSeconds(5f);

            ResetFloor();
        }
        else
        {
            GenerateRooms();
        }
    }

    void CheckNeighbors(int[,,] a)
    {
        for (int j = 0; j < a.GetLength(1); j++)
        {
            for (int k = 0; k < a.GetLength(2); k++)
            {
                //Check for neighbors

                string neighbors = "";

                //Down
                if (k + 1 <= a.GetLength(2) - 1)
                {
                    if (a[0, j, k + 1] != 0)
                    {
                        neighbors += "D";
                    }
                }
                //Left
                if (j - 1 >= 0)
                {
                    if (a[0, j - 1, k] != 0) {
                        neighbors += "L";
                    }
                }
                //Right
                if (j + 1 <= a.GetLength(1) - 1)
                {
                    if (a[0, j + 1, k] != 0)
                    {
                        neighbors += "R";
                    }
                }
                //Up
                if (k - 1 >= 0)
                {
                    if (a[0, j, k - 1] != 0)
                    {
                        neighbors += "U";
                    }
                }

                Cell roomCell = new Cell(neighbors, 0, j, k);

                if (a[0, j, k] != 0)
                {
                    roomCell.isRoom = true;

                    if (a[0, j, k] == 2)
                    {
                        roomCell.roomType = Cell.RoomType.SPAWN;
                    }
                }

                cells.Add(roomCell);
            }
        }
    }

    void Iterate(int[,,] a)
    {
        while (roomCheck > 0)
        {
            iterations += 1;

            Debug.Log("Beginning iteration #" + iterations);

            roomCheck = desiredRooms;

            for (int j = 0; j < a.GetLength(1); j++)
            {
                for (int k = 0; k < a.GetLength(2); k++)
                {
                    //Checks for spawning cells in the 3d array

                    if (a[1, j, k] == 1 || a[1, j, k] == 2)
                    {
                        if (!spawnCreated && a[1, j, k] == 2)
                        {
                            Debug.Log("Spawn created");
                            a[0, j, k] = 2;
                            spawnCoordinates = new int[] { j, k };
                            spawnCreated = true;
                        }
                        else
                        {
                            if (a[0, j, k] == 0)
                            {
                                a[0, j, k] = 1;
                            }
                        }

                        while (print == false)
                        {
                            int dir = UnityEngine.Random.Range(0, 4);

                            //0 is RIGHT
                            //1 is UP
                            //2 is LEFT
                            //3 is DOWN

                            if (dir == 0)
                            {
                                if (j + 1 <= a.GetLength(1) - 1 && a[1, j + 1, k] != 1 && a[1, j + 1, k] != 2)
                                {
                                    a[1, j + 1, k] = 2;
                                    print = true;
                                    break;
                                }
                            }

                            if (dir == 1)
                            {
                                if (k - 1 >= 0 && a[1, j, k - 1] != 1 && a[1, j, k - 1] != 2)
                                {
                                    a[1, j, k - 1] = 2;
                                    print = true;
                                    break;
                                }
                            }

                            if (dir == 2)
                            {
                                if (j - 1 >= 0 && a[1, j - 1, k] != 1 && a[1, j - 1, k] != 2)
                                {
                                    a[1, j - 1, k] = 2;
                                    print = true;
                                    break;
                                }
                            }

                            if (dir == 3)
                            {
                                if (k + 1 <= a.GetLength(2) - 1 && a[1, j, k + 1] != 1 && a[1, j, k + 1] != 2)
                                {
                                    a[1, j, k + 1] = 2;
                                    print = true;
                                    break;
                                }
                            }
                        }

                        //Removes the spawning cell

                        a[1, j, k] = 0;
                        print = false;

                    }

                    if (a[0, j, k] == 1)
                    {
                        roomCheck -= 1;
                    }
                }
            }

            for (int j = 0; j < a.GetLength(1); j++)
            {
                for (int k = 0; k < a.GetLength(2); k++)
                {
                    if (a[1, j, k] == 2)
                    {
                        a[1, j, k] = 1;
                    }
                }
            }
        }
    }

    void DebugRoom(Cell cell, GameObject markerObject)
    {
        if (debugMode)
        {
            Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
            GameObject marker = Instantiate(markerObject, roomPosition, Quaternion.identity, grid.transform);
            cleanUp.Add(marker);
        }
    }
    public void GenerateRooms()
    {
        foreach(Cell cell in cells)
        {
            if (cell.isRoom)
            {
                if (cell.roomType == Cell.RoomType.DEFAULT)
                {
                    Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
                    GameObject newRoom = Instantiate(roomTemplate, roomPosition, Quaternion.identity, grid.transform);
                    newRoom.GetComponent<TemplateScript>().roomType = TemplateScript.RoomType.DEFAULT;
                    rooms.Add(newRoom);
                }

                if (cell.roomType == Cell.RoomType.SPAWN)
                {
                    Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
                    GameObject newSpawn = Instantiate(roomTemplate, roomPosition, Quaternion.identity, grid.transform);
                    Debug.Log("Creating spawn");
                    newSpawn.GetComponent<TemplateScript>().roomType = TemplateScript.RoomType.SPAWN;
                    rooms.Add(newSpawn);
                }

                if (cell.roomType == Cell.RoomType.SHOP)
                {
                    Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
                    GameObject shop = Instantiate(roomTemplate, roomPosition, Quaternion.identity, grid.transform);
                    shop.GetComponent<TemplateScript>().roomType = TemplateScript.RoomType.SHOP;
                    rooms.Add(shop);
                }

                if (cell.roomType == Cell.RoomType.ITEM)
                {
                    Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
                    GameObject itemRoom = Instantiate(roomTemplate, roomPosition, Quaternion.identity, grid.transform);
                    itemRoom.GetComponent<TemplateScript>().roomType = TemplateScript.RoomType.ITEM;
                    rooms.Add(itemRoom);
                }

                if (cell.roomType == Cell.RoomType.BOSS)
                {
                    Vector3 roomPosition = new(20f * cell.row, 12f * cell.column, 0f);
                    GameObject bossRoom = Instantiate(roomTemplate, roomPosition, Quaternion.identity, grid.transform);
                    Debug.Log("Creating spawn");
                    bossRoom.GetComponent<TemplateScript>().roomType = TemplateScript.RoomType.BOSS;
                    rooms.Add(bossRoom);
                }
            }
        }
    }

    public void ResetFloor()
    {
        StopAllCoroutines();

        PlayerMove.instance.transform.position = new Vector3(-100, -100, 0);

        foreach (GameObject trashObject in cleanUp.ToArray())
        {
            Destroy(trashObject);
        }

        cleanUp.Clear();

        foreach (GameObject room in generatedRooms.ToArray())
        {
            Destroy(room);
        }
        generatedRooms.Clear();

        foreach (GameObject template in rooms.ToArray())
        {
            Destroy(template);
        }
        rooms.Clear();

        cells.Clear();

        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                for (int k = 0; k < a.GetLength(2); k++)
                {
                    a[i, j, k] = 0;
                }
            }
        }

        iterations = 0;
        spawnCreated = false;
        spawnRoom = null;
        bossCreated = false;
        print = false;


        rooms = new List<GameObject>();
        StartCoroutine(Initialize(a));
    }
}
