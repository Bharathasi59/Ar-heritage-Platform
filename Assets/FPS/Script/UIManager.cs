using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI promptText;
    public GameObject choicePanel;

    private Action onExplain;
    private Action onSkip;

    void Awake()
    {
        Instance = this;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    public void ShowPrompt(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
            promptText.gameObject.SetActive(true);
        }
    }

    public void HidePrompt()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    public void ShowChoices(Action onExplain, Action onSkip)
    {
        this.onExplain = onExplain;
        this.onSkip = onSkip;

        if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    public void HideChoices()
    {
        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    // ✅ These must be PUBLIC and return VOID with no parameters
    public void OnExplainClicked()
    {
        Debug.Log("Player chose Explain Temple");
        onExplain?.Invoke();
        HideChoices();
    }

    public void OnSkipClicked()
    {
        Debug.Log("Player chose Skip");
        onSkip?.Invoke();
        HideChoices();
    }
}
