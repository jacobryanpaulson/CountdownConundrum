using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPlayback : MonoBehaviour
{
    private List<LoopStep> savedPath =
        new List<LoopStep>();

    private int currentStepIndex;
    private bool movingForward = true;

    private Tilemap groundTilemap;
    private Tilemap collisionTilemap;

    private void OnEnable()
    {
        PlayerController.OnPlayerMoved += MoveGhost;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerMoved -= MoveGhost;
    }

    public void SetPath(
        List<LoopStep> pathHistory,
        Tilemap newGroundTilemap,
        Tilemap newCollisionTilemap
    )
    {
        savedPath =
            new List<LoopStep>(pathHistory);

        groundTilemap =
            newGroundTilemap;

        collisionTilemap =
            newCollisionTilemap;

        currentStepIndex = 0;
        movingForward = true;
    }

    private void MoveGhost()
    {
        if (savedPath == null || savedPath.Count == 0)
        {
            return;
        }

        LoopStep currentStep =
            savedPath[currentStepIndex];

        ExecuteStep(currentStep);

        UpdatePlaybackPosition();
    }

    private void ExecuteStep(LoopStep step)
    {
        switch (step.StepType)
        {
            case LoopStepType.Move:
                ExecuteMovementStep(step);
                break;

            case LoopStepType.Teleport:
                ExecuteTeleportStep(step);
                break;
        }
    }

    private void ExecuteMovementStep(LoopStep step)
    {
        if (
            groundTilemap == null ||
            collisionTilemap == null
        )
        {
            Debug.LogError(
                $"{gameObject.name} is missing its Tilemap references."
            );

            return;
        }

        Vector2 playbackDirection =
            movingForward
                ? step.Direction
                : -step.Direction;

        GridMovement.TryMoveActor(
            transform,
            playbackDirection,
            groundTilemap,
            collisionTilemap
        );
    }

    private void ExecuteTeleportStep(LoopStep step)
    {
        if (movingForward)
        {
            transform.position =
                step.TeleportDestination;
        }
        else
        {
            transform.position =
                step.TeleportStartPosition;
        }
    }

    private void UpdatePlaybackPosition()
    {
        if (movingForward)
        {
            if (currentStepIndex >= savedPath.Count - 1)
            {
                movingForward = false;
            }
            else
            {
                currentStepIndex++;
            }
        }
        else
        {
            if (currentStepIndex <= 0)
            {
                movingForward = true;
            }
            else
            {
                currentStepIndex--;
            }
        }
    }
}