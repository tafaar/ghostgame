using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnRoomScript : MonoBehaviour
{
    PlayerMove playerRef;
    Camera cam;

    [SerializeField]
    SpriteRenderer obscurer;

    private void Start()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

            obscurer.enabled = false;

            playerRef = collision.GetComponent<PlayerMove>();
            cam = playerRef.cam;

            playerRef.camFollow = false;

            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            obscurer.enabled = true;

            Debug.Log("Player left room " + gameObject.name);
            playerRef = collision.GetComponent<PlayerMove>();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, 
                new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, cam.transform.position.z), 20 * Time.deltaTime);

        }
    }
}
