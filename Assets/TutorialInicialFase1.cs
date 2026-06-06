using UnityEngine;

public class TutorialInicialFase1 : MonoBehaviour
{
    private void Start()
    {
        TutorialPopup.Instance.MostrarSequencia(new string[]
        {
            "Use A e D para se movimentar.",

            "Pressione ESPAÇO para pular. Quanto mais tempo segurar o botão, maior será o salto.",

            "Clique com o BOTÃO ESQUERDO DO MOUSE para realizar o ataque básico."
        });
    }
}