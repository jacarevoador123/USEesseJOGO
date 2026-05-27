using UnityEngine;

public class BossPatrulha : MonoBehaviour
{
    [Header("Pontos de patrulha")]
    public Transform pontoA;
    public Transform pontoB;

    [Header("Movimento")]
    public float velocidadePatrulha = 2f;
    public float velocidadePerseguicao = 3f;
    public float distanciaChegada = 0.2f;
    public bool spriteOlhaParaDireita = true;

    [Header("Animacoes")]
    public string animAndar = "andar";
    public string animParado = "parado";

    private BossVida bossVida;

    private Transform alvoPatrulha;
    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        bossVida = GetComponent<BossVida>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        alvoPatrulha = pontoB;
    }

    public void Patrulhar(bool ativo)
    {
        if (!ativo)
        {
            Parar();
            return;
        }

        if (pontoA == null || pontoB == null)
        {
            Parar();
            return;
        }

        if (alvoPatrulha == null)
            alvoPatrulha = pontoB;

        float distanciaX = Mathf.Abs(alvoPatrulha.position.x - transform.position.x);
        if (distanciaX <= distanciaChegada)
            alvoPatrulha = alvoPatrulha == pontoA ? pontoB : pontoA;

        MoverHorizontal(Mathf.Sign(alvoPatrulha.position.x - transform.position.x), velocidadePatrulha);
    }

    public void MoverEmDirecao(Vector2 destino, float pararQuandoChegarNaDistancia)
{
    float diferencaX = destino.x - transform.position.x;

    if (Mathf.Abs(diferencaX) <= pararQuandoChegarNaDistancia)
    {
        Parar();
        return;
    }

    MoverHorizontal(diferencaX > 0f ? 1f : -1f, velocidadePerseguicao);
}

    public void VirarPara(Transform alvo)
    {
        if (alvo == null)
            return;

        float diferencaX = alvo.position.x - transform.position.x;
        if (Mathf.Abs(diferencaX) > 0.01f)
            AtualizarFlip(Mathf.Sign(diferencaX));
    }

    public void Parar()
    {
        if (rb != null)
            rb.velocity = new Vector2(0f, rb.velocity.y);

        TocarAnimacao(animParado);
    }

    public void ForcarAnimacaoParado()
    {
        if (anim != null && !string.IsNullOrEmpty(animParado))
            anim.Play(animParado, 0, 0f);
    }

    private void MoverHorizontal(float direcao, float velocidade)
    {
        if (Mathf.Abs(direcao) < 0.01f)
        {
            Parar();
            return;
        }

        if (rb != null)
            rb.velocity = new Vector2(direcao * velocidade, rb.velocity.y);
        else
            transform.position += Vector3.right * direcao * velocidade * Time.deltaTime;

        AtualizarFlip(direcao);
        TocarAnimacao(animAndar);
    }

    private void AtualizarFlip(float direcao)
    {
        Vector3 escala = transform.localScale;
        float sinalDireita = spriteOlhaParaDireita ? 1f : -1f;
        escala.x = Mathf.Abs(escala.x) * (direcao >= 0f ? sinalDireita : -sinalDireita);
        transform.localScale = escala;
    }

    private void TocarAnimacao(string nome)
    {
        if (bossVida != null && bossVida.EstaEmHit)
            return;
        if (anim == null || string.IsNullOrEmpty(nome))
            return;

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.IsName(nome))
            return;

        anim.Play(nome, 0, 0f);
    }
}
