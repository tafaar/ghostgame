using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity Data/Player Data")]
public class PlayerData : EntityData
{
    public bool damageInvuln;

    [Header("Shooting Stats")]
    public float bulletDamage;
    public float bulletForce;
    public float bulletDuration;
    public float shootTime;
    public int piercePower;

    [Header("Other")]
    public int coins;

    public List<PassiveItem> passiveItems;

    public PlayerData UpdateStats(PlayerData original)
    {
        PlayerData updated = Instantiate(original);
        updated.passiveItems = passiveItems;

        foreach (PassiveItem item in passiveItems)
        {
            Debug.Log("Updating stats with item named " + item.name);

            updated.health += item.health;
            updated.speed += item.speed;
            updated.acceleration += item.acceleration;
            updated.deceleration += item.deceleration;
            updated.velPower += item.velPower;

            updated.bulletDamage += item.bulletDamage;
            updated.bulletForce += item.bulletForce;
            updated.bulletDuration += item.bulletDuration;
            updated.shootTime += item.shootTime;
            updated.piercePower += item.piercePower;
        }
        updated.shootTime = Mathf.Clamp(updated.shootTime, 0.05f, 2);
        updated.piercePower = Mathf.RoundToInt(Mathf.Clamp(updated.piercePower, 1, Mathf.Infinity));
        updated.bulletDamage = Mathf.Clamp(updated.bulletDamage, 0.5f, Mathf.Infinity);

        return updated;
    }
}
