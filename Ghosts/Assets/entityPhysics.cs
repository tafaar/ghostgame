using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entityPhysics : MonoBehaviour
{
    Rigidbody2D rbParent;
    void Start()
    {
        rbParent = GetComponentInParent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            Debug.Log("enemy contact");
        }
    }
}
