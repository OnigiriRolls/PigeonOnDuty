using UnityEngine;
using UnityEngine.UI;

public class NPCTrustUI : MonoBehaviour
{
    [SerializeField] private Image[] hearts;

    public void Refresh(int trust)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].enabled = i < trust;
    }
}
