using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class TrocarCena : MonoBehaviour
{
    public string nomeDaCena;
    public Image fadeImage;
    public float duracaoFade = 1f;

    private bool jogadorPerto = false;

    void Update()
    {
        if (jogadorPerto && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(CarregarCena());
        }
    }

    IEnumerator CarregarCena()
    {
        Color cor = fadeImage.color;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
            fadeImage.color = cor;
            yield return null;
        }

        SceneManager.LoadScene(nomeDaCena);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorPerto = false;
    }
}