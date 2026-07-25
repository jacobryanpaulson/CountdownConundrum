using System.Buffers.Text;
using System.Collections;
using System.Numerics;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;
using UnityEngine.UI;


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
   public int currentPuzzleIndex = 0;
   public GameObject menuCanvas;

   public float levelTransitionDelay = 2.0f;
   public UnityEngine.UI.Image fadeImage;
   public float fadeDuration = 1f;

  


   
  

    public void PlayButton()
    {
       
        ChangeState(GameState.Playing);
        
    }
    public void QuitButton()
    {
        Application.Quit();

         #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnPause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
             if (CurrentState == GameState.Playing)
            {
                ChangeState(GameState.Pause);
            }
            else if (CurrentState == GameState.Pause)
            {
                ChangeState(GameState.Playing);
            }
        }
    }
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
            return;

        }
        ChangeState(GameState.MainMenu);
        
    }
    private void Start()
    {
        
        ResetToFirstPuzzle();

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
       
    }
    private void Update()
    {
        OnPause();
    }
    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;
         if (newState == GameState.Pause || newState == GameState.Playing && CurrentState == GameState.Pause)
    {
        CurrentState = newState;
        HandleStateChange();
    }
    else
    {
        StartCoroutine(TransitionToState(newState));
        }
    }
    private IEnumerator TransitionToState(GameState newState)
    {
        yield return new WaitForSecondsRealtime(.5f);
        CurrentState = newState;
        HandleStateChange();
    }

    private void HandleStateChange()
    {
        switch (CurrentState)
        {
             case GameState.MainMenu:
             Time.timeScale = 0;
              if(menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
                break;
             case GameState.Playing:
             Time.timeScale = 1;
              if(menuCanvas != null)
        { 
            menuCanvas.SetActive(false);
        }
                break;
             case GameState.Pause:
             Time.timeScale = 0;
              
              if(menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
                break;
             case GameState.GameOver:
             Time.timeScale = 0;
                break;
        }
    }
    public void ResetToFirstPuzzle()
{
   
    currentPuzzleIndex = 0;

     if (levelGoals != null)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (levelGoals[i] != null)
            {
                levelGoals[i].ResetGoal();
            }
        }
    }

    
    if (player != null && player.TryGetComponent<PlayerController>(out PlayerController pc))
    {
        
        if (spawnPoints != null && spawnPoints.Length > 0 && spawnPoints[0] != null)
        {
            player.transform.position = spawnPoints[0].position;
            
            if (LoopManager.Instance != null)
            {
                LoopManager.Instance.UpdateSpawnPoint(spawnPoints[0]);
            }
        }

      
        if (levelGoals != null && levelGoals.Length > 0)
        {
            pc.UpdatePuzzleReferences(levelGoals[0]);
        }
        if (puzzleMoveLimits != null && puzzleMoveLimits.Length > 0)
        {
            pc.ResetMoves(puzzleMoveLimits[0]);
        }
    }

    
    for (int i = 0; i < puzzleCam.Length; i++)
    {
        if (puzzleCam[i] != null)
        {
            puzzleCam[i].gameObject.SetActive(i == 0);
        }
    }
}
    
    public void StartDelayedAdvance()
    {
        StartCoroutine(AdvanceToNextPuzzle());
    }

    private IEnumerator AdvanceToNextPuzzle()
    {
        yield return new WaitForSeconds(.5f);
           if (fadeImage != null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    
        

    
       

        int nextIndex = currentPuzzleIndex + 1;
        if(nextIndex >= puzzleCam.Length)
        {
            ChangeState(GameState.MainMenu);
            ResetToFirstPuzzle();
        }

        if(nextIndex < puzzleCam.Length && nextIndex < spawnPoints.Length)
        {
              if (levelGoals != null && levelGoals[currentPuzzleIndex] != null)
        {
            levelGoals[currentPuzzleIndex].ResetGoal();
        }

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
        yield return new WaitForSeconds(levelTransitionDelay);
        
         if (fadeImage != null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    }
  
}
