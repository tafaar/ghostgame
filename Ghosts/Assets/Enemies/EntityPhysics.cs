using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityPhysics : MonoBehaviour
{
    public EntityData entityData;
    EntityData instance;
    Rigidbody2D rb;

    public float pushRadius;
    public bool avoidEntityCollision;

    public bool initialized = false;
    float initializeTimer = 0;

    public bool alive = true;

    public bool addedToRoom;

    public void Start()
    {
        instance = Instantiate(entityData);
        rb = GetComponent<Rigidbody2D>();
    }

    public void Update()
    {
        if (initialized)
        {
            initializeTimer += Time.deltaTime;
        }
        else
        {
            initializeTimer = 0;
        }
    }

    public void FixedUpdate()
    {
        Collider2D[] enemySpace = Physics2D.OverlapCircleAll(gameObject.transform.position, pushRadius);
        if (alive)
        {
            foreach (Collider2D enemyCollider in enemySpace)
            {
                if (avoidEntityCollision == true &&
                    enemyCollider.gameObject != gameObject &&
                   (enemyCollider.gameObject.layer == LayerMask.NameToLayer("Enemies") ||
                    enemyCollider.gameObject.layer == LayerMask.NameToLayer("Player") ||
                    enemyCollider.gameObject.layer == LayerMask.NameToLayer("Pickups")))
                {
                    float distance = Vector2.Distance(gameObject.transform.position, enemyCollider.transform.position);
                    rb.AddForce(5 * instance.speed * Mathf.Pow(1 + pushRadius - distance, 2) * (gameObject.transform.position - enemyCollider.transform.position).normalized);
                    if (gameObject.transform.position == enemyCollider.transform.position)
                    {
                        rb.AddForce(5 * instance.speed * Mathf.Pow(1 + pushRadius, 2) * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)));
                    }
                }


                if (enemyCollider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    if (entityData.damage > 0)
                    {
                        enemyCollider.gameObject.GetComponent<PlayerMove>().TakeDamage(entityData.damage, transform.position, 20);
                    }
                }
            }
        }
    }

    public void Move(Vector2 moveDir)
    {
        if (initialized && initializeTimer >= 1.0f)
        {
            float targetSpeedX = moveDir.x * instance.speed;

            float speedDifX = targetSpeedX - rb.velocity.x;

            float accelRateX = (Mathf.Abs(targetSpeedX) > 0.01f) ? instance.acceleration : instance.deceleration;

            float movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, instance.velPower) * Mathf.Sign(speedDifX);

            float targetSpeedY = moveDir.y * instance.speed;

            float speedDifY = targetSpeedY - rb.velocity.y;

            float accelRateY = (Mathf.Abs(targetSpeedY) > 0.01f) ? instance.acceleration : instance.deceleration;

            float movementY = Mathf.Pow(Mathf.Abs(speedDifY) * accelRateY, instance.velPower) * Mathf.Sign(speedDifY);


            rb.AddForce(movementX * Vector2.right);
            rb.AddForce(movementY * Vector2.up);
        }
    }

    public void OnBecameVisible()
    {
        initialized = true;
    }
}
