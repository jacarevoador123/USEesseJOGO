using UnityEngine;

[CreateAssetMenu(fileName = "NovoUpgrade", menuName = "Sistema de Upgrade/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public string nomeUpgrade;
    public int custoMoedas;
    public bool ativado = false;
    public bool comprado = false;
    public string descricao;
}