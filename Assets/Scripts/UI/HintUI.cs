using System.Collections;
using TMPro;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text hintText;

    private Coroutine currentRoutine;

    public void Show(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        hintText.text = message;
        panel.SetActive(true);
    }

    public void Show(string message, float duration)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowRoutine(message, duration));
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    private IEnumerator ShowRoutine(string message, float duration)
    {
        hintText.text = message;
        panel.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        panel.SetActive(false);
    }
}
