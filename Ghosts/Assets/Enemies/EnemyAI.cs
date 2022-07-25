using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [Header("Physics Data")]
    public EntityData entityData;
    [HideInInspector] 
    public EntityData dataInstance;
    public bool ignoreBarriers = true;
    [Tooltip("Should assign itself on start")]
    public Rigidbody2D rb;
    public float pushRadius;
    public bool avoidStacking;

    [Header("Appearance Data")]
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    [Tooltip("Default is assigned in floor manager")]
    public bool facePlayer = true;

    [Header("Combat Settings")]
    public GameObject defaultBullet;
    public bool dealsTouchDamage = true;
    public bool invincible = false;
    public bool takesKnockback = true;
    public float attackRadius = 1;
    public float attackForce = 25;

    [Header("Debug")]

    public bool droppedPickup;
    public bool initialized;
    public float initializeTimer;
    public bool addedToRoom = false;
    public GameObject player;
    public List<Collider2D> ignoredCollisions;
    public bool dropping;
    public Vector2 dropLocation;
    public float dropTimer;


    public bool alive = true;

    public void EnemyStart()
    {   if(defaultBullet == null) defaultBullet = BulletManager.instance.defaultBullet;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ignoredCollisions = new List<Collider2D>();
        player = PlayerMove.instance.gameObject;
        dataInstance = Instantiate(entityData);
        rb = GetComponent<Rigidbody2D>();
    }

    public void EnemyUpdate()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);

        if (initialized)
        {
            initializeTimer += Time.deltaTime;
        }

        if(dataInstance.health <= 0)
        {
            alive = false;
        }

        if (facePlayer == true && alive)
        {
            if (player.transform.position.x - transform.position.x > 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        if (dropping && transform.position.y > dropLocation.y)
        {
            dropTimer += Time.deltaTime;

            transform.position = Vector2.MoveTowards(transform.position, dropLocation, 7 * Mathf.Pow(dropTimer, 2) * (Time.deltaTime));
        }
        else
        {
            GetComponent<Collider2D>().enabled = true;
            dropping = false;
        }
    }

    public void IgnoreBarriers(Collision2D collision)
    {

        if (ignoreBarriers == true) {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Barriers"))
            {
                Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.collider, true);

                bool notListed = true;

                foreach(Collider2D collider in ignoredCollisions)
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

    public GameObject Shoot(GameObject projectile, Vector2 aimDir, Vector2 shootPosition, float angleModifier = 0, float setDuration = 1.2f, float setDamage = 15f, int setPierce = 1, float setForce = 6, bool scaleBullet = false)
    {

        float angle = Mathf.Atan2(aimDir.y, aimDir.x);
        Vector2 shootDir = new Vector2(Mathf.Cos(angle + angleModifier * Mathf.Deg2Rad), Mathf.Sin(angle + angleModifier * Mathf.Deg2Rad));

        GameObject bulletInstance = Instantiate(projectile, shootPosition, Quaternion.Euler(0, 0, (angle + angleModifier) * Mathf.Rad2Deg - 90f));
        bulletInstance.GetComponent<Bullet>().enemy = true;

        Rigidbody2D bulletRb = bulletInstance.GetComponent<Rigidbody2D>();

        Bullet bullet = bulletInstance.GetComponent<Bullet>();

        bullet.bulletDuration = setDuration;
        bullet.bulletDamage = setDamage;
        bullet.knockback = setForce;

        if (scaleBullet == false) bullet.scaleSize = false; bullet.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        if (bullet.initialized == false)
        {
            bullet.pierce = setPierce;
        }
        bullet.initialized = true;

        bulletRb.AddForce(shootDir.normalized * setForce, ForceMode2D.Impulse);

        return bulletInstance;
    }

    public void ClearIgnoredCollisions()
    {
        foreach(Collider2D collider in ignoredCollisions.ToArray())
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collider, false);
            ignoredCollisions.Remove(collider);
        }
    }

    public void EnemyPush()
    {
        Collider2D[] enemySpace = Physics2D.OverlapCircleAll(gameObject.transform.position, pushRadius);
        if (alive)
        {
            foreach (Collider2D enemyCollider in enemySpace)
            {
                float distance = Vector2.Distance(gameObject.transform.position, enemyCollider.transform.position);

                if (!dropping && avoidStacking == true)
                {
                    if (enemyCollider.gameObject != gameObject &&
                       (enemyCollider.gameObject.layer == LayerMask.NameToLayer("Enemies") ||
                        enemyCollider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                        enemyCollider.gameObject.layer == LayerMask.NameToLayer("Pickups")))
                    {
                        rb.AddForce(5 * enemyCollider.GetComponent<Rigidbody2D>().mass * Mathf.Pow(1 + pushRadius - distance, 2) * (gameObject.transform.position - enemyCollider.transform.position).normalized);
                        enemyCollider.GetComponent<Rigidbody2D>().AddForce(5 * Mathf.Pow(1 + pushRadius - distance, 2) * (enemyCollider.transform.position - gameObject.transform.position).normalized * rb.mass);
                    }
                    if (dealsTouchDamage && enemyCollider.gameObject.layer == LayerMask.NameToLayer("Player") && distance <= attackRadius)
                    {
                        if (entityData.damage > 0)
                        {
                            enemyCollider.gameObject.GetComponent<PlayerMove>().TakeDamage(entityData.damage, transform.position, attackForce);
                        }
                    }
                }
            }
        }
    }

    public virtual void Move(Vector2 moveDir)
    {
        if (initialized && initializeTimer >= 0.6f)
        {
            float targetSpeedX = moveDir.x * dataInstance.speed * rb.mass;

            float speedDifX = targetSpeedX - rb.velocity.x;

            float accelRateX = (Mathf.Abs(targetSpeedX) > 0.01f) ? dataInstance.acceleration : dataInstance.deceleration;

            float movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, dataInstance.velPower) * Mathf.Sign(speedDifX);

            float targetSpeedY = moveDir.y * dataInstance.speed * rb.mass;

            float speedDifY = targetSpeedY - rb.velocity.y;

            float accelRateY = (Mathf.Abs(targetSpeedY) > 0.01f) ? dataInstance.acceleration : dataInstance.deceleration;

            float movementY = Mathf.Pow(Mathf.Abs(speedDifY) * accelRateY, dataInstance.velPower) * Mathf.Sign(speedDifY);


            rb.AddForce(movementX * Vector2.right);
            rb.AddForce(movementY * Vector2.up);
        }
    }

    public bool CheckPlayerCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Touched player");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DropIn(Vector2 location, float offset = 0)
    {
        GetComponent<Collider2D>().enabled = false;

        dropLocation = location;

        transform.position = new Vector2(location.x, location.y + 20);

        dropTimer = offset;
        dropping = true;
    }


    public bool CheckShoot(Collider2D collision)
    {
        if (alive)
        {
            if (collision.CompareTag("Bullet"))
            {
                Bullet bullet = collision.GetComponent<Bullet>();

                if (bullet.enemy == false)
                {
                    if (bullet.pierce >= 1)
                    {
                        GetShot(bullet);
                        bullet.pierce -= 1;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void GetShot(Bullet bullet)
    {
        if (takesKnockback)
        {
            rb.AddForce(bullet.rb.velocity * 0.1f, ForceMode2D.Impulse);
        }
        if (!invincible)
        {
            dataInstance.health -= bullet.bulletDamage;
            if (dataInstance.health <= 0)
            {
                alive = false;
            }
        }
    }

    public virtual void Die(GameObject enemy, float chance = 30f, bool defaultDestroy = true, float time = 0.2f)
    {
        if (!droppedPickup)
        {

            ItemManager.instance.CreatePickup(enemy.transform.position, chance, true);

            if (defaultDestroy == true)
            {
                StartCoroutine(DestroyEnemy(enemy));
            }

            droppedPickup = true;
        }

        IEnumerator DestroyEnemy(GameObject enemy)
        {
            yield return new WaitForSeconds(time);
            Destroy(enemy);
        }
    }
}
