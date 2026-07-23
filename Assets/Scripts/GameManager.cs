using System.Buffers.Text;
using System.Collections;
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
   public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);

        }
    }
    private void Start()
    {
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
}
