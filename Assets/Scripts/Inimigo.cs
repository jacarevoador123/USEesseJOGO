using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform pontoA;
    public Transform pontoB;
    public float toleranciaPonto = 0.2f;
    
    private Transform alvoPatrulha;

    [Header("Configurações")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float visionRange = 7f;
    public float visionHeight = 2f;
    public float knockbackForce = 5f;

    [Header("Ataque")]
    public int danoAtaque = 10;
    public float tempoEntreAtaques = 2f;      // frequência do ataque
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

        alvoPatrulha = pontoA; // começa indo pro ponto A
    }

    void Update()
    {
        if (isKnockBacked || !vivo || atacando) return;
        if (player == null) return;

        DetectarPlayer();

        if (vendoPlayer)
        {
            PerseguirPlayer();
        }
        else
        {
            Move();
        }
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
        MirrorChildren();

        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    // -----------------------------------
    // MOVIMENTO NORMAL
    // -----------------------------------
    void Move()
    {
        if (pontoA == null || pontoB == null) return;

        Vector2 pos = transform.position;
        float destinoX = alvoPatrulha.position.x;

        // move só no eixo X
        float novoX = Mathf.MoveTowards(pos.x, destinoX, moveSpeed * Time.deltaTime);

        transform.position = new Vector2(novoX, pos.y);

        float direction = destinoX - pos.x;

        if (Mathf.Abs(direction) > 0.05f)
            movingRight = direction > 0;

        spriteRenderer.flipX = !movingRight;
        MirrorChildren();

        anim.SetFloat("Velocidade", Mathf.Abs(direction));

        // chegou no ponto (considera só X)
        if (Mathf.Abs(transform.position.x - destinoX) < toleranciaPonto)
        {
            transform.position = new Vector2(destinoX, transform.position.y);

            alvoPatrulha = (alvoPatrulha == pontoA) ? pontoB : pontoA;
        }
    }

    private void SetDirecaoInicial()
    {
        float initialDir = movingRight ? 1f : -1f;
        spriteRenderer.flipX = movingRight == false;
        MirrorChildren();

        rb.velocity = Vector2.zero;
    }

    // ========================================================
    //    ATAQUE INFINITO ENQUANTO PLAYER ESTIVER COLIDINDO
    // ========================================================

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerNoAlcance = true;

            rb.velocity = Vector2.zero;

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
            
        }
    }

    IEnumerator AtaqueContinuo(GameObject playerObj)
    {
        atacando = true;

        while (vivo)
        {
            rb.velocity = Vector2.zero;

            if (playerNoAlcance)
            {
                //Travar o rigidbody em X enquanto ele tá atacando]
                anim.SetTrigger("Ataque");
                //colocar uma segunda opção de grito pelo amor de deus
                AudioManager.Instance.Play("GritoDeGuerra");
            }

            yield return new WaitForSeconds(tempoEntreAtaques);

            if (!playerNoAlcance)
                break;
        }

        //destrava o rigidbody em x
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

        AudioManager.Instance.Play("Morte");

        EfeitoDePiscar();

        Destroy(gameObject, 3f);
    }

    public void TocarAttack()
    {
        //colocar mais um som aqui ou um random pq tá deixando maluco
        AudioManager.Instance.Play("Attack");
    }

    public void AplicarDanoNoPlayer()
    {
        if (!playerNoAlcance) return; // evita dano fantasma

        SistemaDeVida vida = player.GetComponent<SistemaDeVida>();

        if (vida != null)
        {
            vida.AplicarDano(danoAtaque);
            AudioManager.Instance.Play("Dano");
        }
    }
    [SerializeField] private GameObject HitboxAtaqueInimigo;
    public void AtivarHitbox()
    {
        HitboxAtaqueInimigo.SetActive(true);
    }
    public void DesativarHitbox()
    {
        HitboxAtaqueInimigo.SetActive(false);
    }

    private void MirrorChildren()
    {
        foreach (var child in transform.GetComponentsInChildren<Transform>())
        {
            if (child == transform) continue;

            Quaternion newRotation = Quaternion.identity;

            if (spriteRenderer.flipX)
            {
                newRotation = Quaternion.Euler(0f, 180f, 0f);
            }
            
            child.rotation = newRotation;
        }
    }
}
