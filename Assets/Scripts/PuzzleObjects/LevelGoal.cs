using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGoal : MonoBehaviour
{
    [Header("Goal Setup")]
    [SerializeField] private Tilemap goalTilemap;
    [SerializeField] private GameObject levelCompleteMessage;
   
    

    public bool IsCompleted { get; private set; }

    private void Awake()
    {
        if (levelCompleteMessage != null)
        {
            levelCompleteMessage.SetActive(false);
        }
    }

    public bool CheckForCompletion(Vector3 playerPosition)
    {
       
        if (IsCompleted)
        {
       
            return false;
        }

        if (goalTilemap == null)
        {
            Debug.LogError(
                "LevelGoal does not have a Goal Tilemap assigned."
            );

            return false;
        }

        Vector3Int playerCell =
            goalTilemap.WorldToCell(playerPosition);

        if (!goalTilemap.HasTile(playerCell))
        {
            return false;
        }

        IsCompleted = true;

          if (levelCompleteMessage != null)
        {
                    if (WinLoseLevelSFX.Instance != null)
        {
            WinLoseLevelSFX.Instance.WinSound();
        }

            levelCompleteMessage.SetActive(true);
        }
        Debug.Log("Level Complete!");

        if (GameManager.Instance != null)
    {
        GameManager.Instance.StartDelayedAdvance();
    }
    else
    {
        Debug.LogWarning("GameManager instance not found in scene!");
    }

     

        

        return true;
    }

    public void ResetGoal()
    {
        IsCompleted = false;

        if (levelCompleteMessage != null)
        {
            levelCompleteMessage.SetActive(false);
        }
    }
}