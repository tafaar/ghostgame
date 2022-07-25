using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI : EnemyAI
{
    void Start()
    {
        EnemyStart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        if (!alive)
        {
            Die(gameObject);
        }
    }

    private void FixedUpdate()
    {
        EnemyPush();

        Vector2 moveDir;

        if (alive)
        {
            moveDir = (player.transform.position - gameObject.transform.position).normalized;
        }
        else
        {
            moveDir = Vector2.zero;
        }

        Move(moveDir);
    }


}
