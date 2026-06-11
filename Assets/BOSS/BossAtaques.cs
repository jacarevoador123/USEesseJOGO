using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAtaques : MonoBehaviour
{
    [Header("Dano por contato")]
public int danoContato = 10;
public float intervaloDanoContato = 0.5f;

private float proximoDanoContato;

    [Header("Ataque Area - Pontos separados")]
public Transform pontoSpawnArea;
public Transform pontoDanoArea;

    [Header("Ataque Area (Fase 2)")]
public GameObject efeitoAtaqueArea;

public float raioArea = 3f;
public int danoArea = 25;
public float delayArea = 0.2f;

    [Header("Referencias")]
    public Transform player;
    public LayerMask playerLayer;
    public string metodoDanoPlayer = "AplicarDano";

    [Header("Ataque curto")]
    public int danoCurto = 20;
    public Transform pontoAtaqueCurto;
    public Vector2 tamanhoAtaqueCurto = new Vector2(2f, 1.4f);
    public float atrasoPrimeiroCorte = 0.18f;
    public float intervaloEntreCortes = 0.22f;
    public int quantidadeCortes = 3;
    public float duracaoAtaqueCurto = 0.9f;
    public string animAtaqueCurto = "ataque_curto";

    [Header("Ataque longo")]
    public GameObject espadaPrefab;
    public Transform pontoSpawnEspada;
    public int danoEspada = 30;
    public float velocidadeEspada = 15f;
    public float tempoVidaEspada = 0.35f;
    public float atrasoSpawnEspada = 0.28f;
    public float duracaoAtaqueLongo = 0.8f;
    public bool espadaMiraComAltura = true;
    public string animAtaqueLongo = "ataque_longo";

    [Header("Ataque smash")]
    public GameObject fumacaVermelhaPrefab;
    public GameObject particulaPousoPrefab;
    public Transform pontoPouso;
    public int danoSmashProjetil = 35;
    public float velocidadeFumaca = 9f;
    public float tempoVidaFumaca = 0.75f;
    public float distanciaSpawnFumaca = 1.4f;
    public float alturaSpawnFumaca = 0.2f;
    public float atrasoSpawnFumaca = 0.15f;
    public float atrasoInicioPuloSmash = 0.05f;
    public float duracaoPuloSmash = 0.55f;
    public float alturaPuloSmash = 2.5f;
    public float duracaoParticulaPouso = 1.2f;
    public float duracaoAtaqueSmash = 1.3f;
    public string animAtaqueSmash = "ataque_smash";
    public string animAtaqueArea = "ataque_area";

    [Header("Animacao de saida")]
    public string animParado = "parado";

    private float gravidadeOriginal;
    private bool gravidadeAlteradaNoSmash;

    private BossVida bossVida;

    private BossState currentState = BossState.Patrol;
    private Rigidbody2D rb;
    private Animator anim;
    private BossPatrulha patrulha;
    private Coroutine rotinaAtual;

    private void Awake()
    {
        bossVida = GetComponent<BossVida>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        patrulha = GetComponent<BossPatrulha>();
    }

    public BossState GetState()
    {
        return currentState;
    }

    public void Attack_Curto()
    {
        ComecarAtaque(CorrotinaAtaqueCurto());
    }

    public void Attack_Longo()
    {
        ComecarAtaque(CorrotinaAtaqueLongo());
    }

    public void Attack_Smash()
    {
        ComecarAtaque(CorrotinaAtaqueSmash());
    }

    public void CancelarAtaques()
    {
        if (rotinaAtual != null)
            StopCoroutine(rotinaAtual);

        rotinaAtual = null;
        currentState = BossState.Patrol;

        RestaurarRigidbodyDepoisSmash();

        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private RigidbodyType2D bodyTypeOriginal;

private void ComecarAtaque(IEnumerator rotina)
{
    if (currentState == BossState.Attacking)
        return;

    if (rb != null)
    {
        bodyTypeOriginal = rb.bodyType;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
    }

    if (patrulha != null)
        patrulha.Parar();

    if (player != null && patrulha != null)
        patrulha.VirarPara(player);

    currentState = BossState.Attacking;
    rotinaAtual = StartCoroutine(rotina);
}

    private IEnumerator CorrotinaAtaqueCurto()
    {
        TocarAnimacao(animAtaqueCurto);
        yield return new WaitForSeconds(atrasoPrimeiroCorte);
        AudioManager.Instance.Play("3CORTES");

        for (int i = 0; i < quantidadeCortes; i++)
        {
            AcertarAtaqueCurto();

            if (i < quantidadeCortes - 1)
                yield return new WaitForSeconds(intervaloEntreCortes);
        }

        float tempoUsado = atrasoPrimeiroCorte + intervaloEntreCortes * Mathf.Max(0, quantidadeCortes - 1);
        yield return new WaitForSeconds(Mathf.Max(0f, duracaoAtaqueCurto - tempoUsado));
        FinalizarAtaque();
    }

    private IEnumerator CorrotinaAtaqueLongo()
    {
        TocarAnimacao(animAtaqueLongo);
        yield return new WaitForSeconds(atrasoSpawnEspada);
        AudioManager.Instance.Play("VORAZ");

        SpawnarProjetil(
            espadaPrefab,
            pontoSpawnEspada != null ? pontoSpawnEspada.position : transform.position,
            danoEspada,
            velocidadeEspada,
            tempoVidaEspada,
            espadaMiraComAltura
        );

        yield return new WaitForSeconds(Mathf.Max(0f, duracaoAtaqueLongo - atrasoSpawnEspada));
        FinalizarAtaque();
    }

    private IEnumerator CorrotinaAtaqueSmash()
{
    TocarAnimacao(animAtaqueSmash);
    AudioManager.Instance.Play("INICIOSMASH");

    if (atrasoInicioPuloSmash > 0f)
        yield return new WaitForSeconds(atrasoInicioPuloSmash);

    yield return StartCoroutine(PularSmash());

    SpawnarParticulaPouso();

    if (atrasoSpawnFumaca > 0f)
        yield return new WaitForSeconds(atrasoSpawnFumaca);

    SpawnarFumacaSmash();
    AudioManager.Instance.Play("FIMSMASH");

    float tempoUsado = atrasoInicioPuloSmash + duracaoPuloSmash + Mathf.Max(0f, atrasoSpawnFumaca);
    yield return new WaitForSeconds(Mathf.Max(0f, duracaoAtaqueSmash - tempoUsado));

    FinalizarAtaque();
}

    private void AcertarAtaqueCurto()
    {
        Vector2 centro = pontoAtaqueCurto != null ? pontoAtaqueCurto.position : transform.position;
        Collider2D[] hits = playerLayer.value == 0
            ? Physics2D.OverlapBoxAll(centro, tamanhoAtaqueCurto, 0f)
            : Physics2D.OverlapBoxAll(centro, tamanhoAtaqueCurto, 0f, playerLayer);

        HashSet<Collider2D> acertados = new HashSet<Collider2D>();

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player") || acertados.Contains(hit))
                continue;

            hit.SendMessage(metodoDanoPlayer, danoCurto, SendMessageOptions.DontRequireReceiver);
            acertados.Add(hit);
        }
    }

    private void SpawnarFumacaSmash()
    {
        if (fumacaVermelhaPrefab == null)
            return;

        Vector2 direcao = DirecaoParaPlayer(false);
        Vector3 posicao = transform.position + (Vector3)(direcao * distanciaSpawnFumaca) + Vector3.up * alturaSpawnFumaca;
        SpawnarProjetil(fumacaVermelhaPrefab, posicao, danoSmashProjetil, velocidadeFumaca, tempoVidaFumaca, false);
    }

    private void SpawnarParticulaPouso()
    {
        if (particulaPousoPrefab == null)
            return;

        Vector3 posicao = pontoPouso != null ? pontoPouso.position : transform.position;
        GameObject particula = Instantiate(particulaPousoPrefab, posicao, Quaternion.identity);
        Destroy(particula, duracaoParticulaPouso);
    }

    private void SpawnarProjetil(GameObject prefab, Vector3 posicao, int dano, float velocidade, float tempoDeVida, bool mirarComAltura)
    {
        if (prefab == null)
            return;

        Vector2 direcao = DirecaoParaPlayer(mirarComAltura);
        GameObject projetilObj = Instantiate(prefab, posicao, Quaternion.identity);
        BossProjetil projetil = projetilObj.GetComponent<BossProjetil>();

if (projetil != null)
{
    projetil.Iniciar(direcao, dano, velocidade, tempoDeVida, transform, metodoDanoPlayer);
    return;
}

BossSmash smash = projetilObj.GetComponent<BossSmash>();

if (smash != null)
{
    smash.Iniciar(direcao, dano, velocidade, tempoDeVida, transform, metodoDanoPlayer);
    return;
}

Debug.LogWarning(
    "O prefab " + prefab.name +
    " precisa do script BossProjetil ou BossSmash."
);
    }

    private Vector2 DirecaoParaPlayer(bool mirarComAltura)
    {
        if (player == null)
            return transform.localScale.x >= 0f ? Vector2.right : Vector2.left;

        Vector2 direcao = player.position - transform.position;

        if (!mirarComAltura)
            direcao.y = 0f;

        if (direcao.sqrMagnitude < 0.001f)
            direcao = transform.localScale.x >= 0f ? Vector2.right : Vector2.left;

        return direcao.normalized;
    }

   private void FinalizarAtaque()
{
    if (bossVida != null)
        bossVida.Invulneravel = false;

    if (rb != null)
    {
        rb.bodyType = bodyTypeOriginal;
        rb.velocity = Vector2.zero;
    }

    rotinaAtual = null;
    currentState = BossState.Recover;
    ForcarAnimacaoParado();
}

    private void TocarAnimacao(string nome)
{
    if (anim == null || string.IsNullOrEmpty(nome))
        return;

    anim.CrossFade(nome, 0.05f);
}

    public void ForcarAnimacaoParado()
{
     if (bossVida != null && bossVida.EstaEmHit)
        return;

    if (rb != null)
        rb.velocity = Vector2.zero;

    if (anim == null || string.IsNullOrEmpty(animParado))
        return;

    AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

    if (!state.IsName(animParado))
        anim.Play(animParado, 0, 0f);
}

private IEnumerator PularSmash()
{
    float duracao = Mathf.Max(0.01f, duracaoPuloSmash);
    Vector2 posicaoInicial = rb != null ? rb.position : (Vector2)transform.position;

    PrepararRigidbodyParaSmash();

    float tempo = 0f;

    while (tempo < duracao)
    {
        float delta = rb != null ? Time.fixedDeltaTime : Time.deltaTime;
        tempo += delta;

        float progresso = Mathf.Clamp01(tempo / duracao);
        float alturaAtual = Mathf.Sin(progresso * Mathf.PI) * alturaPuloSmash;

        Vector2 novaPosicao = new Vector2(posicaoInicial.x, posicaoInicial.y + alturaAtual);

        if (rb != null)
        {
            rb.MovePosition(novaPosicao);
            yield return new WaitForFixedUpdate();
        }
        else
        {
            transform.position = novaPosicao;
            yield return null;
        }
    }

    if (rb != null)
    {
        rb.MovePosition(posicaoInicial);
        yield return new WaitForFixedUpdate();
    }
    else
    {
        transform.position = posicaoInicial;
    }

    RestaurarRigidbodyDepoisSmash();
}

private void PrepararRigidbodyParaSmash()
{
    if (rb == null)
        return;

    gravidadeOriginal = rb.gravityScale;
    gravidadeAlteradaNoSmash = true;

    rb.velocity = Vector2.zero;
    rb.gravityScale = 0f;
}

private void RestaurarRigidbodyDepoisSmash()
{
    if (rb == null || !gravidadeAlteradaNoSmash)
        return;

    rb.gravityScale = gravidadeOriginal;
    rb.velocity = Vector2.zero;
    gravidadeAlteradaNoSmash = false;
}

public void Attack_Area()
{
    ComecarAtaque(CorrotinaAtaqueArea());
}

private IEnumerator CorrotinaAtaqueArea()
{
    // toca animação
    TocarAnimacao(animAtaqueArea);

    if (AudioManager.Instance != null)
        AudioManager.Instance.Play("SMASH_AREA");

    // duração total da animação
    float duracaoAnimacao = 3.4f;

    // tempo até o impacto
    float tempoImpacto = 1.5f;

    GameObject efeitoInstanciado = null;

    // espera até o impacto
    yield return new WaitForSeconds(tempoImpacto);

    // SPAWN DO EFEITO
    Vector3 posSpawn = pontoSpawnArea != null ? pontoSpawnArea.position : transform.position;

    if (efeitoAtaqueArea != null)
    {
        efeitoInstanciado = Instantiate(
    efeitoAtaqueArea,
    posSpawn,
    Quaternion.identity
);

AreaBossDano area = efeitoInstanciado.GetComponent<AreaBossDano>();

if (area != null)
{
    area.dano = danoArea;
}
    }

    // espera terminar animação total
    float restante = duracaoAnimacao - tempoImpacto;
    if (restante > 0f)
        yield return new WaitForSeconds(restante);

    // destrói efeito no final da animação
    if (efeitoInstanciado != null)
        Destroy(efeitoInstanciado);

    FinalizarAtaque();
}

private void OnDrawGizmosSelected()
{
    // SPAWN (amarelo)
    Gizmos.color = Color.yellow;
    Vector3 spawn = pontoSpawnArea != null ? pontoSpawnArea.position : transform.position;
    Gizmos.DrawWireSphere(spawn, 0.2f);

    // DANO (vermelho)
    Gizmos.color = Color.red;
    Vector3 dano = pontoDanoArea != null ? pontoDanoArea.position : transform.position;
    Gizmos.DrawWireSphere(dano, raioArea);
}

private void OnCollisionStay2D(Collision2D collision)
{
    if (!collision.gameObject.CompareTag("Player"))
        return;

    if (Time.time < proximoDanoContato)
        return;

    collision.gameObject.SendMessage(
        metodoDanoPlayer,
        danoContato,
        SendMessageOptions.DontRequireReceiver
    );

    proximoDanoContato = Time.time + intervaloDanoContato;
}
}
