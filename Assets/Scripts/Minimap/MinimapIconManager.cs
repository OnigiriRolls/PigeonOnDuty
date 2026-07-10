using System.Collections.Generic;
using UnityEngine;

public class MinimapIconManager : MonoBehaviour
{
    [SerializeField] private RectTransform iconsParent;
    [SerializeField] private MinimapScroller minimapScroller;

    private readonly Dictionary<Transform, MinimapIcon> icons = new();

    public void CreateIcon(MinimapIcon prefab, Transform target)
    {
        MinimapIcon icon = Instantiate(prefab, iconsParent);
        icon.Initialize(target, minimapScroller);
        icons.Add(target, icon);
    }

    public void RemoveIcon(Transform target)
    {
        if (!icons.TryGetValue(target, out MinimapIcon icon))
            return;
        Destroy(icon.gameObject);
        icons.Remove(target);
    }
}
