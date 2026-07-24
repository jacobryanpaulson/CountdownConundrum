using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostPlayback : MonoBehaviour
{
    private List<LoopStep> savedPath = new List<LoopStep>();
    private int currentStepIndex;

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
        savedPath = new List<LoopStep>(pathHistory);
        groundTilemap = newGroundTilemap;
        collisionTilemap = newCollisionTilemap;
        currentStepIndex = 0;
    }

    private void MoveGhost()
    {
        if (currentStepIndex >= savedPath.Count)
        {
            Debug.Log("Ghost Has Completed its path");
            return;
        }

        LoopStep currentStep = savedPath[currentStepIndex];
        currentStepIndex++;

        switch (currentStep.StepType)
        {
            case LoopStepType.Move:
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

                GridMovement.TryMoveActor(
                    transform,
                    currentStep.Direction,
                    groundTilemap,
                    collisionTilemap
                );
                break;

            case LoopStepType.Teleport:
                transform.position = currentStep.Destination;
                break;
        }
    }
}