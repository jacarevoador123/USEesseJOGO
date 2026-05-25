using UnityEngine;

public class BossIAFSM : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;

    [Header("Visao")]
    public float distanciaVerPlayer = 12f;
    public float distanciaPerderPlayer = 16f;
    public bool precisaLinhaDeVisao = false;
    public LayerMask camadasBloqueiamVisao;

    [Header("Cooldown geral")]
    public float primeiroAtaqueDelay = 0.4f;
    public float cooldownEntreAtaques = 1.2f;

    [Header("Cooldown por ataque")]
    public float cooldownCurto = 1.4f;
    public float cooldownLongo = 2.2f;
    public float cooldownSmash = 3.5f;

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
    public bool perseguirDuranteCooldown = true;
    public bool aproximarAntesDeAtacar = true;
    public float distanciaMaximaParaIniciarAtaque = 7f;

    private BossState currentState = BossState.Patrol;
    private BossAtaques bossAtaques;
    private BossPatrulha bossPatrulha;
    private BossVida bossVida;

    private float stateTimer;
    private float proximoCurto;
    private float proximoLongo;
    private float proximoSmash;
    private bool playerFoiVisto;

    private void Awake()
    {
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
        if (!PodeContinuarVendoPlayer())
        {
            currentState = BossState.Patrol;
            return;
        }

        stateTimer -= Time.deltaTime;

        if (perseguirDuranteCooldown)
            MoverOuPararConformeDistancia();
        else if (bossPatrulha != null)
        {
            bossPatrulha.Parar();
            bossPatrulha.VirarPara(player);
        }

        if (stateTimer <= 0f)
            currentState = BossState.ChooseAttack;
    }

    private void ChooseAttackState()
    {
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
        if (bossAtaques == null || bossAtaques.GetState() != BossState.Attacking)
        {
            stateTimer = cooldownEntreAtaques;
            currentState = BossState.Recover;
        }
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

        if (DeveAproximarAntesDeAtacar(distancia))
            return BossAttackType.None;

        if (!fase2)
        {
            if (distancia <= fase1CurtoMax && AtaqueDisponivel(BossAttackType.Curto))
                return BossAttackType.Curto;

            if (distancia >= fase1LongoMin && distancia <= fase1LongoMax && AtaqueDisponivel(BossAttackType.Longo))
                return BossAttackType.Longo;

            return BossAttackType.None;
        }

        if (distancia <= fase2CurtoMax && AtaqueDisponivel(BossAttackType.Curto))
            return BossAttackType.Curto;

        if (distancia >= fase2SmashMin && distancia <= fase2SmashMax && AtaqueDisponivel(BossAttackType.Smash))
            return BossAttackType.Smash;

        if (distancia >= fase2LongoMin && distancia <= fase2LongoMax && AtaqueDisponivel(BossAttackType.Longo))
            return BossAttackType.Longo;

        return BossAttackType.None;
    }

    private bool AtaqueDisponivel(BossAttackType ataque)
    {
        float agora = Time.time;

        switch (ataque)
        {
            case BossAttackType.Curto:
                return agora >= proximoCurto;
            case BossAttackType.Longo:
                return agora >= proximoLongo;
            case BossAttackType.Smash:
                return agora >= proximoSmash;
            default:
                return false;
        }
    }

    private bool DeveAproximarAntesDeAtacar(float distancia)
    {
        return aproximarAntesDeAtacar && distancia > distanciaMaximaParaIniciarAtaque;
    }

    private void ExecutarAtaque(BossAttackType ataque)
    {
        if (bossAtaques == null)
            return;

        switch (ataque)
        {
            case BossAttackType.Curto:
                proximoCurto = Time.time + cooldownCurto;
                bossAtaques.Attack_Curto();
                break;

            case BossAttackType.Longo:
                proximoLongo = Time.time + cooldownLongo;
                bossAtaques.Attack_Longo();
                break;

            case BossAttackType.Smash:
                proximoSmash = Time.time + cooldownSmash;
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
}
