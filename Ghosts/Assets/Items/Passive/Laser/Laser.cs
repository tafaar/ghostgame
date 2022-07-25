using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attributed Item/Laser")]
public class Laser : PassiveItem
{
    public LayerMask mask;
    public GameObject laserBeam;

    public override void Passive(PlayerMove playerMove)
    {
        Vector2 pos = playerMove.transform.position;

        Debug.DrawRay(playerMove.transform.position, playerMove.aim, Color.blue);

        if (playerMove.GetComponent<Shooting>().shootOverride == false)
        {
            playerMove.GetComponent<Shooting>().shootOverride = true;
        }

        if (playerMove.GetComponent<Shooting>().shot)
        {
            Debug.Log("Shooting laser");

            Vector2 impactPoint = new Vector2();

            RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(playerMove.transform.position, playerMove.aim, 50f);
            foreach(RaycastHit2D raycast in raycastHit2D)
            {
                if(raycast.collider.gameObject.layer == LayerMask.NameToLayer("Walls")){
                    impactPoint = raycast.point;
                }
            }

            float hitDistance = Vector2.Distance(playerMove.transform.position, impactPoint);
            GameObject bulletInstance = Instantiate(laserBeam, playerMove.transform.position, Quaternion.Euler(0, 0, playerMove.GetComponent<Shooting>().angle * Mathf.Rad2Deg));
            bulletInstance.transform.position = playerMove.transform.position;

            Bullet bullet = bulletInstance.GetComponent<Bullet>();
            bulletInstance.transform.localScale = new Vector3(hitDistance / 16, bulletInstance.transform.localScale.y, bulletInstance.transform.localScale.z);
            bullet.bulletDamage = playerMove.GetComponent<Shooting>().playerData.bulletDamage;

            playerMove.GetComponent<Shooting>().liveBullets.Add(bullet);

            if (bullet.initialized == false)
            {
                bullet.pierce = 999;
                bullet.scaleSize = false;
            }

            bullet.initialized = true;
        }
    }
}
