using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;

    [Header("Puzzle Settings")]
    [SerializeField] private int maxMoves = 10;
    [SerializeField] private LevelGoal levelGoal;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI stepText;

    public static Action OnPlayerMoved;

    public Tilemap GroundTilemap => groundTilemap;
    public Tilemap CollisionTilemap => collisionTilemap;

    public Animator animator;

    private GridInputMovement controls;
    private int currentMovesRemaining;
    private bool canMove = true;

    private bool isRecordingClone;
    private Vector3 cloneRecordingStartPosition;

    private readonly List<LoopStep> recordedClonePath =
        new List<LoopStep>();

    private void Awake()
    {
        controls = new GridInputMovement();
        currentMovesRemaining = maxMoves;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        controls.Movement.Movement.performed +=
            context => Move(context.ReadValue<Vector2>());

        UpdateStepText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleCloneRecording();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Manual Reset Triggered!");

            CancelCloneRecording();

            if (LoopManager.Instance != null)
            {
                LoopManager.Instance.FullResetLevel();
            }
        }
    }

    private void Move(Vector2 direction)
    {
        if (!canMove || currentMovesRemaining <= 0)
        {
            return;
        }

        direction =
            GridMovement.GetCardinalDirection(direction);

        bool movementSucceeded =
            GridMovement.TryMoveActor(
                transform,
                direction,
                groundTilemap,
                collisionTilemap
            );

        if (!movementSucceeded)
        {
            return;
        }

        SpendStep();

        if (isRecordingClone)
        {
            recordedClonePath.Add(
                LoopStep.CreateMove(direction)
            );
        }

        OnPlayerMoved?.Invoke();

        if (
            levelGoal != null &&
            levelGoal.CheckForCompletion(transform.position)
        )
        {
            canMove = false;
            return;
        }

        HandleEndOfStepBudget();
    }

   public void TeleportTo(Vector3 destinationPosition)
{
    if (!canMove || currentMovesRemaining <= 0)
    {
        return;
    }

    Vector3 teleportStartPosition =
        transform.position;

    transform.position = destinationPosition;

    SpendStep();

    if (isRecordingClone)
    {
        recordedClonePath.Add(
            LoopStep.CreateTeleport(
                teleportStartPosition,
                destinationPosition
            )
        );
    }

    OnPlayerMoved?.Invoke();

    if (
        levelGoal != null &&
        levelGoal.CheckForCompletion(transform.position)
    )
    {
        canMove = false;
        return;
    }

    HandleEndOfStepBudget();
}
    private void SpendStep()
    {
        currentMovesRemaining--;
        UpdateStepText();
    }

    private void HandleEndOfStepBudget()
    {
        if (currentMovesRemaining > 0)
        {
            return;
        }

        if (isRecordingClone)
        {
            FinishCloneRecording();
        }

        canMove = false;

        Debug.Log(
            "Out of steps! Press R to restart the puzzle."
        );

        UpdateStepText();
    }

    private void ToggleCloneRecording()
    {
        if (isRecordingClone)
        {
            FinishCloneRecording();
        }
        else
        {
            BeginCloneRecording();
        }
    }

    private void BeginCloneRecording()
    {
        if (!canMove || currentMovesRemaining <= 0)
        {
            Debug.LogWarning(
                "There are no steps available for recording."
            );

            return;
        }

        if (LoopManager.Instance == null)
        {
            Debug.LogError(
                "Cannot record a clone because LoopManager is missing."
            );

            return;
        }

        if (!LoopManager.Instance.CanCreateClone())
        {
            Debug.LogWarning(
                "The maximum number of clones has been reached."
            );

            return;
        }

        isRecordingClone = true;
        recordedClonePath.Clear();

        cloneRecordingStartPosition =
            transform.position;

        UpdateStepText();

        Debug.Log(
            "Clone recording started at " +
            cloneRecordingStartPosition
        );
    }

    private void FinishCloneRecording()
    {
        if (!isRecordingClone)
        {
            return;
        }

        isRecordingClone = false;

        if (recordedClonePath.Count == 0)
        {
            Debug.LogWarning(
                "Recording ended without any successful actions."
            );

            recordedClonePath.Clear();
            UpdateStepText();
            return;
        }

        bool cloneWasCreated = false;

        if (LoopManager.Instance != null)
        {
            cloneWasCreated =
                LoopManager.Instance.CreateClone(
                    cloneRecordingStartPosition,
                    recordedClonePath
                );
        }

        if (cloneWasCreated)
        {
            Debug.Log(
                "Clone created with " +
                recordedClonePath.Count +
                " recorded actions."
            );
        }

        recordedClonePath.Clear();
        UpdateStepText();
    }

    private void CancelCloneRecording()
    {
        isRecordingClone = false;
        recordedClonePath.Clear();

        UpdateStepText();
    }

    private void UpdateStepText()
    {
        if (stepText == null)
        {
            return;
        }

        string recordingMessage =
            isRecordingClone
                ? "\nRECORDING CLONE"
                : "\nPress Q to Record";

        if (!canMove && currentMovesRemaining <= 0)
        {
            recordingMessage =
                "\nOut of Steps - Press R";
        }

        stepText.text =
            "Steps Remaining: " +
            currentMovesRemaining +
            recordingMessage;
    }

    public void ResetMoves(int newMaxMoves = -1)
    {
        if (newMaxMoves > 0)
        {
            maxMoves = newMaxMoves;
        }

        isRecordingClone = false;
        recordedClonePath.Clear();

        currentMovesRemaining = maxMoves;
        canMove = true;

        UpdateStepText();
    }

    public static void ClearMovementEvents()
    {
        OnPlayerMoved = null;
    }

    public void UpdatePuzzleReferences(LevelGoal newGoal)
    {
        levelGoal = newGoal;

        Debug.Log(
            "Player successfully synced to the new puzzle's grid and goal!"
        );
    }
}