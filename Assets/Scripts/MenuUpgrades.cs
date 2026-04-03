using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuUpgrades : MonoBehaviour
{
    public UpgradeData upgradePuloDuplo;
    public Button botaoPuloDuplo;
    public TextMeshProUGUI textoDescricao;

    void Start()
    {
        // Atualiza texto do botão
        botaoPuloDuplo.GetComponentInChildren<TextMeshProUGUI>().text =
            upgradePuloDuplo.nomeUpgrade + " - " + upgradePuloDuplo.custoMoedas + " moedas";
    }

    public void ComprarPuloDuplo()
    {
        if (!upgradePuloDuplo.comprado && HUDController.Instance.GastarMoedas(upgradePuloDuplo.custoMoedas))
        {
            upgradePuloDuplo.comprado = true;
            botaoPuloDuplo.interactable = false;
            PlayerController.Instance.AdicionarUpgrade(upgradePuloDuplo);
        }
    }

    public void MostrarDescricao(string descricao)
    {
        textoDescricao.text = descricao;
    }
}