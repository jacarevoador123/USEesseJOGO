using UnityEngine;

public class AtaqueJogador : MonoBehaviour
{
    [SerializeField] int danoJogador = 50;

    public void DefinirDano(int novoDano)
    {
        danoJogador = novoDano;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
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