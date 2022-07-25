using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlueGhostAI : EnemyAI
{
    
    enum AttackState { IDLE, POSSESS, ATTACK}

    [Header("Possession Debug")]
    [SerializeField] AttackState attackState;
    [SerializeField] float searchTimer = 0;
    [SerializeField] float searchTrigger;
    [SerializeField] GameObject possessSelection;

    float animTimer;
    float speedModifier = 1;
    float _moveTimer;

    public LayerMask walls;
    Vector2 lookDir;
    Vector2 moveDir;
    Vector2 randomDir;

    private void OnBecameVisible()
    {
        initialized = true;
    }

    void Start()
    {
        EnemyStart();

        searchTrigger = Random.Range(1f, 2f);

        moveDir = Vector2.zero;
    }

    void Update()
    {
        if (possessSelection = null) attackState = AttackState.IDLE;

        EnemyUpdate();

        _moveTimer += Time.deltaTime;

        if(_moveTimer >= 0.1f)
        {
            randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            _moveTimer = 0;
        }

        searchTimer += Time.deltaTime;

        if (!alive)
        {
            Die(gameObject);

            spriteRenderer.enabled = true;

            if (possessSelection != null)
            {
                possessSelection.GetComponent<PossessableObject>().targeted = false;
                possessSelection.GetComponent<PossessableObject>().possessor = null;
                possessSelection = null;
            }
        }

        if (attackState == AttackState.IDLE)
        {
            if (!ignoreBarriers)
            {
                foreach (Collider2D obj in ignoredCollisions.ToList())
                {
                    if (obj.CompareTag("Barriers") == false)
                    {
                        Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), obj, false);
                        ignoredCollisions.Remove(obj);
                    }
                }
            }
            else
            {
                foreach (Collider2D collider in ignoredCollisions)
                {
                    Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collider, false);
                }
                ignoredCollisions.Clear();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackState != AttackState.ATTACK)
        {
            CheckShoot(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        IgnoreBarriers(collision);

        if (attackState != AttackState.IDLE)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Walls"))
            {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.collider, true);

                bool notListed = true;

                foreach (Collider2D collider in ignoredCollisions)
                {
                    if (collider == collision.collider)
                    {
                        notListed = false;
                    }
                }

                if (notListed == true)
                {
                    ignoredCollisions.Add(collision.collider);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (alive)
        {

            //Handles collision with other enemies

            EnemyPush();

            //Search for an object that can be possessed

            #region search
            if (searchTimer >= searchTrigger && attackState == AttackState.IDLE)
            {
                Collider2D[] possessableObjects = Physics2D.OverlapCircleAll(gameObject.transform.position, 20f);
                foreach (Collider2D possessableObject in possessableObjects)
                {
                    if (possessableObject.CompareTag("Possessable"))
                    {
                        if (possessableObject.GetComponent<PossessableObject>().targeted == false && possessableObject.transform.parent.parent == gameObject.transform.parent.parent)
                        {
                            possessableObject.GetComponent<PossessableObject>().targeted = true;

                            possessSelection = possessableObject.gameObject;
                            anim.Play("BlueGhostAlert");
                            attackState = AttackState.POSSESS;
                            speedModifier = 3f;

                            searchTimer = 0;
                            searchTrigger = Random.Range(1f, 3f);

                            return;
                        }
                    }
                }
                searchTimer = 0;
                searchTrigger = Random.Range(3, 6);
            }
            #endregion

            //Determine the ghost's action based on the state

            #region statebehavior
            if (attackState == AttackState.IDLE)
            {
                anim.Play("BlueGhostIdle");
                speedModifier = 1f;
                lookDir = Mathf.Pow((2 / Vector2.Distance(gameObject.transform.position, player.transform.position)), 2) * 
                    (player.transform.position - gameObject.transform.position).normalized;
            }

            if (attackState == AttackState.POSSESS)
            {
                anim.Play("BlueGhostAlert");

                animTimer += Time.deltaTime;

                if (animTimer >= 1f)
                {
                    anim.Play("BlueGhostIdle");

                    lookDir = (possessSelection.transform.position - gameObject.transform.position).normalized;

                    if (Vector2.Distance(gameObject.transform.position, possessSelection.transform.position) < 1f)
                    {
                        possessSelection.GetComponent<PossessableObject>().possessor = this;

                        attackState = AttackState.ATTACK;
                        searchTimer = 0f;
                        animTimer = 0f;
                    }
                }
                else
                {
                    lookDir = Vector2.zero;
                }
            }

            if (attackState == AttackState.ATTACK)
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;

                speedModifier = 1f;
                lookDir = Vector2.zero;

                if (searchTimer >= 2f)
                {
                    Vector2 throwDir = (player.transform.position - gameObject.transform.position).normalized;
                    float throwDis = Vector2.Distance(possessSelection.transform.position, player.transform.position);

                    gameObject.GetComponent<SpriteRenderer>().enabled = true;

                    possessSelection.GetComponent<PossessableObject>().ThrowAtPlayer(throwDis, throwDir);
                    possessSelection = null;

                    attackState = AttackState.IDLE;
                    searchTimer = 0;
                }
            }
            #endregion statebehavior

            //Movement stuff

            if(attackState == AttackState.IDLE)
            {
                Vector2 temp = transform.position - player.transform.position;
                float clampedDistance = Mathf.Clamp(5 - Vector2.Distance(player.transform.position, transform.position), 0, 5f);
                moveDir = randomDir + (clampedDistance * temp);
            }
            else
            {
                moveDir = lookDir;
            }

            if (!alive)
            {
                moveDir = Vector2.zero;
            }

            Move(moveDir * speedModifier);
        }
    }
}
