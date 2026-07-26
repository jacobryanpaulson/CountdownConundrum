using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    MainMenu,
    Playing,
    Pause,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }

    public static GameManager Instance { get; private set; }

    [Header("Puzzle References")]
    public Camera[] puzzleCam;
    public Transform[] spawnPoints;
    public LevelGoal[] levelGoals;
    public int[] puzzleMoveLimits;
    public GameObject player;

    [Header("Current Puzzle")]
    public int currentPuzzleIndex;

    [Header("Menu UI")]
    public GameObject menuCanvas;

    [Header("Level Transition")]
    public float levelTransitionDelay = 2f;
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Tutorial UI")]
    [SerializeField] private LevelTutorialUI tutorialUI;

    private bool isChangingPuzzle;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (tutorialUI == null)
        {
            tutorialUI =
                FindFirstObjectByType<LevelTutorialUI>(
                    FindObjectsInactive.Include
                );
        }

        if (tutorialUI == null)
        {
            Debug.LogError(
                "GameManager could not find LevelTutorialUI. " +
                "Make sure the TutorialCanvas has the component attached."
            );
        }
    }

    private void Start()
    {
        CurrentState = GameState.MainMenu;

        HandleStateChange();
        ResetToFirstPuzzle();
    }

    private void Update()
    {
        HandlePauseInput();
    }

    public void PlayButton()
    {
        ChangeState(GameState.Playing);
    }

    public void QuitButton()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void HandlePauseInput()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }

        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Pause);
        }
        else if (CurrentState == GameState.Pause)
        {
            ChangeState(GameState.Playing);
        }
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
        {
            return;
        }

        bool isPauseChange =
            newState == GameState.Pause ||
            (
                newState == GameState.Playing &&
                CurrentState == GameState.Pause
            );

        if (isPauseChange)
        {
            CurrentState = newState;
            HandleStateChange();
        }
        else
        {
            StartCoroutine(
                TransitionToState(newState)
            );
        }
    }

    private IEnumerator TransitionToState(
        GameState newState
    )
    {
        yield return new WaitForSecondsRealtime(0.5f);

        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;

                if (menuCanvas != null)
                {
                    menuCanvas.SetActive(true);
                }

                break;

            case GameState.Playing:
                Time.timeScale = 1f;

                if (menuCanvas != null)
                {
                    menuCanvas.SetActive(false);
                }

                if (tutorialUI != null)
                {
                    tutorialUI.ShowTutorialForLevel(
                        currentPuzzleIndex
                    );
                }

                break;

            case GameState.Pause:
                Time.timeScale = 0f;

                if (menuCanvas != null)
                {
                    menuCanvas.SetActive(true);
                }

                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }

    public void ResetToFirstPuzzle()
    {
        currentPuzzleIndex = 0;
        isChangingPuzzle = false;

        if (tutorialUI != null)
        {
            tutorialUI.ResetTutorialProgress();
        }

        if (levelGoals != null)
        {
            for (int i = 0; i < levelGoals.Length; i++)
            {
                if (levelGoals[i] != null)
                {
                    levelGoals[i].ResetGoal();
                }
            }
        }

        if (
            player != null &&
            player.TryGetComponent<PlayerController>(
                out PlayerController playerController
            )
        )
        {
            if (
                spawnPoints != null &&
                spawnPoints.Length > 0 &&
                spawnPoints[0] != null
            )
            {
                player.transform.position =
                    spawnPoints[0].position;

                if (LoopManager.Instance != null)
                {
                    LoopManager.Instance.UpdateSpawnPoint(
                        spawnPoints[0]
                    );
                }
            }

            if (
                levelGoals != null &&
                levelGoals.Length > 0
            )
            {
                playerController.UpdatePuzzleReferences(
                    levelGoals[0]
                );
            }

            if (
                puzzleMoveLimits != null &&
                puzzleMoveLimits.Length > 0
            )
            {
                playerController.ResetMoves(
                    puzzleMoveLimits[0]
                );
            }
        }

        if (puzzleCam == null)
        {
            return;
        }

        for (int i = 0; i < puzzleCam.Length; i++)
        {
            if (puzzleCam[i] != null)
            {
                puzzleCam[i].gameObject.SetActive(
                    i == 0
                );
            }
        }
    }

    public void StartDelayedAdvance()
    {
        if (isChangingPuzzle)
        {
            return;
        }

        StartCoroutine(
            AdvanceToNextPuzzle()
        );
    }

    private IEnumerator AdvanceToNextPuzzle()
    {
        isChangingPuzzle = true;

        yield return new WaitForSeconds(0.5f);

        yield return FadeToBlack();

        int nextIndex =
            currentPuzzleIndex + 1;

        if (
            puzzleCam == null ||
            nextIndex >= puzzleCam.Length
        )
        {
            Debug.Log("All puzzles completed!");

            ResetToFirstPuzzle();

            if (fadeImage != null)
            {
                fadeImage.color =
                    new Color(0f, 0f, 0f, 0f);
            }

            CurrentState = GameState.MainMenu;
            HandleStateChange();

            isChangingPuzzle = false;
            yield break;
        }

        if (
            spawnPoints == null ||
            nextIndex >= spawnPoints.Length ||
            spawnPoints[nextIndex] == null
        )
        {
            Debug.LogError(
                $"Missing spawn point for puzzle index {nextIndex}."
            );

            isChangingPuzzle = false;
            yield break;
        }

        if (
            levelGoals != null &&
            currentPuzzleIndex < levelGoals.Length &&
            levelGoals[currentPuzzleIndex] != null
        )
        {
            levelGoals[currentPuzzleIndex].ResetGoal();
        }

        Camera currentCamera =
            puzzleCam[currentPuzzleIndex];

        Camera nextCamera =
            puzzleCam[nextIndex];

        Transform nextSpawn =
            spawnPoints[nextIndex];

        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.UpdateSpawnPoint(
                nextSpawn
            );
        }

        if (player != null)
        {
            player.transform.position =
                nextSpawn.position;

            if (
                player.TryGetComponent<PlayerController>(
                    out PlayerController playerController
                )
            )
            {
                if (
                    levelGoals != null &&
                    nextIndex < levelGoals.Length
                )
                {
                    playerController.UpdatePuzzleReferences(
                        levelGoals[nextIndex]
                    );
                }

                if (
                    puzzleMoveLimits != null &&
                    nextIndex < puzzleMoveLimits.Length
                )
                {
                    playerController.ResetMoves(
                        puzzleMoveLimits[nextIndex]
                    );
                }
            }
        }

        if (nextCamera != null)
        {
            nextCamera.gameObject.SetActive(true);
        }

        if (currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }

        currentPuzzleIndex =
            nextIndex;

        yield return new WaitForSeconds(
            levelTransitionDelay
        );

        yield return FadeFromBlack();

        // This must happen after currentPuzzleIndex changes
        // and after the new level has faded into view.
        if (tutorialUI != null)
        {
            tutorialUI.ShowTutorialForLevel(
                currentPuzzleIndex
            );
        }

        isChangingPuzzle = false;
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Clamp01(
                elapsedTime / fadeDuration
            );

            fadeImage.color =
                new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fadeImage.color =
            new Color(0f, 0f, 0f, 1f);
    }

    private IEnumerator FadeFromBlack()
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Clamp01(
                1f - elapsedTime / fadeDuration
            );

            fadeImage.color =
                new Color(0f, 0f, 0f, alpha);

            yield return null;
        }

        fadeImage.color =
            new Color(0f, 0f, 0f, 0f);
    }
}