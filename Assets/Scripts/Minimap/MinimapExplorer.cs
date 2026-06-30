using UnityEngine;

public class MinimapExplorer : MonoBehaviour
{
    [SerializeField] private FogPainter painter;
    [SerializeField] private float updateInterval = 0.1f;

    private float timer;
    private Vector3 lastRevealPosition;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < updateInterval)
            return;
        timer = 0f;
        if (Vector3.Distance(transform.position, lastRevealPosition) > 2f)
        {
            painter.Reveal(transform.position);
            lastRevealPosition = transform.position;
        }
    }
}
