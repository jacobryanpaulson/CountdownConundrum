using System;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance {get; private set;}
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject playerObject;
    [SerializeField]private int maxGhostsAllowed = 5;

    private List<List<Vector3>> lastMovePath = new List<List<Vector3>>();

    private List<Vector3> currentMovePath = new List<Vector3>();

    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    public void RecordPosition(Vector3 newPosition)
    {
        currentMovePath.Add(newPosition);
    }
    public void ResetLoop()
    {
        lastMovePath.Add(new List<Vector3>(currentMovePath));
        currentMovePath.Clear();

        playerObject.transform.position = spawnPoint.position; 

        playerObject.GetComponent<PlayerController>().ResetMoves();

        if(lastMovePath.Count > maxGhostsAllowed)
        {
           lastMovePath.RemoveAt(0);
        }
        ClearActiveGhosts();

        SpawnGhost();
    }
    public void SpawnGhost()
    {
        /*foreach(List<Vector3> savedPath in lastMovePath)
        {
        GameObject ghost = Instantiate(ghostPrefab, spawnPoint.position, Quaternion.identity);

        GhostPlayback playback = ghost.GetComponent<GhostPlayback>();
        playback.SetPath(savedPath);
        }*/

        for (int i = 0; i < lastMovePath.Count; i++)
        {
            List<Vector3> savedPath = lastMovePath[i];
            GameObject ghost = Instantiate(ghostPrefab, spawnPoint.position, Quaternion.identity);

             GhostPlayback playback = ghost.GetComponent<GhostPlayback>();
            playback.SetPath(savedPath);

            if (ghost.TryGetComponent<ColorChange>(out ColorChange colorChange))
            {
                colorChange.ColorSet(i);
            }
        }
        
    }

    private void ClearActiveGhosts()
    {
       
        GhostPlayback[] activeGhosts = FindObjectsByType<GhostPlayback>(FindObjectsSortMode.None);
        foreach (GhostPlayback ghost in activeGhosts)
        {
            Destroy(ghost.gameObject); 
        }
    }

    public void FullResetLevel()
{
    Debug.Log("Full Level Reset! Wiping out all ghost data.");
    
    ClearActiveGhosts();

     PlayerController.ClearMovementEvents();
   
    lastMovePath.Clear();
    currentMovePath.Clear();

    
    

    
    playerObject.transform.position = spawnPoint.position;
    playerObject.GetComponent<PlayerController>().ResetMoves();
}

    
}
