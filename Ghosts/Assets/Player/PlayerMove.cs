using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.Universal;

public class PlayerMove : MonoBehaviour
{
    public static PlayerControls controls;
    public static PlayerMove instance;
    public Rigidbody2D rb;
    public GameObject selector;

    public PlayerData playerDataOriginal;
    public PlayerData playerDataUpdated;
    public Slider healthBar;
    public Slider uiStamina;
    public TextMeshProUGUI uiMoney;

    public SpriteRenderer spriteRenderer;

    public PassiveItem itemTest;

    public float maxHealth;
    public float currentHealth;

    public int maxStamina;
    public int currentStamina;
    public float rollForce;

    public float rollTimer;
    public bool rollTriggered;
    public float rechargeTimer;

    public Camera cam;
    public bool camFollow = false;

    public float pushRadius;

    public bool canMove = true;
    [HideInInspector] public bool shooting;

    bool held;
    bool damageInvuln = false;
    float invulnTimer;
    float transitionTime;

    public Vector2 move;
    Vector2 storedDir;
    public Vector2 look;
    public Vector2 aim;

    float dirBuffer;

    public bool dodging;

    public GameObject flashlight;
    public bool flashlightOn = true;

    void Awake()
    {
        instance = this;

        playerDataUpdated = playerDataOriginal;
        playerDataUpdated.passiveItems = new List<PassiveItem>();

        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        controls.Player.Aim.performed += ctx => aim = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled += ctx => aim = Vector2.zero;

        controls.Player.Aim.performed += ctx => shooting = true;
        controls.Player.Aim.canceled += ctx => shooting = false;
    }

    private void Start()
    {
        maxHealth = playerDataUpdated.health;

        currentHealth = playerDataUpdated.health;

        maxStamina = playerDataUpdated.stamina;

        currentStamina = playerDataUpdated.stamina;

        cam.transform.parent = null;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    private void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y);

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Damage: " + playerDataUpdated.bulletDamage + " Fire Rate: " + playerDataUpdated.shootTime);
        }

        if (controls.Player.Flashlight.triggered)
        {
            if (flashlightOn)
            {
                flashlight.GetComponent<Light2D>().intensity = 0;
                flashlightOn = false;
            }
            else
            {
                flashlight.GetComponent<Light2D>().intensity = 0.7f;
                flashlightOn = true;
            }
        }

        foreach (PassiveItem item in playerDataUpdated.passiveItems)
        {
            if (item.hasAttributes)
            {
                item.Passive(this);
                Debug.Log("Applying passive for " + item.name);
            }
        }

        if (healthBar != null) {

            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;

        }

        if (uiMoney != null)
        {
            uiMoney.text = "$" + playerDataUpdated.coins.ToString();
        }

        if (uiStamina != null)
        {
            uiStamina.maxValue = maxStamina;
            uiStamina.value = currentStamina;
        }

        float lookAngle = Mathf.Atan2(move.y, move.x);
        Vector2 lookDir = new Vector2(Mathf.Cos(lookAngle), Mathf.Sin(lookAngle));

        if (damageInvuln)
        {
            invulnTimer += Time.deltaTime;

            if(invulnTimer >= 1f)
            {
                damageInvuln = false;
            }
        }
        else
        {
            invulnTimer = 0;
        }

        if (controls.Player.Move.triggered)
        {
            if (held == false)
            {
                dirBuffer = 0;
                storedDir = lookDir;
                look = lookDir;
            }
        }
        if (controls.Player.Move.IsPressed())
        {
            held = true;

            dirBuffer += Time.deltaTime;
            if(dirBuffer >= 0.05)
            {
                look = lookDir;
                storedDir = look;
                dirBuffer = 0;
            }
        }
        if (controls.Player.Move.WasReleasedThisFrame())
        {
            look = storedDir;
            held = false;
        }

        if (rollTriggered)
        {
            rollTimer += Time.deltaTime;

            if(rollTimer >= 0.3f)
            {
                dodging = false;
            }

            if(rollTimer >= 1f)
            {
                rollTriggered = false;
                rollTimer = 0;
            }
        }

        if(rollTimer == 0)
        {
            rechargeTimer += Time.deltaTime;

            if(rechargeTimer >= 2f)
            {
                if(currentStamina < maxStamina)
                {
                    currentStamina++;

                    if (currentStamina < maxStamina) rechargeTimer = 1.2f; else rechargeTimer = 0;
                }
            }
        }
        else
        {
            rechargeTimer = 0;
        }

        if (controls.Player.Roll.triggered)
        {
            if (rollTriggered == false)
            {
                Debug.Log("Rolling");
                Roll();
            }
        }

        if (!canMove)
        {
            move = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        

        if (camFollow)
        {
            cam.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10f);
        }

        Collider2D[] enemySpace = Physics2D.OverlapCircleAll(gameObject.transform.position, pushRadius);
        foreach (Collider2D enemyCollider in enemySpace)
        {
            if (enemyCollider.gameObject != gameObject &&
                    (enemyCollider.gameObject.layer == LayerMask.NameToLayer("Enemies") ||
                     enemyCollider.gameObject.layer == LayerMask.NameToLayer("Player")))
            {
                float distance = Vector2.Distance(gameObject.transform.position, enemyCollider.transform.position);
                rb.AddForce(playerDataUpdated.speed * (gameObject.transform.position - enemyCollider.transform.position).normalized * Mathf.Pow(1 + pushRadius - distance, 2));
                if (gameObject.transform.position == enemyCollider.transform.position)
                {
                    rb.AddForce(2 * playerDataUpdated.speed * new Vector2(Random.Range(-1, 1), Random.Range(-1, 1)));
                }
            }
        }

        if (canMove)
        {

            #region Movement

            // X Movement

            float targetSpeedX = move.x * playerDataUpdated.speed;

            float speedDifX = targetSpeedX - rb.velocity.x;

            float accelRateX = (Mathf.Abs(targetSpeedX) > 0.01f) ? playerDataUpdated.acceleration : playerDataUpdated.deceleration;

            float movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, playerDataUpdated.velPower) * Mathf.Sign(speedDifX);

            // Y movement

            float targetSpeedY = move.y * playerDataUpdated.speed;

            float speedDifY = targetSpeedY - rb.velocity.y;

            float accelRateY = (Mathf.Abs(targetSpeedY) > 0.01f) ? playerDataUpdated.acceleration : playerDataUpdated.deceleration;

            float movementY = Mathf.Pow(Mathf.Abs(speedDifY) * accelRateY, playerDataUpdated.velPower) * Mathf.Sign(speedDifY);

            rb.AddForce(movementX * Vector2.right);
            rb.AddForce(movementY * Vector2.up);

            #endregion Movement


            Vector2 pos = gameObject.transform.position;
            selector.transform.position = pos + new Vector2(look.x, look.y);

        }
    }

    public void TakeDamage(float damage, Vector2 directionSource, float knockback, bool angledDir = false)
    {
        if (damageInvuln == false && dodging == false)
        {
            Debug.Log("Took " + damage + " damage");

            currentHealth -= damage;

            Vector2 pos = gameObject.transform.position;

            if (angledDir == false)
            {
                rb.velocity = knockback * (pos - directionSource).normalized;
            }
            else
            {
                rb.velocity = knockback * directionSource;
            }

            damageInvuln = true;
        }
    }

    public void Roll()
    {
        if (canMove)
        {
            if(currentStamina > 0)
            {
                dodging = true;
                rb.velocity = move.normalized * rollForce * Mathf.Clamp(playerDataUpdated.speed/5, 1, Mathf.Infinity);
                currentStamina--;
                rollTriggered = true;
            }
        }
    }

    public void Heal(float health)
    {
        currentHealth += health;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void RoomTransition()
    {
        StartCoroutine(RoomSwitch());
    }

    IEnumerator RoomSwitch()
    {
        canMove = false;
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.6f);
        canMove = true;
    }

    public void UpdateStats()
    {
        playerDataUpdated = playerDataUpdated.UpdateStats(playerDataOriginal);
    }
}
