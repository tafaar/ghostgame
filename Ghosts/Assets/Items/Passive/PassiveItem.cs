using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Passive Item")]
public class PassiveItem : ScriptableObject
{


    [Header("Identification")]
    public string name;
    public Sprite sprite;
    public int rarity = 0;

    [Header("Movement Stats")]
    public float health = 0;
    public float mass = 0;
    public float speed = 0;
    public float acceleration = 0;
    public float deceleration = 0;
    public float velPower = 0;

    [Header("Shooting Stats")]
    public float bulletDamage = 0;
    public float bulletForce = 0;
    public float bulletDuration = 0;
    public float shootTime = 0;
    public int piercePower = 0;

    [Header("Other Attributes")]
    public bool hasAttributes = false;

    public virtual void Passive(PlayerMove playerMove)
    {

    }
}
