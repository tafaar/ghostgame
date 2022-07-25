using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceGhostAI : EnemyAI
{
    public bool canBounce;
    public float timer;
    int _horizontal;
    int _vertical;
    Vector2 _moveDir;

    void Start()
    {
        EnemyStart();

        int startDir = Random.Range(0, 4);

        if (startDir == 0)
        { 
            _horizontal = 1; 
            _vertical = 1; 
        }
        if (startDir == 1)
        {
            _horizontal = -1;
            _vertical = 1;
        }
        if (startDir == 2)
        {
            _horizontal = -1;
            _vertical = -1;
        }
        if (startDir == 3)
        {
            _horizontal = 1;
            _vertical = -1;
        }

        _moveDir = new Vector2(_horizontal, _vertical);

        player = PlayerMove.instance.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        IgnoreBarriers(collision);

        if (canBounce)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Barriers"))
            {
                _moveDir = Vector2.Reflect(_moveDir, collision.GetContact(0).normal);
            }
        }

        canBounce = false;
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        if (ignoreBarriers == false && ignoredCollisions.Count > 0)
        {
            ClearIgnoredCollisions();
        }

        if (player.transform.position.x - transform.position.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (!alive)
        {
            Die(gameObject);
        }

        if (!canBounce)
        {
            timer += Time.deltaTime;
            if (timer >= 0.1f)
            {
                canBounce = true;
                timer = 0;
            }
        }
    }

    private void FixedUpdate()
    {
        EnemyPush();

        Move(_moveDir);
    }
}
