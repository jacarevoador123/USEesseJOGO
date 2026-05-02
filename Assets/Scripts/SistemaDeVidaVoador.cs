using UnityEngine;

public class SistemaDeVidaVoador : SistemaDeVida
{
    Voador voador;
    BarraDeVidaVoador BarraDeVidaVoador;

    new void Start()
    {
        base.Start();

        voador = GetComponent<Voador>();

        // procura mesmo se estiver desativado
        BarraDeVidaVoador = GetComponentInChildren<BarraDeVidaVoador>(true);

        if (BarraDeVidaVoador == null)
        {
            Debug.LogError("❌ BarraDeVidaVoador não encontrada no inimigo voador!");
        }
    }

    public override void AplicarDano(float dano)
    {
        if (vidaAtual <= 0) return;

        vidaAtual -= dano;

        AudioManager.Instance.Play("DanoInimigo");

        // EFEITOS DO VOADOR
        voador.Bird_AnimacaoDeDano();
        voador.Bird_EfeitoDePiscar();
        voador.Bird_EfeitoDeRecuo();

        AtualizarVida();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
    }

    protected override void Morrer()
    {
        voador.Bird_AnimacaoDeMorte();
    }

    void AtualizarVida()
    {
        if (BarraDeVidaVoador != null)
        {
            BarraDeVidaVoador.AtualizarUI(vidaAtual / vidaMaxima);
        }
    }
}
