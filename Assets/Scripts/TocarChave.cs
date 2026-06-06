using UnityEngine;
using UnityEngine.UI;

public class TocarChave : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;

    private bool coletada = false;

    private void Start()
{
    if(GameManager.Instance.possuiChave)
    {
        if(imagemDoCanvas != null && novaImagem != null)
            imagemDoCanvas.sprite = novaImagem;

        gameObject.SetActive(false);
    }
}

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

            GameManager.Instance.possuiChave = true;

            // Faz o objeto sumir
            gameObject.SetActive(false);
        }
    }
}