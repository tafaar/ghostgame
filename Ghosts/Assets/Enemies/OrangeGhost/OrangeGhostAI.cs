using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeGhostAI : EnemyAI
{

    float deathTimer;
    bool impacted = false;
    bool animating = false;
    float aimBuffer;


    private void OnBecameVisible()
    {
        initialized = true;
    }

    void Start()
    {
        EnemyStart();

        
        rb.velocity = (player.transform.position - transform.position).normalized * 10f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!impacted)
        {
            float rot = Mathf.Atan2(collision.GetContact(0).point.y - transform.position.y, collision.GetContact(0).point.x - transform.position.x) * Mathf.Rad2Deg - 90f;
            transform.eulerAngles = new Vector3(0f, 0f, rot);
            Impact();
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        aimBuffer += Time.deltaTime;

        if(aimBuffer <= 0.2f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);

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
            Die(gameObject, -1f);
        }
    }

    private void FixedUpdate()
    {
        EnemyPush();

        Vector2 moveDir;

        if (alive && impacted && !animating)
        {
            moveDir = (player.transform.position - gameObject.transform.position).normalized;
            Move(moveDir);
        }

        if(!alive || animating)
        {
            moveDir = Vector2.zero;
            Move(moveDir);
        }

        
    }

    public void Impact()
    {
        rb.velocity = Vector2.zero;
        impacted = true;
        rb.freezeRotation = true;
        animating = true;
        anim.Play("Impact");

    }

    public void EndImpact()
    {
        animating = false;
        transform.eulerAngles = new Vector3(0, 0, 0);
        anim.Play("Idle");
    }

}
