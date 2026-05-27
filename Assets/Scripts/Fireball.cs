using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 0.3f;
    public int damage = 30;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            return;

        if (other.CompareTag("BOSS"))
        {
            var vidaInimigo = other.GetComponent<BossVida>();
            if (vidaInimigo != null)
                vidaInimigo.AplicarDano(damage);

            return;
        }

        var vida = other.GetComponent<SistemaDeVidaInimigo>();
        if (vida != null)
        {
            vida.AplicarDano(damage);
        }

        var vidaVoador = other.GetComponent<SistemaDeVidaVoador>();
        if (vidaVoador != null)
        {
            vidaVoador.AplicarDano(damage);
        }

        var voador = other.GetComponent<Voador>();
        if (voador != null)
        {
            voador.Bird_EfeitoDeRecuo();
            voador.Bird_EfeitoDePiscar();
            voador.Bird_AnimacaoDeDano();
        }

        Destroy(gameObject);
    }
}
