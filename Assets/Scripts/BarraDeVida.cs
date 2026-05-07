using UnityEngine;
using UnityEngine.UI;

public class BarraDeVida : MonoBehaviour
{
    public Image vidaAtualImage;

    public void AtualizarUI(float calculoVida)
    {   
        if(calculoVida >= 0)     {
            vidaAtualImage.fillAmount = calculoVida;
        }
    }
}