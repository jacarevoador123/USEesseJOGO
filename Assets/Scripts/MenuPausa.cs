using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public void Retornar()
    {
        HUDController.Instance.RetornarJogo();
    }

    public void ReiniciarFase()
    {
        Time.timeScale = 1f;

        Scene cenaAtual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(cenaAtual.name);
    }

    public void Sair()
    {
        Time.timeScale = 1f;

        if (HUDController.Instance != null)
        {
            HUDController.Instance.RetornarJogo();
        }

        SceneManager.LoadScene("Menu");
    }
}