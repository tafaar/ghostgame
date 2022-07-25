using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenGhostAI : EnemyAI
{
    [SerializeField] GameObject orangeGhost;
    
    [SerializeField] float spitTimeTrigger;

    bool _spitting = false;
    float spitTimer;

    List<GameObject> childGhosts;



    public void Start()
    {
        EnemyStart();

        childGhosts = new List<GameObject>();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), orangeGhost.GetComponent<Collider2D>(), true);

        player = PlayerMove.instance.gameObject;
    }

    

    // Update is called once per frame
    public void Update()
    {
        EnemyUpdate();

        if (childGhosts.Count <= 3)
        {
            spitTimer += Time.deltaTime;
            if (spitTimer >= spitTimeTrigger && !_spitting)
            {
                _spitting = true;
                StartSpit();
            }
        }

        if (!alive)
        {
            Die(gameObject);
        }

        foreach(GameObject child in childGhosts.ToArray())
        {
            if(child.gameObject == null)
            {
                childGhosts.Remove(child);
            }
        }
    }

    private void FixedUpdate()
    {
        EnemyPush();

        Vector2 moveDir;
        Vector2 randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));


        if (alive)
        {
            moveDir = randomDir;
        }
        else
        {
            moveDir = Vector2.zero;
        }

        Move(moveDir);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    public void StartSpit()
    {
        anim.Play("Spit");
    }

    public void Spit()
    {
        rb.AddForce((transform.position - player.transform.position).normalized * 10f, ForceMode2D.Impulse);
        GameObject childOrange = Instantiate(orangeGhost, transform.position, Quaternion.identity);
        childGhosts.Add(childOrange);
    }

    //Set by animation
    public void ResetSpitTimer()
    {
        anim.Play("Idle");
        spitTimer = 0;
        _spitting = false;
    }
}
