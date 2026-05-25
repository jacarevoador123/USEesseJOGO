using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAtaques : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public LayerMask playerLayer;
    public string metodoDanoPlayer = "TakeDamage";

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
    public float atrasoPousoSmash = 0.55f;
    public float duracaoParticulaPouso = 1.2f;
    public float duracaoAtaqueSmash = 1.3f;
    public string animAtaqueSmash = "ataque_smash";

    [Header("Animacao de saida")]
    public string animParado = "parado";

    private BossState currentState = BossState.Patrol;
    private Rigidbody2D rb;
    private Animator anim;
    private BossPatrulha patrulha;
    private Coroutine rotinaAtual;

    private void Awake()
    {
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

        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    private void ComecarAtaque(IEnumerator rotina)
    {
        if (currentState == BossState.Attacking)
            return;

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
        yield return new WaitForSeconds(atrasoPousoSmash);

        SpawnarParticulaPouso();
        SpawnarFumacaSmash();

        yield return new WaitForSeconds(Mathf.Max(0f, duracaoAtaqueSmash - atrasoPousoSmash));
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
            projetil.Iniciar(direcao, dano, velocidade, tempoDeVida, transform, metodoDanoPlayer);
        else
            Debug.LogWarning("O prefab " + prefab.name + " precisa do script BossProjetil para causar dano e se mover.");
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
        rotinaAtual = null;
        currentState = BossState.Recover;

        if (rb != null)
            rb.velocity = Vector2.zero;

        TocarAnimacao(animParado);
    }

    private void TocarAnimacao(string nome)
    {
        if (anim != null && !string.IsNullOrEmpty(nome))
            anim.Play(nome, 0, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 centro = pontoAtaqueCurto != null ? pontoAtaqueCurto.position : transform.position;
        Gizmos.DrawWireCube(centro, tamanhoAtaqueCurto);
    }
}
