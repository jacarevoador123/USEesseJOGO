using UnityEngine;

public class BossSmash : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 15f;
    public float tempoDeVida = 0.35f;

    [Header("Dano")]
    public int dano = 30;
    public string metodoDanoPlayer = "AplicarDano";
    public LayerMask camadasCenario;

    private Rigidbody2D rb;
    private Vector2 direcao = Vector2.right;
    private Transform dono;
    private bool iniciado;
    private float tempoRestante;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (!iniciado)
            Iniciar(direcao, dano, velocidade, tempoDeVida, dono, metodoDanoPlayer);
    }

    public void Iniciar(Vector2 novaDirecao, int novoDano, float novaVelocidade, float novoTempoDeVida, Transform novoDono, string novoMetodoDano)
    {
        direcao = novaDirecao.sqrMagnitude > 0.001f ? novaDirecao.normalized : Vector2.right;
        dano = novoDano;
        velocidade = novaVelocidade;
        tempoDeVida = novoTempoDeVida;
        dono = novoDono;
        metodoDanoPlayer = string.IsNullOrEmpty(novoMetodoDano) ? "TakeDamage" : novoMetodoDano;
        tempoRestante = tempoDeVida;
        iniciado = true;

        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angulo);
    }

    private void Update()
    {
        tempoRestante -= Time.deltaTime;
        if (tempoRestante <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (rb == null)
            transform.position += (Vector3)(direcao * velocidade * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (rb != null)
            rb.velocity = direcao * velocidade;
    }

    private void OnTriggerEnter2D(Collider2D other)
{
    if (dono != null && other.transform.root == dono.root)
        return;

    if (other.CompareTag("Player"))
    {
        other.SendMessage(metodoDanoPlayer, dano, SendMessageOptions.DontRequireReceiver);
    }
}
}
