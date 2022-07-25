using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity Data/Entity Data")]
public class EntityData : ScriptableObject
{
    [Header("Generic")]

    public string name;
    public float health;

    [Header("Movement Stats")]

    public float speed;
    public float acceleration;
    public float deceleration;
    public float velPower;
    public float damage;
    public int stamina = 3;
}
