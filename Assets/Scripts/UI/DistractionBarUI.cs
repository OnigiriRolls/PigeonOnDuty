using UnityEngine;
using UnityEngine.UI;

public class DistractionBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private HumanFollower human;

    private void Update()
    {
        if (human == null)
            return;
        slider.value = human.DistractionPercent;
    }
}
