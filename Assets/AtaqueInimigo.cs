using UnityEngine;

public class AtaqueInimigo : MonoBehaviour
{
    public int dano = 10;

    private bool jaAcertou = false;

    private void OnEnable()
    {
        // toda vez que a hitbox ativar, pode acertar de novo
        jaAcertou = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (jaAcertou) return;

        if (other.CompareTag("Player"))
        {
            var vida = other.GetComponent<SistemaDeVida>();

            if (vida != null)
            {
                vida.AplicarDano(dano);
                AudioManager.Instance.Play("Dano");
                jaAcertou = true;
            }
        }
    }
}