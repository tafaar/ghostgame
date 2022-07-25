using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableObject : MonoBehaviour
{
    public bool targeted = false;

    public float mass = 1;

    public Rigidbody2D rb;
    public BlueGhostAI possessor;

    public SpriteRenderer possessedSprite;

    float targetCheck;

    bool thrown;
    float initialVelocity;

    float _drag;

    List<GameObject> ignoredCollisions;

    void Start()
    {
        _drag = rb.drag;
        // Ignore collision with enemies.
        possessedSprite.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        possessedSprite.sortingOrder = -Mathf.RoundToInt(transform.position.y) + 1;

        if (possessor == null)
        {
            possessedSprite.enabled = false;
        }
        else
        {
            possessedSprite.enabled = true;
        }

        if (targeted)
        {
            targetCheck += Time.deltaTime;
            if (targetCheck >= 10)
            {
                targeted = false;
            }
        }
        else
        {
            targetCheck = 0;
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(-rb.velocity * mass);

        if (rb.velocity.magnitude <= initialVelocity / 3 && thrown)
        {
            thrown = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (possessor != null)
        {
            if (collision.CompareTag("Bullet"))
            {
                Bullet bullet = collision.GetComponent<Bullet>();
                if (bullet.pierce >= 1)
                {
                    possessor.GetShot(bullet);
                    bullet.pierce -= 1;
                }
            }
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (thrown && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit player");
            collision.gameObject.GetComponent<PlayerMove>().TakeDamage(5f, gameObject.transform.position, rb.velocity.magnitude * 3);
            thrown = false;
        }

        if(thrown && collision.gameObject.layer != LayerMask.NameToLayer("Enemies"))
        {
            rb.drag = _drag;
        }
    }

    public void ThrowAtPlayer(float distance, Vector2 direction)
    {
        targeted = false;
        possessor = null;

        thrown = true;

        float rand = Random.Range(-1, 2);

        rb.drag = 0;
        rb.AddTorque(100f * rand);
        rb.AddForce(4 * distance * direction * rb.mass, ForceMode2D.Impulse);

        initialVelocity = rb.velocity.magnitude;
    }
}
