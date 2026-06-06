using UnityEngine;
using UnityEngine.UI;

public class TocarBilhete : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;
    public ImagemTelaCCheiaController controlador; // adicionar isto

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (imagemDoCanvas != null && novaImagem != null)
                imagemDoCanvas.sprite = novaImagem;

            // Ativa a imagem em tela cheia + pausa o jogo
            if (controlador != null)
                controlador.MostrarImagemInicial();

            Destroy(gameObject);
        }
    }
}
