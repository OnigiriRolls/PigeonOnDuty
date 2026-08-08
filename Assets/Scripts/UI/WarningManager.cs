using System.Collections;
using TMPro;
using UnityEngine;

public class WarningManager : MonoBehaviour
{
    public static WarningManager Instance { get; private set; }

    [SerializeField] private GameObject warningObject;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Hide();
    }

    public void Show(string title, string content)
    {
        titleText.text = title;
        contentText.text = content;
        warningObject.SetActive(true);
    }

    public void Hide()
    {
        warningObject.SetActive(false);
    }

    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    public void SetContent(string content)
    {
        contentText.text = content;
    }
}
