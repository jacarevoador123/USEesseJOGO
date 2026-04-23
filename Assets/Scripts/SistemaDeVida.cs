using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SistemaDeVida : MonoBehaviour
{
    public BarraDeVida barraDeVida;

    //Mostra na unity um slider que vai de 0 a 100
    [Range(0, 100)]
    public float vidaMaxima = 100f;
    [Range(0, 100)]
    public float vidaAtual;

    // Start is called before the first frame update
    protected void Start()
    {
        vidaAtual = vidaMaxima;
        AtualizarVida();
    }

    public virtual void AplicarDano(float dano)
    {
        vidaAtual -= dano;
        if (vidaAtual <= 0)
        {
            Morrer();
        }

        AtualizarVida();
    }

    private void AtualizarVida() //No futuro isso poderia ser controlado através de eventos
    {
        barraDeVida.AtualizarUI(vidaAtual / vidaMaxima);
    }

    protected virtual void Morrer()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    internal void AplicarDano(object dano)
    {
        throw new NotImplementedException();
    }
}