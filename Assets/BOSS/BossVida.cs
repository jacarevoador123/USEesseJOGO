using UnityEngine;

public class BossVida : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 3000;
    public int vidaAtual = 3000;
    [Range(0.1f, 0.9f)] public float porcentagemFase2 = 0.5f;

    [Header("Animacoes")]
    public string animHit = "boss_hit";
    public string animMorte = "boss morte";
    public bool tocarHitAoTomarDano = true;

    public bool EstaMorto { get; private set; }
    public bool EstaNaFase2 => !EstaMorto && vidaAtual <= vidaMaxima * porcentagemFase2;
    public float PercentualVida => vidaMaxima <= 0 ? 0f : (float)vidaAtual / vidaMaxima;

    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        vidaAtual = Mathf.Clamp(vidaAtual <= 0 ? vidaMaxima : vidaAtual, 0, vidaMaxima);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void AplicarDano(int dano)
    {
        if (EstaMorto || dano <= 0)
            return;

        vidaAtual = Mathf.Max(vidaAtual - dano, 0);

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        if (tocarHitAoTomarDano && anim != null && !string.IsNullOrEmpty(animHit))
            anim.Play(animHit, 0, 0f);
    }

    public void TakeDamage(int dano)
    {
        AplicarDano(dano);
    }

    public void TomarDano(int dano)
    {
        AplicarDano(dano);
    }

    public void ReceberDano(int dano)
    {
        AplicarDano(dano);
    }

    public void Damage(int dano)
    {
        AplicarDano(dano);
    }

    public void Curar(int valor)
    {
        if (EstaMorto || valor <= 0)
            return;

        vidaAtual = Mathf.Min(vidaAtual + valor, vidaMaxima);
    }

    private void Morrer()
    {
        EstaMorto = true;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        BossIAFSM fsm = GetComponent<BossIAFSM>();
        if (fsm != null)
            fsm.ForcarMorte();

        BossAtaques ataques = GetComponent<BossAtaques>();
        if (ataques != null)
            ataques.CancelarAtaques();

        if (anim != null && !string.IsNullOrEmpty(animMorte))
            anim.Play(animMorte, 0, 0f);
    }
}
