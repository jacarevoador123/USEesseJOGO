using UnityEngine;
using TMPro;

public class SistemaMoedas : MonoBehaviour
{
    public int moedas = 0;
    public TextMeshProUGUI textoMoedas;

    void Start()
    {
        moedas = GameManager.Instance.moedas;
        AtualizarUIMoedas();
    }

    public void GanharMoedas(int quantidade)
    {
        moedas += quantidade;
        GameManager.Instance.moedas = moedas;
        AtualizarUIMoedas();
    }

    public void AtualizarUIMoedas()
    {
        if (textoMoedas != null)
            textoMoedas.text = "Moedas: " + moedas;
    }

    public bool TemMoedas(int quantidade)
{
    return moedas >= quantidade;
}

public void GastarMoedas(int quantidade)
{
    moedas -= quantidade;
    AtualizarUIMoedas();
}
}