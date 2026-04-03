using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    public float walkSpeed = 5f;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    public float jumpForce = 45f;
    public int jumpBufferFrames = 8;
    private int jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    public float coyoteTime = 0.1f;
    private int airJumpCounter = 0;
    [SerializeField] int maxAirJumps = 0;

    [Space(5)]
    [Header("Ground Check Settings")]
    public float groundCheckY = 0.2f;
    public float groundCheckX = 0.2f;
    public Transform groundCheckPoint;
    public LayerMask groundMask;

    [Space(5)]
    [Header("Dash Settings")]
    public float dashSpeed;
    public float dashTime;
    public float dashCooldown;
    public bool dashed;
    public bool canDash = true;
    public GameObject dashEffect;

    [Space(5)]
    [Header("Damage Effect")]
    public Color damageColor = Color.red; // Cor durante o dano
    public float blinkDuration = 0.1f; // Duração do piscar
    public int blinkCount = 5; // Número de piscadas

    [Space(5)]
    [Header("Fireball")]
    public GameObject fireballPrefab;
    public Transform shootPosition;
    public float shootCooldown;
    private float lastShootTime = 0;

    [Space(5)]
    [Header("Upgrades")]
    [SerializeField] List<UpgradeData> upgrades = new();


    private int playerHealth = 0;
    private float xAxis;
    private float gravity;
    private PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public HealthSystem health;

    public static PlayerController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<HealthSystem>();
        playerHealth = health.currentHealth;
        gravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();

        if (pState.dashing) return;

        Flip();
        Move();
        Jump();
        Shoot();
        StartDash();
        PauseGame();
        VerificarUpgrades();
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.TremerCamera();
        }
    }

    private void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HUDController.Instance.PausarJogo();
        }
    }

    private void Flip()
    {
        if (xAxis < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (xAxis > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Jump()
    {
        //Permite ao jogador parar o pulo no meio
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpForce);
                AudioManager.Instance.Play("Pulo");
            }

        }
        else if (airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            AudioManager.Instance.Play("Pulo");
            airJumpCounter++;
        }

        anim.SetBool("Jumping", !IsGrounded());
    }

    void Move()
    {
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        anim.SetBool("Walking", IsMoving());

    }
    
    void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && lastShootTime < Time.time)
        {
            Vector3 position = shootPosition.position;
            Quaternion rotation = fireballPrefab.transform.rotation;
            if (spriteRenderer.flipX)
            {
                //position.y = shootPosition.position.y * -1;
                rotation.eulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                //position.y = shootPosition.position.y * 1;
                rotation.eulerAngles = new Vector3(0, 0, 0);
            }


            Instantiate(fireballPrefab, position, rotation);
            AudioManager.Instance.Play("Fireball");
            lastShootTime = Time.time + shootCooldown;
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(nameof(Dash));
            dashed = true;
        }

        if (IsGrounded())// Para permitir que o jogador só de 1 dash no ar
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        AudioManager.Instance.Play("Attack");
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;

        float direction;
        if (spriteRenderer.flipX)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        rb.velocity = new Vector2(dashSpeed * direction, 0);

        if (IsGrounded())
        {
            InstantiateDashEffect(direction);
        }

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void InstantiateDashEffect(float direction)
    {
        GameObject instantiatedDashEffect = Instantiate(dashEffect);

        Vector3 currentPosition = instantiatedDashEffect.transform.position;
        SpriteRenderer spriteDashEffect = instantiatedDashEffect.GetComponent<SpriteRenderer>();

        if (direction < 0)
        {
            spriteDashEffect.flipX = true;
            currentPosition.x *= direction; // Inverte o valor de X
        }
        else
        {
            spriteDashEffect.flipX = false;
        }

        //Adiciona a nova posição e faz o objeto ser filho do transform do jogador
        instantiatedDashEffect.transform.position = this.transform.position + currentPosition;
        instantiatedDashEffect.transform.parent = this.transform;
    }

    bool IsMoving()
    {
        if (rb.velocity.x != 0 && IsGrounded())
        {
            Flip();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsGrounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundMask)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundMask)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundMask))
        {
            pState.jumping = false;
            return true;
        }
        else
        {
            pState.jumping = true;
            return false;
        }
    }

    void UpdateJumpVariables()
    {
        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }

    // Método para dar dano no jogador
    public void TakeDamage(int damage = 2)
    {
        health.TakeDamage(damage);
        HUDController.Instance.UpdateHearts(); // Atualiza o HUD dos corações
        AudioManager.Instance.Play("Dano");
        GameManager.Instance.TremerCamera();
        StartCoroutine(BlinkCoroutine());

        if (health.currentHealth <= 0)
        {
            Die();
        }
    }

    // Método para curar o jogador
    public void Heal(int healAmount = 2)
    {
        health.Heal(healAmount);
        HUDController.Instance.UpdateHearts(); // Atualiza o HUD dos corações
    }

    private void Die()
    {
        Debug.Log("No céu tem pão?");
    }

    public void MoveToCheckPoint(Transform checkPointPosition)
    {
        transform.position = checkPointPosition.position;
    }


    private IEnumerator BlinkCoroutine()//Piscar o jogador ao tomar dano
    {
        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }
    }


    public void AdicionarUpgrade(UpgradeData upgrade)
    {
        upgrades.Add(upgrade);
    }

    private void VerificarUpgrades()
    {
        if (upgrades.Count == 0) return;

        foreach (UpgradeData upgrade in upgrades)
        {
            if (upgrade.ativado == false)
            {
                if (upgrade.nomeUpgrade == "Pulo Duplo")
                {
                    EnableDoubleJump();
                    upgrade.ativado = true;
                }
            }
        }
    }

    public void EnableDoubleJump()
    {
        maxAirJumps = 1;
    }
}
