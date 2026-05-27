using UnityEngine;
using System.Collections;

public class BossVida : MonoBehaviour
{
    [Header("Hit")]
    public float duracaoAnimHit = 0.35f;
    public bool EstaEmHit { get; private set; }
    private Coroutine rotinaHit;

    [Header("Vida")]
    public int vidaMaxima = 3000;
    public int vidaAtual = 3000;
    [Range(0.1f, 0.9f)] public float porcentagemFase2 = 0.5f;

    [Header("Animacoes")]
    public string animHit = "boss_hit";
    public string animMorte = "boss_morte";
    public bool tocarHitAoTomarDano = true;

    public bool EstaMorto { get; private set; }
    public bool EstaNaFase2 => !EstaMorto && vidaAtual <= vidaMaxima * porcentagemFase2;
    public float PercentualVida => vidaMaxima <= 0 ? 0f : (float)vidaAtual / vidaMaxima;

    [Header("Visual fase 2")]
public bool mudarCorNaFase2 = true;
public Color corFase2 = new Color(1f, 0.231f, 0f, 1f); // FF3B00

private SpriteRenderer[] sprites;
private Color[] coresOriginais;
private bool corFase2Ativa;

    private Animator anim;
    private Rigidbody2D rb;

    private void Awake()
    {
        vidaAtual = Mathf.Clamp(vidaAtual <= 0 ? vidaMaxima : vidaAtual, 0, vidaMaxima);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        coresOriginais = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            coresOriginais[i] = sprites[i].color;
        
        AtualizarCorFase();
    }

    public void AplicarDano(int dano)
    {
        if (EstaMorto || dano <= 0)
            return;

        vidaAtual = Mathf.Max(vidaAtual - dano, 0);
        AtualizarCorFase();

        if (vidaAtual <= 0)
        {
            Morrer();
            return;
        }

        if (tocarHitAoTomarDano)
        TocarHit();
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
        AtualizarCorFase();
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

        if (rotinaHit != null)
            StopCoroutine(rotinaHit);
            EstaEmHit = false;

        if (anim != null && !string.IsNullOrEmpty(animMorte))
            anim.Play(animMorte, 0, 0f);
    }

    private void TocarHit()
{
    if (anim == null || string.IsNullOrEmpty(animHit))
        return;

    if (rotinaHit != null)
        StopCoroutine(rotinaHit);

    rotinaHit = StartCoroutine(RotinaHit());
}

private IEnumerator RotinaHit()
{
    EstaEmHit = true;
    anim.Play(animHit, 0, 0f);

    yield return new WaitForSeconds(duracaoAnimHit);

    EstaEmHit = false;
    rotinaHit = null;
}

private void AtualizarCorFase()
{
    if (!mudarCorNaFase2 || sprites == null)
        return;

    bool deveUsarCorFase2 = EstaNaFase2;

    if (corFase2Ativa == deveUsarCorFase2)
        return;

    corFase2Ativa = deveUsarCorFase2;

    for (int i = 0; i < sprites.Length; i++)
    {
        if (sprites[i] == null)
            continue;

        sprites[i].color = corFase2Ativa ? corFase2 : coresOriginais[i];
    }
}
}
