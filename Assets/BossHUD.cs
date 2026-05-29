using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossHUD : MonoBehaviour
{
    public Animator anim;

    [Header("Referencias")]
    public BossVida bossVida;
    public BossIAFSM bossIAFSM;

    [Header("UI")]
    public Slider barraVida;
    public GameObject hudObject;

    private void Start()
{
    if (bossVida != null && barraVida != null)
    {
        barraVida.maxValue = bossVida.vidaMaxima;
        barraVida.value = bossVida.vidaAtual;
    }

    if (hudObject != null)
        hudObject.SetActive(false);
}

    private bool animacaoTocada;

private void Update()
{
    if (bossVida == null || barraVida == null)
        return;

    bool mostrar = bossIAFSM != null && bossIAFSM.VendoPlayer;

    if (mostrar)
    {
        hudObject.SetActive(true);

        if (!animacaoTocada)
        {
            animacaoTocada = true;
            StartCoroutine(PararAnimatorDepois());
        }
    }

    barraVida.value = bossVida.vidaAtual;

    if (bossVida.EstaMorto)
        hudObject.SetActive(false);
}

IEnumerator PararAnimatorDepois()
{
    yield return new WaitForSeconds(4f); // duração da animação

    anim.enabled = false;
}
}