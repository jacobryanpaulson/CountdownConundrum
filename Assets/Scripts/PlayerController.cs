using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
   [SerializeField] private Tilemap groundTilemap;
   [SerializeField] private Tilemap collisionTilemap;
   [SerializeField] private int maxMoves = 10;
   [SerializeField] private LevelGoal levelGoal;
   [SerializeField] private TextMeshProUGUI stepText;
   public static Action OnPlayerMoved;

  


    private GridInputMovement controls;
    private int currentMovesRemaining;
    private bool canMove = true;
    private float _pushPower = 2f;

    void Awake()
    {
        controls = new GridInputMovement();
        currentMovesRemaining = maxMoves;
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
      controls.Disable();  
    }
    
    void Start()
    {
       // Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
        controls.Movement.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
       
        if(stepText != null)
        {
            stepText.text = ("Steps Remaining:" + currentMovesRemaining);
        }


    }
    void Update()
    {
         
    
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Manual Reset Triggered!");
            LoopManager.Instance.FullResetLevel();
        }
    
    }

    private void Move(Vector2 direction)
    {
        
        if (!canMove || currentMovesRemaining <= 0)
        {
            return;
        }

       

        if(CanMove(direction))
        {
            currentMovesRemaining--;
        transform.position += (Vector3)direction;
        LoopManager.Instance.RecordPosition(transform.position);
        OnPlayerMoved?.Invoke();
        
        if(stepText != null)
        {
            stepText.text = ("Steps Remaining:" + currentMovesRemaining);
        }

        if (
            levelGoal != null &&
            levelGoal.CheckForCompletion(transform.position)
        )
            {
                //canMove = false;
                return;
            }

         if(currentMovesRemaining <= 0)
        {
            LoopManager.Instance.ResetLoop();
            
           
        }
        }

    }


    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
        if(!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
        return false;

        return true;

    }
     public void TeleportTo(Vector3 destinationPosition)
    {
        if (currentMovesRemaining <= 0) return;

        // Note: Teleportation usually costs 1 move. If you want it to be free, delete the next line:
        currentMovesRemaining--; 

        transform.position = destinationPosition;
        
        // Let the LoopManager know where the player warped to
        LoopManager.Instance.RecordPosition(transform.position);
        OnPlayerMoved?.Invoke();

         CheckMoveLimit();

        
    }
     private void CheckMoveLimit()
    {
        if (currentMovesRemaining <= 0)
        {
            LoopManager.Instance.ResetLoop();
        }
    }
   public void ResetMoves(int newMaxMoves = -1)
{
   
    if (newMaxMoves > 0)
    {
        maxMoves = newMaxMoves;
    }

    currentMovesRemaining = maxMoves;
    if(stepText != null)
        {
            stepText.text = ("Steps Remaining:" + currentMovesRemaining);
        }
    canMove = true; 
}

    public static void ClearMovementEvents()
    {
        OnPlayerMoved = null;
    }
    public void UpdatePuzzleReferences(LevelGoal newGoal)
{
    
    levelGoal = newGoal;
    
    Debug.Log("Player successfully synced to the new puzzle's grid and goal!");
}


}
