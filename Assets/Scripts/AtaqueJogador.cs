using UnityEngine;

public class AtaqueJogador : MonoBehaviour
{
    [SerializeField] int danoJogador = 30;
    public int damage = 30;

    public void DefinirDano(int novoDano)
    {
        danoJogador = novoDano;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BOSS"))
        {
            var vidaInimigo = other.GetComponent<BossVida>();
            if (vidaInimigo != null)
                vidaInimigo.AplicarDano(damage);

            return;
        }

        if (other.CompareTag("Inimigo"))
        {
            var vidaInimigo = other.GetComponent<SistemaDeVidaInimigo>();
            if (vidaInimigo != null)
                vidaInimigo.AplicarDano(danoJogador);

            return;
        }

        if (other.CompareTag("Voador"))
        {
            var vidaInimigo = other.GetComponent<SistemaDeVidaVoador>();
            if (vidaInimigo != null)
                vidaInimigo.AplicarDano(danoJogador);

            return;
        }
    }
}