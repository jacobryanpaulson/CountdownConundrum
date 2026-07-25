using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance { get; private set; }

    [Header("Clone References")]
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private LevelGoal levelGoal;

    [Header("Clone Settings")]
    [SerializeField] private int maxGhostsAllowed = 5;

    private int nextGhostColorIndex;

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
                "LoopManager does not have a Ghost prefab assigned."
            );

            return false;
        }

        if (recordedPath == null || recordedPath.Count == 0)
        {
            Debug.LogWarning(
                "A clone cannot be created without a recorded path."
            );

            return false;
        }

        if (!CanCreateClone())
        {
            Debug.LogWarning(
                "The maximum number of clones has been reached."
            );

            return false;
        }

        PlayerController playerController =
            GetPlayerController();

        if (playerController == null)
        {
            Debug.LogError(
                "LoopManager could not find PlayerController."
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
                "The Ghost prefab is missing GhostPlayback."
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

        nextGhostColorIndex++;

        return true;
    }

    public void UpdateSpawnPoint(Transform newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
        nextGhostColorIndex = 0;

        ClearActiveGhosts();
        GridBox.ResetAllBoxes();
    }

    public void FullResetLevel()
    {
        Debug.Log(
            "Full Level Reset! Wiping out all clone data."
        );

        ClearActiveGhosts();
        PlayerController.ClearMovementEvents();

        nextGhostColorIndex = 0;

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
}