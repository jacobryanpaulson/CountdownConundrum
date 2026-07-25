using System;
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

        if (stepText != null)
        {
            stepText.text =
                "Steps Remaining: " + currentMovesRemaining;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Manual Reset Triggered!");

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

        direction = GridMovement.GetCardinalDirection(direction);

        bool movementSucceeded = GridMovement.TryMoveActor(
            transform,
            direction,
            groundTilemap,
            collisionTilemap
        );
        

        if (!movementSucceeded)
        {
            return;
        }
        

        currentMovesRemaining--;

        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.RecordMove(direction);
        }

        OnPlayerMoved?.Invoke();

        if (stepText != null)
        {
            stepText.text =
                "Steps Remaining: " + currentMovesRemaining;
        }

        if (
            levelGoal != null &&
            levelGoal.CheckForCompletion(transform.position)
        )
        {
            return;
        }

        CheckMoveLimit();
    }

    public void TeleportTo(Vector3 destinationPosition)
    {
        if (!canMove || currentMovesRemaining <= 0)
        {
            return;
        }

        currentMovesRemaining--;
        transform.position = destinationPosition;
        

        if (LoopManager.Instance != null)
        {
            LoopManager.Instance.RecordTeleport(transform.position);
        }

        OnPlayerMoved?.Invoke();

        if (
            levelGoal != null &&
            levelGoal.CheckForCompletion(transform.position)
        )
        {
            return;
        }

        CheckMoveLimit();
    }

    private void CheckMoveLimit()
    {
        if (
            currentMovesRemaining <= 0 &&
            LoopManager.Instance != null
        )
        {
            LoopManager.Instance.ResetLoop();
        }
    }

    public void ResetMoves(int newMaxMoves = -1)
    {
        if (newMaxMoves > 0)
        {
            maxMoves = newMaxMoves;
        }

        currentMovesRemaining = maxMoves;

        if (stepText != null)
        {
            stepText.text =
                "Steps Remaining: " + currentMovesRemaining;
        }

        canMove = true;
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