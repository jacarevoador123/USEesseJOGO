using UnityEngine;
using System.Collections;

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
        if (inimigo.invulneravel || inimigo.atacando)
            return;

        vidaAtual -= dano;

        StartCoroutine(Invulnerabilidade());

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

    private IEnumerator Invulnerabilidade()
    {
        inimigo.invulneravel = true;

        yield return new WaitForSeconds(inimigo.tempoInvulnerabilidade);

        inimigo.invulneravel = false;
    }
}