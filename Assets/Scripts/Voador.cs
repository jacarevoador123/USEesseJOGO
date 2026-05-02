using System.Collections;
using UnityEngine;

public class Voador : MonoBehaviour
{
    [Header("Patrulha A -> B")]
    public Transform pontoA;
    public Transform pontoB;
    public float toleranciaPonto = 0.2f;
    
    private Transform alvoPatrulha;

    [Header("Distâncias")]
    public float birdDistanciaRasante = 3f; // distância mínima pra atacar

    [Header("Configurações Básicas - Bird")]
    public float birdMoveSpeed = 2f;
    public int birdMaxHealth = 2;
    public float birdKnockbackForce = 5f;

    [Header("Rasante / Perseguição - Bird")]
    public float birdDistanciaVer = 8f;
    public float birdVelocidadeRasante = 6f;
    public float birdAlturaRasante = -3f;
    public float birdTempoEntreRasantes = 1.2f;

    private bool birdPodeRasar = true;
    private bool birdVendoPlayer = false;
    private bool birdFazendoRasante = false;

    [SerializeField] private bool birdMovingRight = false;
    private bool birdVivo = true;
    private bool birdIsKnockBacked = false;

    private Transform alvoPlayer;
    private Animator birdAnim;
    private Rigidbody2D birdRb;
    private SpriteRenderer birdSprite;
    private Collider2D birdCol;

    void Awake()
    {
        birdRb = GetComponent<Rigidbody2D>();
        birdAnim = GetComponent<Animator>();
        birdSprite = GetComponent<SpriteRenderer>();
        birdCol = GetComponent<Collider2D>();
    }

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) alvoPlayer = p.transform;
        Bird_SetDirecaoInicial();
        alvoPatrulha = pontoA;
    }

    void Update()
    {
        if (!birdVivo || birdIsKnockBacked) return;
        if (alvoPlayer == null) return;

        Bird_DetectarPlayer();

        float dist = Vector2.Distance(transform.position, alvoPlayer.position);

        if (birdVendoPlayer && birdPodeRasar && !birdFazendoRasante && dist <= birdDistanciaRasante)
        {
            StartCoroutine(Bird_RasanteCoroutine());
        }

        if (!birdFazendoRasante)
        {
            if (birdVendoPlayer)
                Bird_SeguirPlayer();
            else
                Bird_Patrulhar();
        }
    }

    void Bird_Patrulhar()
    {
        if (pontoA == null || pontoB == null) return;

        float dist = Vector2.Distance(transform.position, alvoPatrulha.position);

        if (dist < toleranciaPonto)
        {
            alvoPatrulha = (alvoPatrulha == pontoA) ? pontoB : pontoA;
        }

        float dir = Mathf.Sign(alvoPatrulha.position.x - transform.position.x);

        birdMovingRight = dir > 0;

        birdRb.velocity = new Vector2(dir * birdMoveSpeed, birdRb.velocity.y);

        birdSprite.flipX = !birdMovingRight;

        birdAnim.SetFloat("Bird_Velocidade", Mathf.Abs(birdRb.velocity.x));
    }

    void Bird_SeguirPlayer()
    {
        float dir = Mathf.Sign(alvoPlayer.position.x - transform.position.x);

        birdMovingRight = dir > 0;

        birdRb.velocity = new Vector2(dir * birdMoveSpeed, birdRb.velocity.y);

        birdSprite.flipX = !birdMovingRight;

        birdAnim.SetFloat("Bird_Velocidade", Mathf.Abs(birdRb.velocity.x));
    }

    // Movimento normal
    void Bird_AndarNormal()
    {
        float dir = birdMovingRight ? 1f : -1f;
        birdRb.velocity = new Vector2(dir * birdMoveSpeed, birdRb.velocity.y);

        birdSprite.flipX = !birdMovingRight; // flip consistente

        birdAnim.SetFloat("Bird_Velocidade", Mathf.Abs(birdRb.velocity.x));
    }

    // Detecta player por distância
    void Bird_DetectarPlayer()
    {
        float dist = Vector2.Distance(transform.position, alvoPlayer.position);
        birdVendoPlayer = dist <= birdDistanciaVer;
    }

    // Rasante único do pássaro (coroutine)
    IEnumerator Bird_RasanteCoroutine()
    {
    birdFazendoRasante = true;
    AudioManager.Instance.Play("RasanteVoador");
    birdPodeRasar = false;

    birdAnim.SetBool("Bird_Rasante", true);

    // direção inicial do rasante
    birdMovingRight = alvoPlayer.position.x > transform.position.x;

    // aplica o flip para a direção do rasante
    birdSprite.flipX = !birdMovingRight;

    float hDir = birdMovingRight ? 1f : -1f;

    // guarda a altura original
    float alturaInicial = transform.position.y;


    // =========================
    //   FASE 1: RASANTE (DESCIDA)
    // =========================
    float tempoRasante = 0.7f;
    while (tempoRasante > 0f)
    {
        // mantém o flip correto durante a descida
        birdSprite.flipX = !birdMovingRight;

        birdRb.velocity = new Vector2(
            hDir * birdVelocidadeRasante,
            birdAlturaRasante * birdVelocidadeRasante
        );

        tempoRasante -= Time.deltaTime;
        yield return null;
    }

    birdAnim.SetBool("Bird_Rasante", false);


    // =========================
    //   FASE 2: SUBINDO DE VOLTA
    // =========================
    while (transform.position.y < alturaInicial - 0.2f)
    {
        // mantém mesma direção do rasante
        birdSprite.flipX = !birdMovingRight;

        birdRb.velocity = new Vector2(
            hDir * (birdMoveSpeed * 0.7f),
            +Mathf.Abs(birdAlturaRasante) * birdMoveSpeed
        );

        yield return null;
    }


    // volta exatamente à altura inicial
    birdRb.velocity = Vector2.zero;
    transform.position = new Vector3(transform.position.x, alturaInicial, transform.position.z);

    birdFazendoRasante = false;


    // =========================
    //   FASE 3: COOLDOWN
    // =========================
    yield return new WaitForSeconds(birdTempoEntreRasantes);
    birdPodeRasar = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!birdVivo) return;

        if (col.gameObject.CompareTag("Player"))
        {
            var vida = col.gameObject.GetComponent<SistemaDeVida>();
            if (vida != null) vida.AplicarDano(10);
            AudioManager.Instance.Play("Dano");
            return;
        }

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
            {
                if (col.gameObject.CompareTag("Voador")) continue;
                birdMovingRight = !birdMovingRight;
                return;
            }
        }
    }

    // Efeito de recuo
    public void Bird_EfeitoDeRecuo()
    {
        if (!birdVivo) return;

        birdIsKnockBacked = true;
        birdFazendoRasante = false;

        float dir = birdMovingRight ? -1f : 1f;
        birdRb.velocity = Vector2.zero;
        birdRb.AddForce(new Vector2(dir * birdKnockbackForce, 0f), ForceMode2D.Impulse);

        StartCoroutine(Bird_ResetKnockback());
    }

    IEnumerator Bird_ResetKnockback()
    {
        yield return new WaitForSeconds(0.3f);
        birdIsKnockBacked = false;
    }

    // Piscar ao levar dano
    public void Bird_EfeitoDePiscar()
    {
        StartCoroutine(Bird_Piscar());
    }

    IEnumerator Bird_Piscar()
    {
        Color original = birdSprite.color;
        Color semi = new Color(original.r, original.g, original.b, 0.4f);

        for (int i = 0; i < 4; i++)
        {
            birdSprite.color = semi;
            yield return new WaitForSeconds(0.1f);
            birdSprite.color = original;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Bird_AnimacaoDeDano()
    {
        birdAnim.SetTrigger("Bird_Machucado");
    }

    internal void Bird_AnimacaoDeMorte()
    {
        birdVivo = false;
        birdFazendoRasante = false;

        birdAnim.SetBool("Bird_Vivo", false);

        birdRb.isKinematic = true;
        birdCol.enabled = false;

        Destroy(gameObject, 2f);
    }

    private void Bird_SetDirecaoInicial()
    {
        float dir = birdMovingRight ? 1f : -1f;
        birdSprite.flipX = !birdMovingRight;
        birdRb.velocity = new Vector2(dir * birdMoveSpeed, birdRb.velocity.y);
    }
}
