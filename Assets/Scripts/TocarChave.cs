using UnityEngine;
using UnityEngine.UI;

public class TocarChave : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;

    private bool coletada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // impede coletar mais de uma vez
        if (coletada) return;

        if (other.CompareTag("Player"))
        {
            coletada = true;

            // Troca a imagem no Canvas
            if (imagemDoCanvas != null && novaImagem != null)
            {
                imagemDoCanvas.sprite = novaImagem;
            }

            // Faz o objeto sumir
            gameObject.SetActive(false);
        }
    }
}