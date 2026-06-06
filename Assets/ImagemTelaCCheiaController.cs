using UnityEngine;

public class ImagemTelaCCheiaController : MonoBehaviour
{
    [Header("Painel ou Imagem para mostrar")]
    public GameObject telaImagem; // arraste aqui no Inspector

    private bool desbloqueado = false;

    void Start()
    {
        // Garante que o painel comece desativado
        if (telaImagem != null)
            telaImagem.SetActive(false);
        else
            Debug.LogWarning("telaImagem não foi arrastado no Inspector!");
    }

    void Update()
    {
        if (telaImagem == null) return; // evita NullReference

        // Primeiro fechamento inicial
        if (!desbloqueado)
        {
            if (telaImagem.activeSelf && Input.GetKeyDown(KeyCode.M))
            {
                FecharImagem();
                desbloqueado = true;
            }
        }
        else
        {
            // Abrir/Fechar com M
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (!telaImagem.activeSelf)
                    AbrirImagem();
                else
                    FecharImagem();
            }

            // Fechar com ESC
            if (telaImagem.activeSelf && Input.GetKeyDown(KeyCode.M))
            {
                FecharImagem();
            }
        }
    }

    // Chamado externamente (ex: TocarBilhete)
    public void MostrarImagemInicial()
    {
        if (telaImagem == null) return;
        AbrirImagem();
    }

    private void AbrirImagem()
    {
        telaImagem.SetActive(true);
        // Pausa o jogo
        Time.timeScale = 0f;

        // Garante que o painel esteja na frente de tudo
        Canvas canvas = telaImagem.GetComponentInParent<Canvas>();
        if (canvas != null)
            canvas.sortingOrder = 999;
    }

    private void FecharImagem()
    {
        telaImagem.SetActive(false);
        Time.timeScale = 1f; // volta o jogo
    }
}
