using UnityEngine;

public class UIPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void Open()
    {
        panel.SetActive(true);
    }

    public void Close()
    {
        panel.SetActive(false);
    }

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);
    }
}
