using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KingRatAI : EnemyAI
{

    [SerializeField] GameObject bigRatMinion;

    float _attackTimer;
    float _maxHP;
    float _currentHP;

    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Run = Animator.StringToHash("Run");
    float _lockedTill;
    int _currentState;
    bool _attacking;
    bool _walking;
    bool _running;

    [SerializeField] Sprite cheeseWheel;
    [SerializeField] Sprite cheeseSlice;

    List<GameObject> projectiles;

    List<GameObject> ratChildren;

    // Start is called before the first frame update
    void Start()
    {
        projectiles = new();
        ratChildren = new List<GameObject>();
        EnemyStart();
        _maxHP = dataInstance.health;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject rat in ratChildren.ToArray())
        {
            if(rat.GetComponent<EnemyAI>().alive == false)
            {
                ratChildren.Remove(rat);
            }
        }

        _currentHP = dataInstance.health;

        foreach(GameObject projectile in projectiles.ToList())
        {
            if(projectile.GetComponent<Bullet>().alive == false)
            {
                GameObject slice = Shoot(BulletManager.instance.cheeseSlice, (player.transform.position - projectile.transform.position).normalized, projectile.transform.position, setForce: 5, setDuration: 2f);
                slice.GetComponent<SpriteRenderer>().sprite = cheeseSlice;
                projectiles.Remove(projectile);
            }
        }

        if (!alive)
        {
            Die(gameObject, 100, true, 5f);
        }

        _attackTimer += Time.deltaTime;

        EnemyUpdate();

        if (_attackTimer >= 3f)
        {
            int attackRoll = Random.Range(0, 2);

            if (attackRoll == 1)
            {
                if (ratChildren.Count <= 2f) _attacking = true;
            }
            _attackTimer = 0f;
        }

        if (_currentHP <= _maxHP / 2)
        {
            _running = true;
            dataInstance.speed = entityData.speed * 2.5f;
        }

        var state = GetState();

        if (state == _currentState) return;
        anim.CrossFade(state, 0, 0);
        _currentState = state;

        _attacking = false;
        
    }

    private void OnBecameVisible()
    {
        initialized = true;
    }

    private void FixedUpdate()
    {
        EnemyPush();

        Vector2 moveDir;

        if (alive && _currentState != Attack)
        {
            moveDir = (player.transform.position - gameObject.transform.position).normalized;
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

    public void AttackShoot()
    {
        GameObject projectile = Shoot(BulletManager.instance.cheeseWheel, (player.transform.position - transform.position).normalized, transform.position, setDuration: 2.5f, setDamage: 20f);
        projectile.GetComponent<SpriteRenderer>().sprite = cheeseWheel;
        projectile.GetComponent<Bullet>().splatWalls = true;

        projectiles.Add(projectile);
    }

    public void SummonRat()
    {
        GameObject ratChild = Instantiate(bigRatMinion, gameObject.transform.position, Quaternion.identity);
        ratChild.GetComponent<EnemyAI>().DropIn(player.transform.position, 0.1f);
        ratChildren.Add(ratChild);
    }

    private int GetState()
    {
        if (Time.time < _lockedTill) return _currentState;

        if (!alive) return LockedState(Death, 5f);
        if (_attacking) return LockedState(Attack, 1.2f);
        if (_running) return Run;
        
        return Walk;

        int LockedState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }
    }

}
