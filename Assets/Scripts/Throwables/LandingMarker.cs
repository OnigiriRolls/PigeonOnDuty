using UnityEngine;

public class LandingMarker : MonoBehaviour
{
    public void Show(Vector3 position)
    {
        gameObject.SetActive(true);
        transform.position = position + Vector3.up * 0.05f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
