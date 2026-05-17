using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public AltitudeLayer AltitudeLayer;

    private void OnDrawGizmos()
    {
        switch (AltitudeLayer)
        {
            case AltitudeLayer.Low:
                Gizmos.color = Color.green;
                break;

            case AltitudeLayer.Mid:
                Gizmos.color = Color.yellow;
                break;

            case AltitudeLayer.High:
                Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawSphere(transform.position, 1f);
    }
}
