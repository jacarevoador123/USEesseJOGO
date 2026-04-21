using System;
using System.Collections;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    [Header("Configurações")]
    public float moveSpeed = 2f;       // Velocidade de movimento
    public int maxHealth = 2;          // Vida do inimigo
    public float knockbackForce = 5f;  // Força do recuo ao levar dano
    [SerializeField] bool movingRight = true;   // Direção inicial do movimento
    private bool vivo = true;
    private bool isKnockBacked = false;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {

        if (isKnockBacked || !vivo) return;

        // Movimento básico para frente
        Move();
    }

    void Move()
    {
        // Define a direção do movimento
        float direction = movingRight ? 1 : -1;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        // Inverte a direção do sprite do personagem
        MirrorSprite(direction);

        anim.SetFloat("Velocidade", Mathf.Abs(rb.velocity.x));
    }

    private void MirrorSprite(float moveInput)
    {
        if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Inverte direção ao colidir com paredes ou obstáculos
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Inimigo"))
        {
            movingRight = !movingRight;
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            SistemaDeVida sistemaDeVida = collision.gameObject.GetComponent<SistemaDeVida>();
            sistemaDeVida.AplicarDano(10);
        }
    }

    public void EfeitoDeRecuo()
    {
        isKnockBacked = true;

        float knockbackDirection = movingRight ? -1 : 1;
        Vector2 force = new(knockbackDirection * knockbackForce, 0);

        // Zerar velocidade e Efeito de recuo
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.AddForce(force, ForceMode2D.Impulse);

        StartCoroutine(ResetKnockback());
    }

    IEnumerator ResetKnockback()
    {
        yield return new WaitForSeconds(0.5f); // Aguarde por 0.5 segundos
        isKnockBacked = false;
    }

    public void EfeitoDePiscar()
    {
        StartCoroutine(Piscar());
    }

    IEnumerator Piscar()
    {
        Color corOriginal = spriteRenderer.color;
        Color corTransparente = new Color(corOriginal.r, corOriginal.g, corOriginal.b, 0.5f);

        for (int i = 0; i < 3; i++)
        {
            spriteRenderer.color = corTransparente;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = corOriginal;
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

        Destroy(gameObject, 3); //Configurar o tempo de destruição do objeto
    }
}
