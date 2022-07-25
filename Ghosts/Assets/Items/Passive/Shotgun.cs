using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attributed Item/Shotgun")]
public class Shotgun : PassiveItem
{
    float randomAngle;
    public override void Passive(PlayerMove playerMove)
    {

        if (playerMove.GetComponent<Shooting>().shot)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i < 2)
                {
                    randomAngle = Random.Range(0, 22.5f);
                }
                else
                {
                    randomAngle = Random.Range(-22.5f, 0);
                }
                RandomizeShot(playerMove);
            }
        }
    }

    void RandomizeShot(PlayerMove playerMove)
    {
        playerMove.GetComponent<Shooting>().angleModifier = randomAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(playerMove.GetComponent<Shooting>().angle + randomAngle * Mathf.Deg2Rad), Mathf.Sin(playerMove.GetComponent<Shooting>().angle + randomAngle * Mathf.Deg2Rad));
        playerMove.GetComponent<Shooting>().ShootAlt(dir, true);
    }
}
