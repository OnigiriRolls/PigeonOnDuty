using UnityEngine;
using UnityEngine.InputSystem;

public class InputDisplayHelper : MonoBehaviour
{
    public static InputDisplayHelper Instance { get; private set; }

    [SerializeField] private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
    }

    public string GetDisplayName(string actionName)
    {
        InputAction action = playerInput.actions.FindAction(actionName);
        if (action == null)
            return actionName;
        return action.GetBindingDisplayString();
    }

    public string GetHint(string actionName)
    {
        return $"[{GetDisplayName(actionName)}]";
    }

    public string GetCompositePartDisplayName(string actionName, string partName)
    {
        InputAction action = playerInput.actions.FindAction(actionName);
        if (action == null)
            return actionName;
        for (int i = 0; i < action.bindings.Count; i++)
        {
            InputBinding binding = action.bindings[i];
            if (!binding.isPartOfComposite)
                continue;
            if (binding.name != partName)
                continue;
            return action.GetBindingDisplayString(i);
        }
        return partName;
    }
}
