using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void IniciarJogo(string nomeFase)
    {
        SceneManager.LoadScene(nomeFase);
    }

    public void Sair()
    {
        Application.Quit();
    }
}
