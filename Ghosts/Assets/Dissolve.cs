using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    Material material;

    public float timeModifier = 1;
    public float offset;
    public Color color;

    public float opaqueMod;
    public bool alive = true;
    public float fade;
    float timer;

    void Start()
    {
        offset = Random.Range(0, Mathf.PI);
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            if (opaqueMod > 0)
            {
                opaqueMod -= Time.deltaTime;
            }

            timer += Time.deltaTime;
            fade = Mathf.Cos(timer * timeModifier + offset);
        }

        material.SetFloat("_OpaqueMod", opaqueMod);
        material.SetFloat("_Fade", fade);
        material.SetColor("_Color", color);

    }
}
