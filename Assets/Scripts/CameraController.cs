using System;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform[] povs;
    [SerializeField] float speed;
    [SerializeField] private PlayerInputController input;

    private int povIndex = 0;

    private void Start()
    {
        transform.position = povs[0].position;
    }

    private void OnEnable()
    {
        input.OnCamera1 += SetCamera1;
        input.OnCamera2 += SetCamera2;
        input.OnCamera3 += SetCamera3;
        input.OnChangeView += ChangeView;
    }

    private void OnDisable()
    {
        input.OnCamera1 -= SetCamera1;
        input.OnCamera2 -= SetCamera2;
        input.OnCamera3 -= SetCamera3;
        input.OnChangeView -= ChangeView;
    }

    private void ChangeView()
    {
        if (povIndex == 2)
        {
            povIndex = 0;
            return;
        }

        povIndex++;
    }

    private void SetCamera1()
    {
        povIndex = 0;
    }

    private void SetCamera2()
    {
        povIndex = 1;
    }

    private void SetCamera3()
    {
        povIndex = 2;
    }

    private void FixedUpdate()
    {
        MoveCamera(povIndex);
    }

    private void MoveCamera(int povIndex)
    {
        transform.position = Vector3.MoveTowards(transform.position, povs[povIndex].position, Time.deltaTime * speed);
        transform.forward = povs[povIndex].forward;
    }
}
