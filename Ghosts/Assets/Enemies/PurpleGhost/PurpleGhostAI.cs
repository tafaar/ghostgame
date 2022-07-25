using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleGhostAI : EnemyAI
{

    Vector2 _moveDir;

    public float timer;

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Alert = Animator.StringToHash("Alert");
    private static readonly int Yell = Animator.StringToHash("Yell");
    float _lockedTill;
    int _currentState;
    bool _alerting;
    bool _attacking;


    void Start()
    {
        EnemyStart();

        player = PlayerMove.instance.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckShoot(collision);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IgnoreBarriers(collision);
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        

        if(ignoreBarriers == false && ignoredCollisions.Count > 0)
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

        _moveDir = (player.transform.position - transform.position).normalized;

        if (!alive)
        {
            Die(gameObject);
        }

        if (initialized)
        {
            timer += Time.deltaTime;

            if (timer >= 2f)
            {
                _alerting = true;

                if(timer >= 2.45f)
                {
                    _attacking = true;
                }
            }
        }

        float speedMod = 1;

        if (_alerting) speedMod = 0.5f;
        if (_attacking) speedMod = 0.2f;
        if (!_attacking && !_alerting) speedMod = 1;

        if (alive)
        {
            Move(_moveDir * speedMod);
        }

        var state = GetState();

        if (state != _currentState)
        {
            anim.CrossFade(state, 0, 0);
            _currentState = state;
        }
    }

    public void ShootAlert()
    {
        int shotAmount = Random.Range(5, 8);

        for (int i = shotAmount; i > 0; i--)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 2, ForceMode2D.Impulse);
            Shoot(defaultBullet, aimDir: player.transform.position - transform.position, transform.position, angleModifier: Random.Range(-22.5f, 22.5f), setDamage: Random.Range(5, 11), setForce: Random.Range(4f, 7f));
        }
        
        timer = 0;
        _alerting = false;
        _attacking = false;
    }

    private void FixedUpdate()
    {
        EnemyPush();
    }

    private int GetState()
    {
        if (Time.time < _lockedTill) return _currentState;

        if (!alive) return LockedState(Idle, 5f);
        if (_attacking) {
            return LockedState(Yell, 1f);
        }
        if (_alerting) return LockedState(Alert, 0.45f);

        return Idle;

        int LockedState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }
}
