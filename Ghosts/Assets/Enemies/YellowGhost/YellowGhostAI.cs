using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class YellowGhostAI : EnemyAI
{
    [SerializeField] GameObject lightningBolt;

    Vector2 moveDir;

    [Header("Teleport Debug")]
    [SerializeField] float teleportTimer;

    public class MoveArea
    {
        public Vector2 bottomLeft;
        public Vector2 topRight;

        public MoveArea(Vector2 point1, Vector2 point2)
        {
            bottomLeft = point1;
            topRight = point2;
        }
    }

    MoveArea moveArea;

    private void OnBecameVisible()
    {
        initialized = true;
    }

    void Start()
    {
        moveArea = new MoveArea(new Vector2(-2, -2), new Vector2(2, 2));
        EnemyStart();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (CheckShoot(collision))
        {
            Teleport();
        }

        if (collision.CompareTag("Rooms"))
        {
            Vector2 pos = collision.gameObject.transform.position;
            moveArea.bottomLeft = pos - new Vector2(7.5f, 3.5f);
            moveArea.topRight = pos + new Vector2(7.5f, 3.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnemyUpdate();

        teleportTimer += Time.deltaTime;

        if (teleportTimer >= 2f)
        {
            Teleport();
        }

        if (!alive)
        {
            Die(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Move(Vector2.zero);
    }

    void Teleport()
    {
        float teleportX = Random.Range(moveArea.bottomLeft.x, moveArea.topRight.x);
        float teleportY = Random.Range(moveArea.bottomLeft.y, moveArea.topRight.y);
        Vector2 newPos = new Vector2(teleportX, teleportY);

        Vector2 teleportDir = newPos - new Vector2(transform.position.x, transform.position.y);
        float angle = Mathf.Atan2(teleportDir.y, teleportDir.x) * Mathf.Rad2Deg;

        GameObject lb = Instantiate(lightningBolt, Vector3.Lerp(transform.position, new Vector3(teleportX, teleportY, 0), 0.5f), Quaternion.Euler(new Vector3(0, 0, angle)));
        lb.transform.localScale = new Vector3(Vector2.Distance(transform.position, new Vector3(teleportX, teleportY)) / 8, 1, 1);
        transform.position = newPos;

        teleportTimer = 0;
    }
}
