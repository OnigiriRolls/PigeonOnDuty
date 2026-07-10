using UnityEngine;

[RequireComponent(typeof(NPCDialogueUI))]
public class ParadeChatter : MonoBehaviour
{
    [SerializeField] private string[] messages;
    [SerializeField] private float minInterval = 3f;
    [SerializeField] private float maxInterval = 8f;

    private NPCDialogueUI dialogueUI;
    private float timer;

    private void Awake()
    {
        dialogueUI = GetComponent<NPCDialogueUI>();
    }

    public void StartChatter()
    {
        timer = Random.Range(0f, maxInterval);
        enabled = true;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f)
            return;
        ShowRandomMessage();
        ScheduleNextMessage();
    }

    private void ShowRandomMessage()
    {
        if (messages.Length == 0)
            return;
        string message = messages[Random.Range(0, messages.Length)];
        dialogueUI.ShowMessage(message);
    }

    private void ScheduleNextMessage()
    {
        timer = Random.Range(minInterval, maxInterval);
    }
}
