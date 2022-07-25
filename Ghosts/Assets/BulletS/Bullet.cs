using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 direction;
    public Rigidbody2D rb;
    public Animator anim;

    float time = 0;

    public float bulletDuration;
    public float bulletDamage;
    public float knockback;
    public int pierce = 1;

    public bool scaleSize = true;
    public bool initialized = false;
    public bool alive = true;
    public bool enemy = false;
    bool hitPlayer = false;

    bool _scaled = false;

    Vector2 initialPos;

    [SerializeField] AudioSource audioSource;

    public bool splatWalls = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPos = transform.position;
    }

    private void Update()
    {
        time += Time.deltaTime;

        if (time >= bulletDuration)
        {
            alive = false;
        }

        if (pierce <= 0)
        {
            alive = false;
        }

        if(!alive)
        {
            GetComponent<Collider2D >().enabled = false;
            rb.velocity = Vector2.zero;
            if (anim != null)
            {
                anim.Play("BulletSplat");
            }
            else
            {
                Remove();
            }
        }

        if (initialized && scaleSize)
        {

            float damageSizeCap = Mathf.Clamp(bulletDamage, 2f, 15);
            float bulletScale = Mathf.Clamp((-(Mathf.Pow(damageSizeCap - 15, 2)) / 75) + 3, 0.2f, 5);

            transform.localScale = new Vector3(bulletScale, bulletScale, bulletScale);
            if (_scaled == false)
            {
                GetComponent<CircleCollider2D>().radius = GetComponent<CircleCollider2D>().radius * bulletScale;
                _scaled = true;
            }
        }

        if (enemy)
        {
            if (hitPlayer == false)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Player"))
                    {
                        pierce -= 1;

                        Vector2 currentPos = transform.position;

                        collider.gameObject.GetComponent<PlayerMove>().TakeDamage(bulletDamage, (currentPos - initialPos).normalized, knockback * 3, true);
                        hitPlayer = true;
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (splatWalls && time >= 0.2f)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                alive = false;
            }
        }
    }

    public void Remove()
    {
        PlayerMove.instance.GetComponent<Shooting>().RemoveBullet(this);
        Destroy(gameObject);
    }
}
