using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Progressão do Player")]
    public bool podeDash;
    public bool podeShoot;
    public bool podeAbissal;
    public bool possuiChave;
    public int moedas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetarProgresso()
{
    podeDash = false;
    podeShoot = false;
    podeAbissal = false;
    possuiChave = false;
    moedas = 0;
}
}