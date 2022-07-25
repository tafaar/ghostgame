using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public PassiveItem[] passiveItems;
    public GameObject pickupPrefab;
    public GameObject itemPrefab;
    public GameObject chestPrefab;


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple item managers found");
        }
        instance = this;
    }

    public void CreatePickup(Vector2 position, float dropChance, bool randomizeType = false, PickupType type = PickupType.HEALTH)
    {
        float randomNumber = Random.Range(0, 100f);
        if (randomNumber <= dropChance)
        {
            GameObject newPickup = Instantiate(pickupPrefab, position, Quaternion.identity);

            PickupItem pickupComponent = newPickup.GetComponent<PickupItem>();

            if (randomizeType == false)
            {
                pickupComponent.type = type;
                pickupComponent.UpdateData();
            }
            else
            {
                pickupComponent.RollType();
            }
        }
    }
}
