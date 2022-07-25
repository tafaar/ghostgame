using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Animator _anim;

    public int rarity;

    bool _opened = false;

    public List<GameObject> contents;

    private void Start()
    {
        DetermineItem();
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!_opened)
            {
                _anim.Play("Open" + rarity.ToString());

                if (contents.Count > 0)
                {
                    foreach (GameObject item in contents)
                    {
                        Instantiate(item);
                        item.SetActive(true);
                    }
                    contents.Clear();
                }

                _opened = true;
            }
        }
    }

    public void DetermineItem()
    {
        PassiveItem[] passiveItems = ItemManager.instance.passiveItems;

        _anim.Play("Unopened" + rarity.ToString());

        List<PassiveItem> possibleItems = new List<PassiveItem>();

        foreach (PassiveItem item in passiveItems)
        {
            if (item.rarity == rarity)
            {
                possibleItems.Add(item);
            }
        }

        int itemIndex = Random.Range(0, possibleItems.Count);

        GameObject newItem = Instantiate(ItemManager.instance.itemPrefab, gameObject.transform.position, Quaternion.identity);

        newItem.GetComponent<Item>().item = possibleItems[itemIndex];

        contents.Add(newItem);

        newItem.SetActive(false);
    }
}
