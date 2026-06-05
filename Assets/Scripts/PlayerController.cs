using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class PlayerController : MonoBehaviour
{
    [Header("Abissal Attack")]
public bool podeAbissal = false;
public GameObject abissalPrefab;
public Transform shootPositionAbissal;
public float abissalDelay = 1f;
public float abissalCooldown = 1.5f;

private float lastAbissalTime;
private bool isAbissalAttacking;

    [Header("Shoot Unlock")]
public bool podeShoot = false;

    [Header("Dash Unlock")]
public bool podeDash = false;

    private bool isWalkingSoundPlaying = false;

    private bool isAttacking = false;

    [Header("Horizontal Movement Settings")]
    public float walkSpeed = 5f;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    public float jumpForce = 45f;
    public int jumpBufferFrames = 8;
    private int jumpBufferCounter = 0;
    private float coyoteTimeCounter = 0;
    public float coyoteTime = 0.1f;


    [Space(5)]
    [Header("Ground Check Settings")]
    public float groundCheckY = 0.2f;
    public float groundCheckX = 0.2f;
    public Transform groundCheckPoint;
    public LayerMask groundMask;

    [Header("Physics Materials")]
    public PhysicsMaterial2D materialSemFriccao;
    public PhysicsMaterial2D materialAltaFriccao;

    private Collider2D col;

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
    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    private float lastAttackTime;

    [Space(5)]
    [Header("Upgrades")]
    [SerializeField] List<UpgradeData> upgrades = new();


    public float vida = 0;
    private float xAxis;
    private float gravity;
    private PlayerStateList pState;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public SistemaDeVida sistemaDeVida;

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
        sistemaDeVida = GetComponent<SistemaDeVida>();
        vida = sistemaDeVida.vidaAtual;
        gravity = rb.gravityScale;
        col = GetComponent<Collider2D>();
    }

    void Start()
{
    if (GameManager.Instance != null)
    {
        podeDash = GameManager.Instance.podeDash;
        podeShoot = GameManager.Instance.podeShoot;
    }
}

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpVariables();
        UseAbissal();

        if (pState.dashing) return;

        Flip();

        if (!isAttacking)
        {
            Move();
            Jump();
            Shoot();
            Attack();
        }

        pState.walking = IsMoving();
        
        PlayWalkingSound();
        StartDash();
        PauseGame();
        VerificarUpgrades();
        UpdateFrictionMaterial();
        
       
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
            MirrorChildren();
    }

    private void MirrorChildren()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;

            Quaternion newRotation = Quaternion.identity;

            if (spriteRenderer.flipX)
            {
                newRotation = Quaternion.Euler(180f * Vector3.up);
            }

            child.rotation = newRotation;
        }

    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Jump()
{
    // Permite ao jogador parar o pulo no meio
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

    anim.SetBool("Jumping", !IsGrounded());
}

    void Move()
    {
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);        
        anim.SetBool("Walking", pState.walking);
    }

    void PlayWalkingSound(){
        if(pState.walking && !isWalkingSoundPlaying){
            AudioManager.Instance.Play("Walk");
            isWalkingSoundPlaying = true;
        }else if (!pState.walking){
            AudioManager.Instance.Stop("Walk");
            isWalkingSoundPlaying = false;
        }
        
    }
    
    void Shoot()
    {
        if (!podeShoot) return;

        if (Input.GetButtonDown("Fire2") && lastShootTime < Time.time && !isAttacking)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("adaga");

            // termina o ataque normal
            Invoke(nameof(EndAttack), 0.7f);

            // ATRASA o disparo
            Invoke(nameof(SpawnFireball), 0.25f);

            lastShootTime = Time.time + shootCooldown;
        }
    }

    void SpawnFireball()
    {
        Vector3 position = shootPosition.position;
        Quaternion rotation = fireballPrefab.transform.rotation;

        if (spriteRenderer.flipX)
            {
                rotation.eulerAngles = new Vector3(0, 180, 0);
            }
        else
            {
                rotation.eulerAngles = new Vector3(0, 0, 0);
            }

        Instantiate(fireballPrefab, position, rotation);
        AudioManager.Instance.Play("Fireball");
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime)
        {
            if (pState.dashing) return; // opcional, evita atacar durante dash

            isAttacking = true;
            rb.velocity = Vector2.zero;
            lastAttackTime = Time.time + attackCooldown;

            anim.SetTrigger("Attack");
            AudioManager.Instance.Play("Attack");
            Invoke(nameof(EndAttack), 0.7f);
        }
    }


    void StartDash()
    {
        if (!podeDash) return;

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
        AudioManager.Instance.Play("Dash");
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
        if (rb.velocity.x != 0 && IsGrounded() && !isAttacking)
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
    public void AplicarDano(float damage = 2)
    {
        sistemaDeVida.AplicarDano(damage);
        HUDController.Instance.AtualizarVida(); // Atualiza o HUD dos corações
        AudioManager.Instance.Play("Dano");
        if (CinemachineShake.Instance != null)
        {
            CinemachineShake.Instance.Shake(8f, 15f, 0.5f);
        }
        Debug.Log("Tomou dano!");
        if (sistemaDeVida.vidaAtual <= 0)
        {
            Die();
        }
    }

    // Método para curar o jogador
    /*public void Heal(int healAmount = 2)
    {
        vida.Heal(healAmount);
        HUDController.Instance.UpdateHearts(); // Atualiza o HUD dos corações
    }*/

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
        isBlinking = true;

        Color originalColor = spriteRenderer.color;

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.color = damageColor;
            yield return new WaitForSeconds(blinkDuration);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(blinkDuration);
        }

        spriteRenderer.color = originalColor;
        isBlinking = false;
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
                // futuros upgrades
            }
        }
    }

    private bool isBlinking = false;
    public void FeedbackDano()
    {
        if (!isBlinking)
            StartCoroutine(BlinkCoroutine());
    }

    void UpdateFrictionMaterial()
    {
        // Em movimento no chão → baixa fricção
        if (Mathf.Abs(xAxis) > 0.01f && IsGrounded())
        {
            if (col.sharedMaterial != materialSemFriccao)
                col.sharedMaterial = materialSemFriccao;
        }
        // Parado no chão → alta fricção (segura na rampa)
        else if (IsGrounded())
        {
            if (col.sharedMaterial != materialAltaFriccao)
                col.sharedMaterial = materialAltaFriccao;
        }
        // No ar → sem fricção (evitar grudar em paredes)
        else
        {
            if (col.sharedMaterial != materialSemFriccao)
                col.sharedMaterial = materialSemFriccao;
        }
    }

    void UseAbissal()
{
    if (!podeAbissal) return;
    if (isAbissalAttacking) return;
    if (Time.time < lastAbissalTime) return;
    if (!Input.GetKeyDown(KeyCode.Q)) return;

    StartCoroutine(AbissalRoutine());
}

IEnumerator AbissalRoutine()
{
    isAbissalAttacking = true;
    lastAbissalTime = Time.time + abissalCooldown;

    rb.velocity = Vector2.zero;

    anim.Play("Player_Abissal", 0, 0f);

    // pega duração REAL da animação
    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
    float animDuration = stateInfo.length;

    yield return new WaitForSeconds(abissalDelay);

    SpawnAbissalProjectile();

    yield return new WaitForSeconds(0.2f);

    isAbissalAttacking = false;
}

void SpawnAbissalProjectile()
{
    if (abissalPrefab == null || shootPositionAbissal == null)
        return;

    Vector3 position = shootPositionAbissal.position;
    Quaternion rotation = abissalPrefab.transform.rotation;

    if (spriteRenderer.flipX)
        rotation.eulerAngles = new Vector3(0, 180, 0);
    else
        rotation.eulerAngles = new Vector3(0, 0, 0);

    Instantiate(abissalPrefab, position, rotation);
}

public void UnlockAbissal()
{
    podeAbissal = true;
}
}
