using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    public static BulletManager instance;

    public GameObject laserBeam;
    public GameObject defaultBullet;
    public GameObject cheeseWheel;
    public GameObject cheeseSlice;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("Multiple instances of Bullet Manager found");
            Destroy(instance);
        }

        instance = this;
    }
}
