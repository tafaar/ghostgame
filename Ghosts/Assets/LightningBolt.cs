using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightningBolt : MonoBehaviour
{
    public void RemoveLightning()
    {
        Destroy(gameObject);
    }
}
