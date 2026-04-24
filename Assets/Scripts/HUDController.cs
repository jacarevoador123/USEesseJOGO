using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SistemaMoedas))]
public class HUDController : MonoBehaviour
{
    public bool jogoPausado { get; private set; }

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
    }

    // 🔥 Mantido só pra compatibilidade (não faz mais nada)
    public void AtualizarVida()
    {
        // Agora quem cuida da vida é o SistemaDeVida + BarraDeVida
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