using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RoomScript : MonoBehaviour
{
    PlayerMove playerRef;
    Camera cam;

    [SerializeField]
    SpriteRenderer obscurer;


    [SerializeField] GameObject[] lightsComplete;
    [SerializeField] GameObject[] lightsPersistent;
    [SerializeField] GameObject roomArt;

    GameObject pickupPrefab;

    List<GameObject> contents;
    List<GameObject> contentCopies;
    List<GameObject> enemies;

    GameObject boss;

    public string roomType;
    public bool isSpawnRoom = false;
    public bool isBossRoom = false;
    public bool completed = false;
    int aliveEnemies;
    float lightIntensity;

    private void Start()
    {
        contents = new List<GameObject>();
        contentCopies = new List<GameObject>();
        enemies = new List<GameObject>();
        pickupPrefab = ItemManager.instance.pickupPrefab;

        FloorGenerator.instance.generatedRooms.Add(gameObject);

        StartCoroutine(GrabContents());

        roomArt = FloorGenerator.instance.roomArt;

        GameObject art = Instantiate(roomArt, gameObject.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
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

            Debug.Log(entryDirection);

            if(entryDirection.x > 7)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x - 7.5f, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else if(entryDirection.x < -7)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x + 7.5f, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x, playerPos.y, 0);
                playerPos = collision.gameObject.transform.position;
            }

            if(entryDirection.y > 4)
            {
                collision.gameObject.transform.position = new Vector3(roomPos.x, roomPos.y - 3.5f, 0);
                playerPos = collision.gameObject.transform.position;
            }
            else if(entryDirection.y < -4)
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

            if(completed == false)
            {
                if (!isSpawnRoom)
                {
                    foreach (GameObject roomObject in contents)
                    {
                        GameObject copy = Instantiate(roomObject, gameObject.transform);
                        contentCopies.Add(copy);
                        copy.SetActive(true);

                        foreach (GameObject roomCopy in contentCopies)
                        {
                            if (roomCopy.layer == LayerMask.NameToLayer("Enemies"))
                            {
                                enemies.Add(roomCopy);
                            }
                        }
                    }
                }
                else if(isSpawnRoom)
                {
                    completed = true;
                }
            }

            obscurer.enabled = false;

            playerRef = collision.GetComponent<PlayerMove>();
            cam = playerRef.cam;

            playerRef.camFollow = false;

            foreach (GameObject light in lightsPersistent)
            {
                light.GetComponent<Light2D>().enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            obscurer.enabled = true;

            Debug.Log("Player left room " + gameObject.name);
            playerRef = collision.GetComponent<PlayerMove>();
            if (completed == false)
            {
                foreach (GameObject copy in contentCopies)
                {
                    Destroy(copy);
                }
                contentCopies.Clear();
            }

            lightIntensity = 0;
            foreach(GameObject light in lightsComplete)
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
        

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (enemies.Count !=0 )
            {
                aliveEnemies = enemies.Count;
                foreach(GameObject enemy in enemies)
                {
                    if(enemy.gameObject == null)
                    {
                        aliveEnemies -= 1;
                    }
                }
                if (aliveEnemies == 0)
                {
                    if (completed == false)
                    {
                        Debug.Log("Room cleared!");
                        GameObject pickup = Instantiate(pickupPrefab, gameObject.transform.position, Quaternion.identity);
                        pickup.GetComponent<PickupItem>().Initialize();
                        pickup.GetComponent<PickupItem>().RollType();
                        completed = true;
                    }
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

    public IEnumerator GrabContents()
    {
        yield return new WaitForSeconds(1f);


        Vector2 pos = transform.position;
        Collider2D[] contentsColliders = Physics2D.OverlapAreaAll(pos + new Vector2(-8f, -4f), pos + new Vector2(8, 4));
        foreach (Collider2D collider in contentsColliders)
        {
            if (collider.gameObject != gameObject
                && collider.gameObject.CompareTag("SpawnPoint") == false
                && collider.gameObject.CompareTag("Rooms") == false
                && collider.gameObject.CompareTag("Item") == false
                && collider.gameObject.CompareTag("Template") == false
                && collider.gameObject.CompareTag("Player") == false)
            {
                contents.Add(collider.gameObject);
                collider.gameObject.SetActive(false);
            }
        }

        if (isBossRoom)
        {
            contents.Clear();
            GameObject selectedBoss = FloorGenerator.instance.bosses[Random.Range(0, FloorGenerator.instance.bosses.Length - 1)];
            contents.Add(selectedBoss);
        }

        foreach (GameObject light in lightsPersistent)
        {
            light.GetComponent<Light2D>().enabled = false;
        }

        yield return null;
    }

    public void Complete()
    {
        completed = true;
    }
}
