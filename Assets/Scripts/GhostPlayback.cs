using System.Collections.Generic;
using UnityEngine;

public class GhostPlayback : MonoBehaviour
{
private List<Vector3> savedPath = new List<Vector3>();
private int currentStepIndex = 0;

    public void OnEnable()
    {
        PlayerController.OnPlayerMoved += moveGhost;
    }
    public void OnDisable()
    {
        PlayerController.OnPlayerMoved -= moveGhost;
    }

    public void SetPath(List<Vector3> pathHistory)
    {
        savedPath = pathHistory;
        currentStepIndex = 0;
    }
public void moveGhost()
    {
        if(currentStepIndex < savedPath.Count)
        {
            transform.position = savedPath[currentStepIndex];
            currentStepIndex++;
        }
        else
        {
            Debug.Log("Ghost Has Completed its path");
        }
    }
}
