using NUnit.Framework;
using System.Collections;
using UnityEngine;

public class PedestrianPickupFeedback : MonoBehaviour
{
    [SerializeField] private SpriteRenderer feedbackCircle;
    [SerializeField] private Color correctCircleColor;
    [SerializeField] private Color wrongCircleColor;
    [SerializeField] private NPCDialogueUI dialogue;
    [SerializeField] private AudioClip correctClip;
    [SerializeField] private AudioClip wrongClip;
    [SerializeField] private string[] correctTexts;
    [SerializeField] private string[] wrongTexts;
    [SerializeField] private AudioSource audioSource;

    private Coroutine currentRoutine;

    public void PlayCorrect()
    {
        feedbackCircle.color = correctCircleColor;
        feedbackCircle.gameObject.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(correctClip, audioSource);
        dialogue.ShowMessage(correctTexts[Random.Range(0, correctTexts.Length)]);
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DeactivateFeedbackCircle());
    }

    public void PlayWrong()
    {
        feedbackCircle.color = wrongCircleColor;
        feedbackCircle.gameObject.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(wrongClip, audioSource);
        dialogue.ShowMessage(wrongTexts[Random.Range(0, wrongTexts.Length)]);
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DeactivateFeedbackCircle());
    }

    private IEnumerator DeactivateFeedbackCircle()
    {
        yield return new WaitForSeconds(3f);
        feedbackCircle.gameObject.SetActive(false);
    }
}
