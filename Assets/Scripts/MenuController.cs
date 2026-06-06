using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void IniciarJogo(string nomeFase)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetarProgresso();
        }

        SceneManager.LoadScene(nomeFase);
    }

    public void Sair()
    {
        Application.Quit();
    }
}