using System.Collections;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 0.3f;
    public int damage = 30;

    private void Start()
    {
        // Destroi depois de um tempo
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move para frente (direita local)
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 🔴 IGNORA PLAYER (resolve o bug de spawn sumindo)
        if (other.CompareTag("Player"))
            return;

        // 🟡 Tenta aplicar dano padrão
        var vida = other.GetComponent<SistemaDeVidaInimigo>();
        if (vida != null)
        {
            vida.AplicarDano(damage);
        }

        // 🔵 DANO NO VOADOR (AGORA SIM)
        var vidaVoador = other.GetComponent<SistemaDeVidaVoador>();
        if (vidaVoador != null)
        {
            vidaVoador.AplicarDano(damage);
        }

        // 🔵 Se for Voador, aplica efeitos extras
        var voador = other.GetComponent<Voador>();
        if (voador != null)
        {
            voador.Bird_EfeitoDeRecuo();
            voador.Bird_EfeitoDePiscar();
            voador.Bird_AnimacaoDeDano();
        }

        // 🟢 Destroi a adaga ao colidir (com qualquer coisa que não seja player)
        Destroy(gameObject);
    }
}