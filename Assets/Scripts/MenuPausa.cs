using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPausa : MonoBehaviour
{
    public void Retornar()
    {
        HUDController.Instance.RetornarJogo();
    }

    public void Sair()
    {
        Debug.Log("Sair do jogo");
    }
}
