public class SistemaDeVidaInimigo : SistemaDeVida
{
    Inimigo inimigo;
    BarraDeVidaInimigo barraDeVidaInimigo;
    new void Start()
    {
        base.Start();
        inimigo = GetComponent<Inimigo>();
        barraDeVidaInimigo = GetComponentInChildren<BarraDeVidaInimigo>();
    }

    public override void AplicarDano(float dano)
    {
        vidaAtual -= dano;
        if (vidaAtual <= 0)
        {
            Morrer();
        }

        inimigo.AnimacaoDeDano();
        inimigo.EfeitoDePiscar();
        inimigo.EfeitoDeRecuo();
        AtualizarVida();
    }

    override protected void Morrer()
    {
        inimigo.AnimacaoDeMorte();
    }
    void AtualizarVida()
    {
        barraDeVidaInimigo.AtualizarUI(vidaAtual / vidaMaxima);
    }
}