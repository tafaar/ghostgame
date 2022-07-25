using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] public string direction;
    [SerializeField] LayerMask playerLayer;

    [SerializeField] Collider2D doorBlocker;

    GameObject _player;
    bool visible;
    bool entered;

    public bool battleLocked;

    float timer;

    private void Start()
    {
        anim.Play("Closed" + direction);
    }

    private void Update()
    {
        if (battleLocked)
        {
            doorBlocker.enabled = true;
        }
        else
        {
            doorBlocker.enabled = false;
        }

        if (entered)
        {
            timer += Time.deltaTime;
            if (timer >= 0.4f)
            {
                entered = false;
                if (direction == "D")
                {
                    _player.transform.position = new Vector3(transform.position.x, transform.position.y - 4.5f, 0);
                }
                else if(direction == "L"){
                    _player.transform.position = new Vector3(transform.position.x - 4.5f, transform.position.y, 0);
                }
                else if (direction == "R")
                {
                    _player.transform.position = new Vector3(transform.position.x + 4.5f, transform.position.y, 0);
                }
                else if (direction == "U")
                {
                    _player.transform.position = new Vector3(transform.position.x, transform.position.y + 4.5f, 0);
                }
                _player.GetComponent<PlayerMove>().rb.velocity = Vector2.zero;
                _player = null;

                timer = 0;
            }
        }
    }
    private void FixedUpdate()
    {
        if (!battleLocked)
        {
            if (direction == "D" || direction == "U")
            {
                Collider2D[] colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 1f, transform.position.y - 0.4f), new Vector2(transform.position.x + 1f, transform.position.y + 0.4f));
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Player"))
                    {
                        if (!entered)
                        {
                            _player = collider.gameObject;
                            anim.Play("Open" + direction);
                            entered = true;
                        }
                    }
                }
            }

            if (direction == "L" || direction == "R")
            {
                Collider2D[] colliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 0.4f, transform.position.y - 1f), new Vector2(transform.position.x + 0.4f, transform.position.y + 1f));
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.CompareTag("Player"))
                    {
                        if (!entered)
                        {
                            _player = collider.gameObject;
                            anim.Play("Open" + direction);
                            entered = true;
                        }
                    }
                }
            }
        }
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
