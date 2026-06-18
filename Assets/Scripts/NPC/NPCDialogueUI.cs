using System.Collections;
using TMPro;
using UnityEngine;

public class NPCDialogueUI : MonoBehaviour
{
    [SerializeField] private GameObject dialog;
    [SerializeField] private TMP_Text messageText;

    private Coroutine currentRoutine;

    public void ShowMessage(string message, float duration = 3f)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;
        dialog.SetActive(true);
        yield return new WaitForSeconds(duration);
        dialog.SetActive(false);
    }
}
