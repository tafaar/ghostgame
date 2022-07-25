using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    float angle;

    void Update()
    {
        if (PlayerMove.instance.aim == Vector2.zero)
        {
            angle = Mathf.Atan2(PlayerMove.instance.look.y, PlayerMove.instance.look.x) * Mathf.Rad2Deg - 90;
        }
        else
        {
            angle = Mathf.Atan2(PlayerMove.instance.aim.y, PlayerMove.instance.aim.x) * Mathf.Rad2Deg - 90;
        }
        transform.eulerAngles = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, angle, 0.5f));
    }
}
