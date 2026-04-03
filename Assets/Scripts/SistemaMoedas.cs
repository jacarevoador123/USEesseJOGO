using UnityEngine;
using TMPro;

public class SistemaMoedas : MonoBehaviour
{
    public int moedas = 0;
    public TextMeshProUGUI textoMoedas;

    void Start()
    {
        AtualizarUIMoedas();
    }

    public void GanharMoedas(int quantidade)
    {
        moedas += quantidade;
        AtualizarUIMoedas();
    }

    public void AtualizarUIMoedas()
    {
        if (textoMoedas != null)
            textoMoedas.text = "Moedas: " + moedas;
    }
}