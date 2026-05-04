using UnityEngine;
using UnityEngine.UI;

public class TocarChave : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Troca a imagem no Canvas
            if (imagemDoCanvas != null && novaImagem != null)
            {
                imagemDoCanvas.sprite = novaImagem;
            }

            // Faz o objeto sumir
            Destroy(gameObject);
        }
    }
}