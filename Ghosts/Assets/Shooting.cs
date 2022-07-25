using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject player;
    public GameObject bulletPrefab;

    public PlayerData playerData;

    public bool canShoot = true;
    public bool shot;
    public bool shootOverride = false;

    public List<Bullet> liveBullets;

    public float time;
    float transitionTime;
    public float angle;

    public float angleModifier;

    Rigidbody2D playerRb;

    Vector2 shootDir;

    private void Start()
    {
        playerData = PlayerMove.instance.playerDataUpdated;
        liveBullets = new List<Bullet>();
        playerRb = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 lookDir = PlayerMove.instance.aim;
        angle = Mathf.Atan2(lookDir.y, lookDir.x);
        shootDir = new Vector2(Mathf.Cos(angle + angleModifier), Mathf.Sin(angle + angleModifier));
        time += Time.deltaTime;

        if (PlayerMove.instance.shooting == true)
        {
            playerData = PlayerMove.instance.playerDataUpdated;
            StartCoroutine(Shoot(shootDir));
        }

        if(transitionTime > 0)
        {
            transitionTime += Time.deltaTime;

            if(transitionTime >= 2f)
            {
                canShoot = true;
                transitionTime = 0;
            }
        }
    }

    public void ShootAlt(Vector2 dir, bool yn)
    {
        Debug.Log("Shoot alt");
        StartCoroutine(Shoot(dir, yn));
    }

    public IEnumerator Shoot(Vector2 dir, bool forceShoot = false)
    {
        if (canShoot)
        {
            if (time >= playerData.shootTime || forceShoot == true)
            {
                shot = true;

                if (shootOverride == false)
                {

                    GameObject bulletInstance = Instantiate(bulletPrefab, player.transform.position, Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90f));
                    Rigidbody2D rb = bulletInstance.GetComponent<Rigidbody2D>();

                    Bullet bullet = bulletInstance.GetComponent<Bullet>();

                    liveBullets.Add(bullet);

                    bullet.bulletDuration = playerData.bulletDuration;
                    bullet.bulletDamage = playerData.bulletDamage;

                    if (bullet.initialized == false)
                    {
                        bullet.pierce = playerData.piercePower;
                    }
                    bullet.initialized = true;

                    rb.AddForce(dir * playerData.bulletForce, ForceMode2D.Impulse);

                    rb.AddForce(playerRb.velocity.x * Vector2.right * 0.2f, ForceMode2D.Impulse);
                    rb.AddForce(playerRb.velocity.y * Vector2.up * 0.2f, ForceMode2D.Impulse);

                }
                time = 0;
            }
        }

        yield return new WaitForEndOfFrame();

        shot = false;
    }

    public void RemoveAllBullets()
    {
        foreach(Bullet bullet in liveBullets.ToArray())
        {
            if (bullet.gameObject != null)
            {
                bullet.Remove();
            }
        }
        liveBullets.Clear();
    }

    public void RoomTransition()
    {
        canShoot = false;
        transitionTime += 1;
    }

    public void RemoveBullet(Bullet bullet)
    {
        liveBullets.Remove(bullet);
    }

}
