using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SistemaMoedas))]
public class HUDController : MonoBehaviour
{
    public Image[] hearts; // Array para armazenar as imagens dos coracoes
    public Sprite fullHeart; // Sprite do coracao cheio
    public Sprite halfHeart; // Sprite do coracao pela metade
    public Sprite emptyHeart; // Sprite do coracao vazio
    public bool jogoPausado { get; private set; }

    private float currentvida;
    private SistemaMoedas sistemaMoedas;
    private MenuPausa menuPausa;

    public static HUDController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        sistemaMoedas = GetComponent<SistemaMoedas>();
        menuPausa = GetComponentInChildren<MenuPausa>(includeInactive: true);
        AtualizarVida();
    }

    // M�todo que atualiza a interface dos coracoes
    public void AtualizarVida()
    {
        currentvida = PlayerController.Instance.vida;
        /*
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentvida / 2) // Se a posi��o do coracao est� totalmente preenchida
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i < currentvida / 2 + (currentvida % 2)) // Se o coracao est� pela metade
            {
                hearts[i].sprite = halfHeart;
            }
            else // coracao vazio
            {
                hearts[i].sprite = emptyHeart;
            }
        }
        */
        Debug.Log("Criar o UpdateVida");
    }

    public void ColetarMoeda(int valor)
    {
        sistemaMoedas.GanharMoedas(valor);
    }
    public bool GastarMoedas(int custo)
    {
        if (custo > sistemaMoedas.moedas)
        {
            return false;
        }
        else
        {
            sistemaMoedas.moedas -= custo;
            sistemaMoedas.AtualizarUIMoedas();
            return true;
        }
    }

    public void PausarJogo()
    {
        jogoPausado = true;
        Time.timeScale = 0;
        menuPausa.gameObject.SetActive(true);
    }

    public void RetornarJogo()
    {
        jogoPausado = false;
        Time.timeScale = 1;
        menuPausa.gameObject.SetActive(false);
    }
}
