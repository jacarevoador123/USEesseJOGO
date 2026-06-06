using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    private Vector3 checkpointAtual;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 pos)
    {
        checkpointAtual = pos;
        Debug.Log("Checkpoint salvo: " + checkpointAtual);
    }

    public Vector3 GetCheckpoint()
    {
        return checkpointAtual;
    }
}