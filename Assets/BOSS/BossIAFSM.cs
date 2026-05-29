using UnityEngine;
using System.Collections;

public class BossIAFSM : MonoBehaviour
{
    private bool fase2Ativada;
    private bool transicaoFase2;

    [Header("Referencias")]
    public Transform player;

    [Header("Visao")]
    public float distanciaVerPlayer = 12f;
    public float distanciaPerderPlayer = 16f;
    public bool precisaLinhaDeVisao = false;
    public LayerMask camadasBloqueiamVisao;

    [Header("Cooldown geral")]
    public float primeiroAtaqueDelay = 0.4f;
    public float tempoParadoDepoisAtaque = 4f;
    public float cooldownEntreAtaques = 10f;


    [Header("Fase 1 - distancias")]
    public float fase1CurtoMax = 2f;
    public float fase1LongoMin = 2f;
    public float fase1LongoMax = 12f;

    [Header("Fase 2 - distancias")]
    public float fase2CurtoMax = 1f;
    public float fase2SmashMin = 2f;
    public float fase2SmashMax = 6f;
    public float fase2LongoMin = 7f;
    public float fase2LongoMax = 12f;

    [Header("Comportamento")]
    public float distanciaPararPerseguicao = 0.8f;

    private Rigidbody2D rb;
    private bool recuperandoDepoisDeAtaque;
    private bool paradoDepoisDeAtaque;

    private BossState currentState = BossState.Patrol;
    private BossAtaques bossAtaques;
    private BossPatrulha bossPatrulha;
    private BossVida bossVida;

    private float stateTimer;
    private bool playerFoiVisto;

    public bool VendoPlayer => playerFoiVisto;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bossAtaques = GetComponent<BossAtaques>();
        bossPatrulha = GetComponent<BossPatrulha>();
        bossVida = GetComponent<BossVida>();
    }

    private void Start()
    {
        if (bossAtaques != null && bossAtaques.player == null)
            bossAtaques.player = player;
    }

    private void Update()
    {
        if (bossVida != null && bossVida.EstaMorto)
        {
            currentState = BossState.Dead;
            return;
        }

        if (transicaoFase2)
{
    PararBoss();

    if (bossAtaques != null)
        bossAtaques.ForcarAnimacaoParado();

    return;
}

        if (bossVida != null && bossVida.EstaNaFase2 && !fase2Ativada)
{
    fase2Ativada = true;
    StartCoroutine(TransicaoFase2());
    return;
}

        switch (currentState)
        {
            case BossState.Patrol:
                PatrolState();
                break;

            case BossState.Chase:
                ChaseState();
                break;

            case BossState.ChooseAttack:
                ChooseAttackState();
                break;

            case BossState.Attacking:
                AttackState();
                break;

            case BossState.Recover:
                RecoverState();
                break;

            case BossState.Dead:
                break;
        }
    }

    public void ForcarMorte()
    {
        currentState = BossState.Dead;

        if (bossPatrulha != null)
            bossPatrulha.Parar();
    }

    private void PatrolState()
    {
        if (PodeVerPlayer())
        {
            playerFoiVisto = true;
            stateTimer = primeiroAtaqueDelay;
            currentState = BossState.Recover;
            AudioManager.Instance.Play("FASE1");

            if (bossPatrulha != null)
                bossPatrulha.VirarPara(player);

            return;
        }


        playerFoiVisto = false;

        if (bossPatrulha != null)
            bossPatrulha.Patrulhar(true);
    }

    private void ChaseState()
    {
        if (!PodeContinuarVendoPlayer())
        {
            currentState = BossState.Patrol;
            return;
        }

        BossAttackType ataque = EscolherAtaque();
        if (ataque != BossAttackType.None)
        {
            currentState = BossState.ChooseAttack;
            return;
        }

        PerseguirPlayer();
    }

    private void RecoverState()
{
    if (recuperandoDepoisDeAtaque)
    {
        if (paradoDepoisDeAtaque)
        {
            PararBoss();

            if (bossAtaques != null)
                bossAtaques.ForcarAnimacaoParado();

            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0f)
            {
                paradoDepoisDeAtaque = false;
                stateTimer = cooldownEntreAtaques;
            }

            return;
        }

        if (PodeContinuarVendoPlayer())
            PerseguirPlayer();
        else if (bossPatrulha != null)
            bossPatrulha.Patrulhar(true);

        stateTimer -= Time.deltaTime;

        if (stateTimer <= 0f)
        {
            recuperandoDepoisDeAtaque = false;
            currentState = PodeContinuarVendoPlayer() ? BossState.ChooseAttack : BossState.Patrol;
        }

        return;
    }

    if (!PodeContinuarVendoPlayer())
    {
        currentState = BossState.Patrol;
        return;
    }

    PararBoss();

    stateTimer -= Time.deltaTime;

    if (stateTimer <= 0f)
        currentState = BossState.ChooseAttack;
}

    private void ChooseAttackState()
    {
        if (bossVida != null)
            bossVida.Invulneravel = true;

        if (!PodeContinuarVendoPlayer())
        {
            currentState = BossState.Patrol;
            return;
        }

        if (bossPatrulha != null)
            bossPatrulha.VirarPara(player);

        BossAttackType ataque = EscolherAtaque();
        if (ataque == BossAttackType.None)
        {
            currentState = BossState.Chase;
            return;
        }

        if (bossPatrulha != null)
            bossPatrulha.Parar();

        ExecutarAtaque(ataque);
        currentState = BossState.Attacking;
    }

    private void AttackState()
{
    if (bossAtaques != null && bossAtaques.GetState() == BossState.Attacking)
        return;

    recuperandoDepoisDeAtaque = true;
    paradoDepoisDeAtaque = true;
    stateTimer = tempoParadoDepoisAtaque;

    PararBoss();

    if (bossAtaques != null)
        bossAtaques.ForcarAnimacaoParado();

    currentState = BossState.Recover;
}

    private void MoverOuPararConformeDistancia()
    {
        if (player == null || bossPatrulha == null)
            return;

        if (EscolherAtaque() != BossAttackType.None)
        {
            bossPatrulha.Parar();
            bossPatrulha.VirarPara(player);
            return;
        }

        PerseguirPlayer();
    }

    private void PerseguirPlayer()
    {
        if (player == null || bossPatrulha == null)
            return;

        bossPatrulha.MoverEmDirecao(player.position, distanciaPararPerseguicao);
        bossPatrulha.VirarPara(player);
    }

    private BossAttackType EscolherAtaque()
{
    if (player == null)
        return BossAttackType.None;

    float distancia = Vector2.Distance(transform.position, player.position);
    bool fase2 = bossVida != null && bossVida.EstaNaFase2;

    if (!fase2)
    {
        if (distancia <= fase1CurtoMax)
            return BossAttackType.Curto;

        if (distancia >= fase1LongoMin && distancia <= fase1LongoMax)
            return BossAttackType.Longo;

        return BossAttackType.None;
    }

    if (distancia <= fase2CurtoMax)
        return BossAttackType.Curto;

    if (distancia >= fase2SmashMin && distancia <= fase2SmashMax)
        return BossAttackType.Smash;

    if (distancia >= fase2LongoMin && distancia <= fase2LongoMax)
        return BossAttackType.Longo;

    return BossAttackType.None;
}

    private void ExecutarAtaque(BossAttackType ataque)
{
    if (bossAtaques == null)
        return;

    switch (ataque)
    {
        case BossAttackType.Curto:
            bossAtaques.Attack_Curto();
            break;

        case BossAttackType.Longo:
            bossAtaques.Attack_Longo();
            break;

        case BossAttackType.Smash:
            bossAtaques.Attack_Smash();
            break;
    }
}

    private bool PodeVerPlayer()
    {
        return VerificarPlayer(distanciaVerPlayer);
    }

    private bool PodeContinuarVendoPlayer()
    {
        if (!playerFoiVisto)
            return PodeVerPlayer();

        return VerificarPlayer(distanciaPerderPlayer);
    }

    private bool VerificarPlayer(float distanciaMaxima)
    {
        if (player == null)
            return false;

        Vector2 origem = transform.position;
        Vector2 destino = player.position;
        float distancia = Vector2.Distance(origem, destino);

        if (distancia > distanciaMaxima)
            return false;

        if (!precisaLinhaDeVisao || camadasBloqueiamVisao.value == 0)
            return true;

        Vector2 direcao = (destino - origem).normalized;
        RaycastHit2D hit = Physics2D.Raycast(origem, direcao, distancia, camadasBloqueiamVisao);
        return hit.collider == null;
    }

   private void PararBoss()
{
    if (bossPatrulha != null)
    {
        bossPatrulha.Parar();
        bossPatrulha.VirarPara(player);
        return;
    }

    if (rb != null)
        rb.velocity = Vector2.zero;
}

private IEnumerator TransicaoFase2()
{
    transicaoFase2 = true;

    currentState = BossState.Recover;

    if (bossVida != null)
        bossVida.Invulneravel = true;

    AudioManager.Instance.Stop("Tema");

    if (bossPatrulha != null)
        bossPatrulha.Parar();

    if (bossAtaques != null)
        bossAtaques.ForcarAnimacaoParado();

    if (rb != null)
        rb.velocity = Vector2.zero;

    float tempo = 16f;

    while (tempo > 0f)
    {
        tempo -= Time.deltaTime;

        if (bossPatrulha != null)
            bossPatrulha.Parar();

        if (bossAtaques != null)
            bossAtaques.ForcarAnimacaoParado();

        if (rb != null)
            rb.velocity = Vector2.zero;

        yield return null;
    }

    if (bossVida != null && !bossVida.EstaMorto)
        bossVida.Invulneravel = false;

    AudioManager.Instance.Play("Tema");

    transicaoFase2 = false;

    recuperandoDepoisDeAtaque = true;
    paradoDepoisDeAtaque = false;
    stateTimer = cooldownEntreAtaques;

    currentState = PodeContinuarVendoPlayer()
        ? BossState.Recover
        : BossState.Patrol;
}
}
