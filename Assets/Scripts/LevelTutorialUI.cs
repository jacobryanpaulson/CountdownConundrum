using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelTutorialMessage
{
    public bool showTutorial = true;
    public string title;

    [TextArea(4, 10)]
    public string message;
}

public class LevelTutorialUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button continueButton;

    [Header("Level Messages")]
    [SerializeField] private LevelTutorialMessage[] levelTutorials;

    [Header("Settings")]
    [SerializeField] private bool pauseWhileOpen = true;
    [SerializeField] private KeyCode keyboardCloseKey = KeyCode.Return;

    private readonly HashSet<int> shownLevels =
        new HashSet<int>();

    public bool IsOpen =>
        tutorialPanel != null &&
        tutorialPanel.activeSelf;

    private void Awake()
    {
        ValidateReferences();

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(HideTutorial);
        }

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(HideTutorial);
        }
    }

    private void Update()
    {
        if (
            IsOpen &&
            Input.GetKeyDown(keyboardCloseKey)
        )
        {
            HideTutorial();
        }
    }

    public void ShowTutorialForLevel(int levelIndex)
    {
        Debug.Log(
            $"Tutorial requested for level index {levelIndex}."
        );

        // Put this component on the TutorialCanvas root.
        // This ensures the canvas object is active when called.
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        if (tutorialPanel == null)
        {
            Debug.LogError(
                "Tutorial cannot open because Tutorial Panel is not assigned.",
                this
            );

            return;
        }

        if (titleText == null)
        {
            Debug.LogError(
                "Tutorial cannot open because Title Text is not assigned.",
                this
            );

            return;
        }

        if (messageText == null)
        {
            Debug.LogError(
                "Tutorial cannot open because Message Text is not assigned.",
                this
            );

            return;
        }

        if (levelIndex < 0)
        {
            Debug.LogError(
                $"Tutorial level index cannot be negative: {levelIndex}.",
                this
            );

            return;
        }

        if (shownLevels.Contains(levelIndex))
        {
            Debug.Log(
                $"Tutorial for level index {levelIndex} was already shown."
            );

            return;
        }

        LevelTutorialMessage tutorial =
            GetTutorialForLevel(levelIndex);

        if (tutorial == null)
        {
            Debug.LogWarning(
                $"No tutorial is configured for level index {levelIndex}.",
                this
            );

            return;
        }

        if (!tutorial.showTutorial)
        {
            Debug.Log(
                $"Tutorial for level index {levelIndex} is disabled."
            );

            return;
        }

        shownLevels.Add(levelIndex);

        titleText.text =
            string.IsNullOrWhiteSpace(tutorial.title)
                ? $"LEVEL {levelIndex + 1}"
                : tutorial.title;

        messageText.text =
            string.IsNullOrWhiteSpace(tutorial.message)
                ? "No tutorial message has been entered."
                : tutorial.message;

        tutorialPanel.SetActive(true);

        // Forces Unity to refresh the UI immediately.
        Canvas.ForceUpdateCanvases();

        if (pauseWhileOpen)
        {
            Time.timeScale = 0f;
        }

        Debug.Log(
            $"Tutorial opened for level index {levelIndex}.",
            this
        );
    }

    public void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        if (
            pauseWhileOpen &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentState == GameState.Playing
        )
        {
            Time.timeScale = 1f;
        }
    }

    public void ResetTutorialProgress()
    {
        shownLevels.Clear();

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    // Useful for testing from a temporary Unity UI Button.
    public void ShowCurrentLevelTutorial()
    {
        int levelIndex =
            GameManager.Instance != null
                ? GameManager.Instance.currentPuzzleIndex
                : 0;

        ShowTutorialForLevel(levelIndex);
    }

    private LevelTutorialMessage GetTutorialForLevel(
        int levelIndex
    )
    {
        if (
            levelTutorials != null &&
            levelIndex < levelTutorials.Length &&
            levelTutorials[levelIndex] != null
        )
        {
            return levelTutorials[levelIndex];
        }

      
        if (levelIndex == 0)
        {
            return new LevelTutorialMessage
            {
                showTutorial = true,
                title = "LEVEL 1 — CONTROLS",
                message =
                    "WASD — Move one tile\n\n" +
                    "R — Restart the level\n\n" +
                    "Every successful move costs 1 step.\n\n" +
                    "Reach the goal before the countdown reaches 0."
            };
        }

        if (levelIndex == 1)
        {
            return new LevelTutorialMessage
            {
                showTutorial = true,
                title = "LEVEL 2 — CREATE A CLONE",
                message =
                    "Q — Start recording\n\n" +
                    "Move to record the clone's path\n\n" +
                    "Q — Stop recording and create the clone\n\n" +
                    "Clones repeat their path forward and backward.\n\n" +
                    "Recorded actions use the same shared countdown."
            };
        }

        return null;
    }

    private void ValidateReferences()
    {
        if (tutorialPanel == null)
        {
            Debug.LogError(
                "LevelTutorialUI: Tutorial Panel is not assigned.",
                this
            );
        }

        if (titleText == null)
        {
            Debug.LogError(
                "LevelTutorialUI: Title Text is not assigned.",
                this
            );
        }

        if (messageText == null)
        {
            Debug.LogError(
                "LevelTutorialUI: Message Text is not assigned.",
                this
            );
        }

        if (continueButton == null)
        {
            Debug.LogWarning(
                "LevelTutorialUI: Continue Button is not assigned. " +
                "The Return key can still close the tutorial.",
                this
            );
        }
    }
}