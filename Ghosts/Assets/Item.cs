using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public PassiveItem item;

    SpriteRenderer sprite;
    bool visible;
    bool canBePickedUp = false;

    float pickupTimer;

    [SerializeField] bool persistent;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = item.sprite;
    }

    private void Update()
    {
        if (visible)
        {
            pickupTimer += Time.deltaTime;
        }

        if (pickupTimer >= 0.5f)
        {
            canBePickedUp = true;
        }
    }

    private void FixedUpdate()
    {
        if (visible && canBePickedUp)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    collider.GetComponent<PlayerMove>().playerDataUpdated.passiveItems.Add(item);
                    collider.GetComponent<PlayerMove>().UpdateStats();
                    if (!persistent)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
