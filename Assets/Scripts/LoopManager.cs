using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    [Header("Loop References")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private LevelGoal levelGoal;

    [Header("Loop Settings")]
    [SerializeField] private int maxGhostsAllowed = 5;

    private readonly List<List<LoopStep>> lastMovePath =
        new List<List<LoopStep>>();

    private int nextGhostColorIndex;

    private readonly List<LoopStep> currentMovePath =
        new List<LoopStep>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecordMove(Vector2 direction)
    {
        currentMovePath.Add(
            LoopStep.CreateMove(direction)
        );
    }

    public void RecordTeleport(Vector3 destination)
    {
        currentMovePath.Add(
            LoopStep.CreateTeleport(destination)
        );
    }

    public void ResetLoop()
    {
        lastMovePath.Add(
            new List<LoopStep>(currentMovePath)
        );

        currentMovePath.Clear();

        ClearActiveGhosts();
        GridBox.ResetAllBoxes();

        if (playerObject != null && spawnPoint != null)
        {
            playerObject.transform.position =
                spawnPoint.position;
        }

        PlayerController playerController =
            GetPlayerController();

        if (playerController != null)
        {
            playerController.ResetMoves();
        }

        if (lastMovePath.Count > maxGhostsAllowed)
        {
            lastMovePath.RemoveAt(0);
        }

        SpawnGhosts();
    }

    private void SpawnGhosts()
    {
        PlayerController playerController =
            GetPlayerController();

        if (
            ghostPrefab == null ||
            spawnPoint == null ||
            playerController == null
        )
        {
            Debug.LogError(
                "LoopManager is missing the ghost prefab, spawn point, " +
                "or PlayerController reference."
            );

            return;
        }

        for (int i = 0; i < lastMovePath.Count; i++)
        {
            GameObject ghost = Instantiate(
                ghostPrefab,
                spawnPoint.position,
                Quaternion.identity
            );

            GhostPlayback playback =
                ghost.GetComponent<GhostPlayback>();

            if (playback == null)
            {
                Debug.LogError(
                    "The Ghost prefab is missing GhostPlayback."
                );

                Destroy(ghost);
                continue;
            }

            playback.SetPath(
                lastMovePath[i],
                playerController.GroundTilemap,
                playerController.CollisionTilemap
            );

            if (
                ghost.TryGetComponent<ColorChange>(
                    out ColorChange colorChange
                )
            )
            {
                colorChange.ColorSet(i);
            }
        }
    }

    public void UpdateSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;

        lastMovePath.Clear();
        currentMovePath.Clear();

        ClearActiveGhosts();
        GridBox.ResetAllBoxes();
    }

    public void FullResetLevel()
    {
        Debug.Log(
            "Full Level Reset! Wiping out all ghost data."
        );

        ClearActiveGhosts();
        PlayerController.ClearMovementEvents();

        lastMovePath.Clear();
        currentMovePath.Clear();

        GridBox.ResetAllBoxes();

        if (playerObject != null && spawnPoint != null)
        {
            playerObject.transform.position =
                spawnPoint.position;
        }

        PlayerController playerController =
            GetPlayerController();

        if (playerController != null)
        {
            playerController.ResetMoves();
        }

        if (levelGoal != null)
        {
            levelGoal.ResetGoal();
        }
    }

    private PlayerController GetPlayerController()
    {
        if (playerObject == null)
        {
            return null;
        }

        return playerObject.GetComponent<PlayerController>();
    }

    private void ClearActiveGhosts()
    {
        GhostPlayback[] activeGhosts =
            FindObjectsByType<GhostPlayback>(
                FindObjectsSortMode.None
            );

        foreach (GhostPlayback ghost in activeGhosts)
        {
            Destroy(ghost.gameObject);
        }
    }

    public bool CanCreateClone()
    {
        GhostPlayback[] activeGhosts =
            FindObjectsByType<GhostPlayback>(
                FindObjectsSortMode.None
            );
        return activeGhosts.Length < maxGhostsAllowed;
    }

    public bool CreateClone(
        Vector3 cloneStartPosition,
        List<LoopStep> recordedPath
    )
    {
        if (ghostPrefab == null)
        {
            Debug.LogError(
                "LoopManager does not have a Ghost Prefab Assigned"
            );

            return false;
        }

        if (recordedPath == null || recordedPath.Count == 0)
        {
            Debug.LogWarning(
                "A clone cannot be crated without a recored path."
            );

            return false;
        }

        if (!CanCreateClone())
        {
            Debug.LogWarning(
                "The maximum nuber of clones has been created."
            );

            return false;
        }

        PlayerController playerController =
            GetPlayerController();

        if (playerController == null)
        {
            Debug.LogError(
                "LoopManager could not find PLaterController"
            );

            return false;
        }

        GameObject ghost = Instantiate(
            ghostPrefab,
            cloneStartPosition,
            Quaternion.identity
        );

        GhostPlayback playback =
            ghost.GetComponent<GhostPlayback>();

        if (playback == null)
        {
            Debug.LogError(
                "The Ghost prefab is missing GhostPlayback"
            );

            Destroy(ghost);
            return false;
        }

        playback.SetPath(
            recordedPath,
            playerController.GroundTilemap,
            playerController.CollisionTilemap
        );

        if (
            ghost.TryGetComponent<ColorChange>(
                out ColorChange colorChange
            )
        )
        {
            colorChange.ColorSet(nextGhostColorIndex);
        }

        nextGhostColorIndex --;

        return true;
    }
}