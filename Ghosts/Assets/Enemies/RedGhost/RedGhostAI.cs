using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedGhostAI : EnemyAI
{
    enum AttackState { MOVE, CHARGE, ATTACK }
    AttackState attackState;
    float deathTimer = 0;
    float attackTimer = 0;
    float coolDown = 0;

    float distanceToPlayer;

    Vector2 _moveDir;

    void Start()
    {
        EnemyStart();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        if(!alive)
        {
            Die(gameObject);
        }

        distanceToPlayer = Vector2.Distance(player.transform.position, gameObject.transform.position);

        if(coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }

        if(distanceToPlayer < 4f && attackState == AttackState.MOVE && coolDown <= 0 && initialized == true)
        {
            attackState = AttackState.CHARGE;
        }

    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Debug.Log(collision.GetContact(0));
        IgnoreBarriers(collision);
    }
    private void FixedUpdate()
    {
        EnemyPush();

        if (alive)
        {
            _moveDir = (player.transform.position - transform.position).normalized;
        }
        else
        {
            _moveDir = Vector2.zero;
        }

        if (attackState == AttackState.MOVE)
        {
            anim.Play("RedGhost");
            Move(_moveDir);
        } 
        
        if (attackState == AttackState.CHARGE)
        {
            anim.Play("Charge");
            attackTimer += Time.deltaTime;

            Move(_moveDir * 0.2f);

            if(attackTimer >= 1.2f && alive)
            {
                attackState = AttackState.ATTACK;
                rb.AddForce(3 * distanceToPlayer * _moveDir, ForceMode2D.Impulse);
            }
        }

        if(attackState == AttackState.ATTACK)
        {
            anim.Play("RedGhost");
            attackTimer += Time.deltaTime;

            if (attackTimer >= 1.4f)
            {
                attackState = AttackState.MOVE;

                attackTimer = 0f;
                coolDown = 3f;
            }
        }
    }
}
