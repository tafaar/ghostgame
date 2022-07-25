using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;

public class Room : MonoBehaviour
{
    public enum RoomType { DEFAULT, SPAWN, BOSS, ITEM, SHOP }
    public RoomType roomType = RoomType.DEFAULT;

    PlayerMove playerRef;
    Camera cam;

    [SerializeField]
    GameObject obscurer;

    public GameObject[] lightsComplete;
    public GameObject[] lightsPersistent;
    public GameObject enemyContents;
    public GameObject persistentContents;
    public GameObject nonPersistentContents;

    GameObject roomCopy;

    [SerializeField] List<GameObject> enemies;
    [SerializeField] GameObject door;
    List<GameObject> doors;

    GameObject boss;

    public string doorsNeeded;
    public bool completed = false;
    [SerializeField] bool playerInside = false;

    int aliveEnemies;
    float lightIntensity;
    float checkWait;

    private void Start()
    {
        if (FloorGenerator.instance != null)
        {
            FloorGenerator.instance.generatedRooms.Add(gameObject);
            obscurer = Instantiate(FloorGenerator.instance.obscurer, gameObject.transform);
            door = FloorGenerator.instance.doorPrefab;
        }

        GetComponent<Renderer>().enabled = false;

        doors = new List<GameObject>();
        enemies = new List<GameObject>();

        if(enemyContents != null)
        {
            enemyContents.SetActive(false);
        }

        if(persistentContents != null)
        {
            persistentContents.SetActive(false);
        }

        if (nonPersistentContents != null)
        {
            nonPersistentContents.SetActive(false);
        }

        foreach (char letter in doorsNeeded)
        {
            if(letter.ToString() == "D")
            {
                GameObject doorD = Instantiate(door, new Vector2(transform.position.x, transform.position.y - 4.5f), Quaternion.identity, transform);
                doorD.GetComponent<DoorScript>().direction = "D";
                doors.Add(doorD);
            }
            if (letter.ToString() == "L")
            {
                GameObject doorL = Instantiate(door, new Vector2(transform.position.x - 8.5f, transform.position.y), Quaternion.identity, transform);
                doorL.GetComponent<DoorScript>().direction = "L";
                doors.Add(doorL);
            }
            if (letter.ToString() == "R")
            {
                GameObject doorR = Instantiate(door, new Vector2(transform.position.x + 8.5f, transform.position.y), Quaternion.identity, transform);
                doorR.GetComponent<DoorScript>().direction = "R";
                doors.Add(doorR);
            }
            if (letter.ToString() == "U")
            {
                GameObject doorU = Instantiate(door, new Vector2(transform.position.x, transform.position.y + 4.5f), Quaternion.identity, transform);
                doorU.GetComponent<DoorScript>().direction = "U";
                doors.Add(doorU);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkWait = 0;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = true;

            if (persistentContents != null && roomType == RoomType.DEFAULT)
            {
                persistentContents.SetActive(true);
            }

            if(nonPersistentContents != null)
            {
                nonPersistentContents.SetActive(true);
            }

            if (completed == false)
            {
                collision.gameObject.GetComponent<Shooting>().RemoveAllBullets();
                collision.gameObject.GetComponent<Shooting>().RoomTransition();
            }
            collision.gameObject.GetComponent<PlayerMove>().RoomTransition();

            #region RepositionPlayer

            Vector2 playerPos = collision.gameObject.transform.position;
            Vector2 roomPos = gameObject.transform.localPosition;

            Vector2 entryDirection = roomPos - playerPos;

            if (entryDirection.x > 3)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x - 7.5f, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else if (entryDirection.x < -3)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x + 7.5f, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }

            if (entryDirection.y > 2)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x, roomPos.y - 3.5f, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else if (entryDirection.y < -2)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x, roomPos.y + 3.5f, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else
            {
                collision.gameObject.transform.position = new Vector3(playerPos.x, roomPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }

            #endregion RepositionPlayer

            Debug.Log("Player entered room " + gameObject.name);

            enemies.Clear();

            if (completed == false)
            {
                if (roomType != RoomType.SPAWN)
                {
                    if (enemyContents != null)
                    {
                        roomCopy = Instantiate(enemyContents, gameObject.transform);
                        roomCopy.SetActive(true);
                    }
                }
                else
                {
                    completed = true;
                }
            }

            if(obscurer != null) obscurer.GetComponent<SpriteRenderer>().enabled = false;

            playerRef = collision.GetComponent<PlayerMove>();
            cam = playerRef.cam;

            playerRef.camFollow = false;

            foreach (GameObject light in lightsPersistent)
            {
                light.GetComponent<Light2D>().enabled = true;
            }

            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                enemies.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        checkWait = 0;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInside = false;

            if (obscurer != null) obscurer.GetComponent<SpriteRenderer>().enabled = true;

            if (persistentContents != null)
            {
                persistentContents.SetActive(false);
            }

            if (nonPersistentContents != null)
            {
                nonPersistentContents.SetActive(false);
            }

            Debug.Log("Player left room " + gameObject.name);
            playerRef = collision.GetComponent<PlayerMove>();

            Destroy(roomCopy);

            foreach(GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            lightIntensity = 0;
            foreach (GameObject light in lightsComplete)
            {
                light.GetComponent<Light2D>().intensity = 0;
            }

            foreach (GameObject light in lightsPersistent)
            {
                light.GetComponent<Light2D>().enabled = false;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerInside)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                if (collision.gameObject.GetComponent<EnemyAI>().addedToRoom == false)
                {
                    collision.gameObject.GetComponent<EnemyAI>().addedToRoom = true;
                    enemies.Add(collision.gameObject);
                }
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            checkWait += Time.deltaTime;

            if (!completed)
            {
                foreach(GameObject door in doors)
                {
                    door.GetComponent<DoorScript>().battleLocked = true;
                }
            }
            else
            {
                foreach (GameObject door in doors)
                {
                    door.GetComponent<DoorScript>().battleLocked = false;
                }
            }

            foreach(GameObject enemy in enemies.ToArray())
            {
                if(enemy.gameObject == null)
                {
                    enemies.Remove(enemy);
                }
            }

            if (enemies.Count == 0 && checkWait >= 0.2f)
            {
                if (completed == false)
                {
                    Debug.Log("Room cleared!");
                    
                    if (roomType == RoomType.BOSS)
                    {
                        GameObject bossChest = Instantiate(ItemManager.instance.chestPrefab, transform.position, Quaternion.identity);
                        bossChest.GetComponent<Chest>().rarity = 3;
                        FloorGenerator.instance.desiredRooms += 3;
                        FloorGenerator.instance.Invoke("ResetFloor", 5f);
                    }
                    completed = true;
                }
            }

            if (lightIntensity < 0.7f && completed == true)
            {
                lightIntensity = Mathf.Clamp(lightIntensity + Time.deltaTime * 0.3f, 0, 0.7f);

                foreach (GameObject light in lightsComplete)
                {
                    light.GetComponent<Light2D>().intensity = lightIntensity;
                }
            }

            cam.transform.position = Vector3.MoveTowards(cam.transform.position, new Vector3(gameObject.transform.position.x,
                gameObject.transform.position.y, cam.transform.position.z), 20 * Time.deltaTime);
        }
    }

    public void SetBoss()
    {
        int randomInt = UnityEngine.Random.Range(0, FloorGenerator.instance.bosses.Length - 1);
        enemyContents = Instantiate(FloorGenerator.instance.bosses[randomInt], transform.position, Quaternion.identity, transform);
    }

    public void SetShop()
    {
        int randomInt = UnityEngine.Random.Range(0, FloorGenerator.instance.bosses.Length - 1);
        nonPersistentContents = Instantiate(FloorGenerator.instance.shops[randomInt], transform.position, Quaternion.identity, transform);
    }

    public void SetItemRoom()
    {
        int randomInt = UnityEngine.Random.Range(0, FloorGenerator.instance.bosses.Length - 1);
        nonPersistentContents = Instantiate(FloorGenerator.instance.itemrooms[randomInt], transform.position, Quaternion.identity, transform);
    }

    public void Complete()
    {
        completed = true;
    }
}



