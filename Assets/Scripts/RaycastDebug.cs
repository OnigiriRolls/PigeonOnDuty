using UnityEngine;

public class RaycastDebug : MonoBehaviour
{
    [SerializeField] private float rayLength = 20f;

    private void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);
    }
}
