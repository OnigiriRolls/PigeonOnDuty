using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    public float Roll { get; private set; }
    public float Pitch { get; private set; }
    public bool ThrottleIncreaseHeld { get; private set; }
    public bool ThrottleDecreaseHeld { get; private set; }
    public bool HoverHeld { get; private set; }
    public bool InteractHeld { get; private set; }
    public event Action OnCamera1;
    public event Action OnCamera2;
    public event Action OnCamera3;
    public event Action OnToggleNPCFollow;
    public event Action OnInteract;
    public event Action OnPause;
    public event Action OnThrowPressed;
    public event Action OnThrowReleased;
    public event Action OnCancelThrow;
    public event Action OnSelectItem;
    public event Action OnChangeView;

    [SerializeField] private string defaultActionMap = "Gameplay";

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        playerInput.SwitchCurrentActionMap(defaultActionMap);
    }

    public void OnRoll(CallbackContext context)
    {
        Roll = context.ReadValue<float>();
    }

    public void OnPitch(CallbackContext context)
    {
        Pitch = context.ReadValue<float>();
    }

    public void OnThrottleIncrease(CallbackContext context)
    {
        ThrottleIncreaseHeld = context.ReadValueAsButton();
    }

    public void OnThrottleDecrease(CallbackContext context)
    {
        ThrottleDecreaseHeld = context.ReadValueAsButton();
    }

    public void OnHover(CallbackContext context)
    {
        HoverHeld = context.ReadValueAsButton();
    }

    public void OnCamera1Input(CallbackContext context)
    {
        if (context.performed)
            OnCamera1?.Invoke();
    }

    public void OnCamera2Input(CallbackContext context)
    {
        if (context.performed)
            OnCamera2?.Invoke();
    }

    public void OnCamera3Input(CallbackContext context)
    {
        if (context.performed)
            OnCamera3?.Invoke();
    }

    public void OnToggleNPCFollowInput(CallbackContext context)
    {
        if (context.performed)
            OnToggleNPCFollow?.Invoke();
    }

    public void OnInteractInput(CallbackContext context)
    {
        InteractHeld = context.ReadValueAsButton();
        if (context.performed)
            OnInteract?.Invoke();
    }

    public void OnPauseInput(CallbackContext context)
    {
        if (context.performed)
            OnPause?.Invoke();
    }

    public void OnThrowInput(InputAction.CallbackContext context)
    {
        if (context.started)
            OnThrowPressed?.Invoke();
        if (context.canceled)
            OnThrowReleased?.Invoke();
    }

    public void OnCancelThrowInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCancelThrow?.Invoke();
    }

    public void OnSelectItemInput(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectItem?.Invoke();
    }

    public void MobileChangeView()
    {
        OnChangeView?.Invoke();
    }
}
