using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void TremerCamera(float duracao = -1)
    {
        Camera.main.GetComponent<CameraFollow>().Tremer(duracao);
    }
}
