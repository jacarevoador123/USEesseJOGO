using UnityEngine;

public class SistemaDeVidaInimigo : SistemaDeVida
{
    private Inimigo inimigo;
    private BarraDeVidaInimigo barraDeVidaInimigo;

    new void Start()
    {
        base.Start();

        inimigo = GetComponent<Inimigo>();
        barraDeVidaInimigo = GetComponentInChildren<BarraDeVidaInimigo>();

        AtualizarVida();
    }

    public override void AplicarDano(float dano)
    {
        vidaAtual -= dano;

        AudioManager.Instance.Play("DanoInimigo");

        inimigo.AnimacaoDeDano();
        inimigo.EfeitoDePiscar();
        inimigo.EfeitoDeRecuo();

        AtualizarVida();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    protected override void Morrer()
    {
        inimigo.AnimacaoDeMorte();
    }

    void AtualizarVida()
    {
        if (barraDeVidaInimigo != null)
        {
            barraDeVidaInimigo.AtualizarUI(vidaAtual / vidaMaxima);
        }
    }
}