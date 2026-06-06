using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalDoJogo : MonoBehaviour
{
    public static FinalDoJogo Instance;

    public GameObject PainelDesbloqueado;
    public GameObject PainelFinal;

    private void Awake()
    {
        Instance = this;
    }

    public void BossDerrotado()
    {
        StartCoroutine(FinalRoutine());
    }

    IEnumerator FinalRoutine()
    {
        yield return new WaitForSeconds(6f);

        PainelDesbloqueado.SetActive(true);
    }

    public void ConfirmarDesbloqueio()
{
    StartCoroutine(MostrarAgradecimento());
}

IEnumerator MostrarAgradecimento()
{
    PainelDesbloqueado.SetActive(false);

    yield return new WaitForSeconds(8f);

    PainelFinal.SetActive(true);

    Time.timeScale = 0f;
}

    public void ReiniciarComAbissal()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Fase1");
    }

    public void VoltarMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Menu");
    }
}