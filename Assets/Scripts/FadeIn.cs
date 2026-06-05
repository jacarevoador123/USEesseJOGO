using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;
    public float duracaoFade = 1f;

    void Start()
    {
        StartCoroutine(FazerFadeIn());
    }

    IEnumerator FazerFadeIn()
    {
        Color cor = fadeImage.color;
        cor.a = 1f;
        fadeImage.color = cor;

        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(1f, 0f, tempo / duracaoFade);
            fadeImage.color = cor;
            yield return null;
        }

        cor.a = 0f;
        fadeImage.color = cor;
    }
}