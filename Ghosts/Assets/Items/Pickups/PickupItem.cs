using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PickupType { HEALTH, COIN }
public class PickupItem : MonoBehaviour
{
    public Sprite[] sprites;
    SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Light2D light;

    public float pushForce;
    public bool rollOnAwake = false;

    public PickupType type;

    void Awake()
    {
        Initialize();
        if(rollOnAwake == true)
        {
            RollType();
        }
        else
        {
            UpdateData();
        }
    }

    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }
    private void FixedUpdate()
    {
        Collider2D[] itemSpace = Physics2D.OverlapCircleAll(gameObject.transform.position, 0.25f);
        foreach(Collider2D player in itemSpace)
        {
            if (player.gameObject.CompareTag("Player"))
            {
                PlayerMove playerMove = player.GetComponent<PlayerMove>();
                if (type == PickupType.HEALTH)
                {
                    if (playerMove.currentHealth != playerMove.maxHealth)
                    {
                        player.GetComponent<PlayerMove>().Heal(10f);
                        Destroy(gameObject);
                    }
                    else
                    {
                        rb.AddForce((pushForce - Vector2.Distance(transform.position, player.transform.position)) * (transform.position - player.transform.position));
                    }
                }
                else if (type == PickupType.COIN)
                {
                    player.GetComponent<PlayerMove>().playerDataUpdated.coins += 1;
                    Destroy(gameObject);
                }
            }
        }
    }

    public void RollType()
    {
        int roll = Random.Range(0, 2);

        if(roll == 0)
        {
            type = PickupType.HEALTH;
        }else if(roll == 1)
        {
            type = PickupType.COIN;
        }

        UpdateData();
    }

    public void Initialize()
    {
        light = GetComponent<Light2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void UpdateData()
    {
        if(type == PickupType.HEALTH)
        {
            light.intensity = 1;
            light.color = Color.red;
            spriteRenderer.sprite = sprites[0];
        }
        else if(type == PickupType.COIN)
        {
            light.intensity = 0;
            spriteRenderer.sprite = sprites[1];
        }
    }
}
