using System.Buffers.Text;
using System.Collections;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;

public enum GameState
{
    MainMenu,
    Playing,
    Pause,
    GameOver,

}


public class GameManager : MonoBehaviour
{
    public GameState CurrentState { get; private set; }
    
   public static GameManager Instance {get; private set;}

   [Header("Camera References")]
   public Camera[] puzzleCam;
   public Transform[] spawnPoints;
   public LevelGoal[] levelGoals;
   public int[] puzzleMoveLimits;
   public GameObject player;
   private int currentPuzzleIndex = 0;

   
  


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

        }
    }
    private void Start()
    {
        if(player != null && player.TryGetComponent<PlayerController>(out PlayerController pc))
        {
              if (spawnPoints != null && spawnPoints.Length > 0 && spawnPoints[0] != null)
        {
            player.transform.position = spawnPoints[0].position;
            
            if (LoopManager.Instance != null)
            {
                LoopManager.Instance.UpdateSpawnPoint(spawnPoints[0]);
            }
        }
         if (levelGoals.Length > 0)
            {
                pc.UpdatePuzzleReferences(levelGoals[0]);
            }
            if(puzzleMoveLimits != null && puzzleMoveLimits.Length > 0)
            {
                pc.ResetMoves(puzzleMoveLimits[0]);
            }
        }
        ChangeState(GameState.MainMenu);
    }
    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;
        StartCoroutine(TransitionToState(newState));
    }
    private IEnumerator TransitionToState(GameState newState)
    {
        yield return new WaitForSeconds(1f);
        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        switch (CurrentState)
        {
             case GameState.MainMenu:
             Time.timeScale = 0;
                break;
             case GameState.Playing:
             Time.timeScale = 1;
                break;
             case GameState.Pause:
             Time.timeScale = 0;
                break;
             case GameState.GameOver:
             Time.timeScale = 0;
                break;
        }
    }

    public void AdvanceToNextPuzzle()
    {
        int nextIndex = currentPuzzleIndex + 1;

        if(nextIndex < puzzleCam.Length && nextIndex < spawnPoints.Length)
        {
          Camera currentCam = puzzleCam[currentPuzzleIndex];
          Camera nextCam = puzzleCam[currentPuzzleIndex + 1];
          Transform nextSpawn = spawnPoints[currentPuzzleIndex + 1];

            if (LoopManager.Instance != null && nextSpawn != null)
        {
            LoopManager.Instance.UpdateSpawnPoint(nextSpawn);
        }


          

          if(player != null )
            {
                player.transform.position = nextSpawn.position;
               
                if(player.TryGetComponent<PlayerController>(out PlayerController pc))
                {
                    pc.UpdatePuzzleReferences(levelGoals[nextIndex]);

                    int nextMoveLimit = puzzleMoveLimits[nextIndex];
                    pc.ResetMoves(nextMoveLimit);
                }
                
            }
          
          if(currentCam != null && nextCam != null)
            {
                nextCam.gameObject.SetActive(true);
                currentCam.gameObject.SetActive(false);
            }
            
            currentPuzzleIndex++;

        }
        else
        {
            Debug.Log("All puzzles completed!");
        }
    }
  
}
