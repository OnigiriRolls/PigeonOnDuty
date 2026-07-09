using UnityEngine;
using UnityEngine.UI;

public class PedestrianCarryVisual : MonoBehaviour
{
    [SerializeField] private Image carriedItemIcon;

    public void Show(ThrowableData item)
    {
        if (item == null)
            return;
        carriedItemIcon.sprite = item.icon;
        carriedItemIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        carriedItemIcon.gameObject.SetActive(false);
    }
}
