using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTransition : MonoBehaviour
{
    public bool IsBlack => isBlack;

    [SerializeField] private Image overlay;
    [SerializeField] private float fadeDuration = 0.4f;

    private Coroutine currentTransition;
    private bool isBlack;

    private void Awake()
    {
        overlay.color = new Color(0f, 0f, 0f, 0f);
        overlay.gameObject.SetActive(true);
    }

    public void FadeToBlack()
    {
        isBlack = true;
        StartTransition(0f, 1f);
    }

    public void FadeFromBlack()
    {
        StartTransition(1f, 0f);
        isBlack = false;
    }

    private void StartTransition(float from, float to)
    {
        if (currentTransition != null)
            StopCoroutine(currentTransition);
        currentTransition = StartCoroutine(FadeCoroutine(from, to));
    }

    private IEnumerator FadeCoroutine(float from, float to)
    {
        float timer = 0f;
        Color color = overlay.color;
        color.a = from;
        overlay.color = color;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            t = Mathf.SmoothStep(0f, 1f, t);
            color.a = Mathf.Lerp(from, to, t);
            overlay.color = color;
            yield return null;
        }

        color.a = to;
        overlay.color = color;
        currentTransition = null;
    }
}
