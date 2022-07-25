using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attributed Item/Beer")]
public class Beer : PassiveItem
{
    float timer;
    public override void Passive(PlayerMove playerMove)
    {
        timer += Time.deltaTime;

        if (timer > 0.02f)
        {
            float randomAngle = Random.Range(-22.5f, 22.5f);

            playerMove.GetComponent<Shooting>().angleModifier = randomAngle * Mathf.Deg2Rad;

            float randomX = Random.Range(-0.1f, 0.1f);
            float randomY = Random.Range(-0.1f, 0.1f);
            playerMove.move += new Vector2(randomX, randomY);
            playerMove.move.x = Mathf.Clamp(playerMove.move.x, -2f, 2f);
            playerMove.move.y = Mathf.Clamp(playerMove.move.y, -2f, 2f);
            timer = 0;
        }
    }
}
