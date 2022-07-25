using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostEnemy : EnemyAI
{

    float deathTimer;

    void Start()
    {
        EnemyStart();
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    void Update()
    {
        EnemyUpdate();

        if (!alive)
        {
            #region Death

            Dissolve dissolve = GetComponent<Dissolve>();

            dissolve.alive = false;
            
            deathTimer += Time.deltaTime;

            dissolve.opaqueMod = Mathf.Clamp(1 - deathTimer, 0, 1f);
            dissolve.fade = Mathf.Clamp(1 - deathTimer, 0, 1f);

            if(dissolve.opaqueMod < 0)
            {
                dissolve.opaqueMod = 0;
            }

            Die(gameObject, 30, true, 1f);

            #endregion
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
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
