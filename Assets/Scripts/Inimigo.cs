using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float visionRange = 7f;
    public float visionHeight = 2f;
    public float knockbackForce = 5f;

    [Header("Ataque")]
    public int danoAtaque = 10;
    public float tempoEntreAtaques = 0.7f;      // frequência do ataque
    private bool playerNoAlcance = false;
    private bool atacando = false;

    [SerializeField] private bool movingRight = false;

    private bool vivo = true;
    private bool isKnockBacked = false;
    private bool vendoPlayer = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private Transform player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        SetDirecaoInicial();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isKnockBacked || !vivo || playerNoAlcance) return;
        if (player == null) return;

        DetectarPlayer();

        if (vendoPlayer)
            PerseguirPlayer();
        else
            Move();
    }

    // -----------------------------------
    // DETECÇÃO DE PLAYER
    // -----------------------------------
    void DetectarPlayer()
    {
        float distancia = Vector2.Distance(transform.position, player.position);

        vendoPlayer = distancia <= visionRange &&
                      Mathf.Abs(player.position.y - transform.position.y) <= visionHeight;
    }

    // -----------------------------------
    // PERSEGUIÇÃO
    // -----------------------------------
    void PerseguirPlayer()
    {
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;

        movingRight = direction > 0;

        rb.velocity = new Vector2(direction * chaseSpeed, rb.velocity.y);

        spriteRenderer.flipX = movingRight == false;

        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    // -----------------------------------
    // MOVIMENTO NORMAL
    // -----------------------------------
    void Move()
    {
        float direction = movingRight ? 1f : -1f;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        spriteRenderer.flipX = movingRight == false;
        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    private void SetDirecaoInicial()
    {
        float initialDir = movingRight ? 1f : -1f;
        spriteRenderer.flipX = movingRight == false;
        rb.velocity = new Vector2(initialDir * moveSpeed, rb.velocity.y);
    }

    // ========================================================
    //    ATAQUE INFINITO ENQUANTO PLAYER ESTIVER COLIDINDO
    // ========================================================

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNoAlcance = true;
            anim.SetBool("Atacando", true);

            if (!atacando)
                StartCoroutine(AtaqueContinuo(collision.gameObject));

            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                if (collision.gameObject.CompareTag("Inimigo")) continue;

                movingRight = !movingRight;
                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNoAlcance = false;
            atacando = false;
            anim.SetBool("Atacando", false);
        }
    }

    IEnumerator AtaqueContinuo(GameObject playerObj)
    {
        atacando = true;

        SistemaDeVida vida = playerObj.GetComponent<SistemaDeVida>();

        while (playerNoAlcance && vivo)
        {
            anim.SetTrigger("Ataque");

            if (vida != null)
                vida.AplicarDano(danoAtaque);

            yield return new WaitForSeconds(tempoEntreAtaques);
        }

        atacando = false;
    }

    // ========================================================
    //            EFEITOS DE DANO / MORTE
    // ========================================================
    public void EfeitoDeRecuo()
    {
        isKnockBacked = true;

        float knockbackDirection = movingRight ? -1f : 1f;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        rb.AddForce(new Vector2(knockbackDirection * knockbackForce, 0f), ForceMode2D.Impulse);

        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f);
        isKnockBacked = false;
    }

    public void EfeitoDePiscar()
    {
        StartCoroutine(Piscar());
    }

    IEnumerator Piscar()
    {
        Color original = spriteRenderer.color;
        Color transparente = new Color(original.r, original.g, original.b, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = transparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = original;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void AnimacaoDeDano()
    {
        anim.SetTrigger("Machucado");
        StartCoroutine(ResetMachucado());
    }

    IEnumerator ResetMachucado()
    {
        yield return new WaitForSeconds(0.5f);
        anim.ResetTrigger("Machucado");
    }

    internal void AnimacaoDeMorte()
    {
        vivo = false;
        rb.isKinematic = true;
        col.enabled = false;

        anim.SetBool("Vivo", vivo);
        EfeitoDePiscar();

        Destroy(gameObject, 3f);
    }
}
