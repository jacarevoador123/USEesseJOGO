using UnityEngine;
using UnityEngine.UI;

public class TocarAdaga : MonoBehaviour
{
    public Image imagemDoCanvas;
    public Sprite novaImagem;

    public SistemaMoedas sistemaMoedas;

    private bool jogadorEmCima = false;
    private bool coletada = false;

    private void Start()
{
    if(GameManager.Instance.podeShoot)
    {
        if(imagemDoCanvas != null && novaImagem != null)
            imagemDoCanvas.sprite = novaImagem;

        gameObject.SetActive(false);
    }
}

    private void Update()
    {
        if (coletada) return;

        if (jogadorEmCima && Input.GetKeyDown(KeyCode.E))
        {
            TryColetar();
        }
    }

    private void TryColetar()
    {
        if (sistemaMoedas != null && sistemaMoedas.TemMoedas(3))
        {
            sistemaMoedas.GastarMoedas(3);

            coletada = true;

            if (imagemDoCanvas != null && novaImagem != null)
                imagemDoCanvas.sprite = novaImagem;

            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
{
    GameManager.Instance.podeShoot = true;
    player.podeShoot = true;
}

TutorialPopup.Instance.Mostrar(
    "DESBLOQUEADO: ADAGA\n\nPressione o BOTÃO DIREITO DO MOUSE para lançar a adaga."
);

            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Você precisa de 3 moedas para pegar a adaga!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorEmCima = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            jogadorEmCima = false;
    }
}