using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public Image vidaAtualImage;

    public virtual void AtualizarUI(float calculoVida)
    {
        calculoVida = Mathf.Clamp01(calculoVida);

        vidaAtualImage.fillAmount = calculoVida;
    }

    public class BarraDeVidaInimigo : BarraDeVida
{

}
}