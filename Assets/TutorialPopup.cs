using UnityEngine;
using TMPro;

public class TutorialPopup : MonoBehaviour
{
    public static TutorialPopup Instance;

    public GameObject painel;
    public TMP_Text textoTutorial;

    private string[] mensagens;
    private int indiceAtual;

    private void Awake()
    {
        Instance = this;
        painel.SetActive(false);
    }

    public void Mostrar(string mensagem)
    {
        textoTutorial.text = mensagem;

        painel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void MostrarSequencia(string[] tutoriais)
    {
        mensagens = tutoriais;
        indiceAtual = 0;

        textoTutorial.text = mensagens[indiceAtual];

        painel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Fechar()
    {
        if (mensagens != null && indiceAtual < mensagens.Length - 1)
        {
            indiceAtual++;
            textoTutorial.text = mensagens[indiceAtual];
            return;
        }

        mensagens = null;
        painel.SetActive(false);
        Time.timeScale = 1f;
    }
}