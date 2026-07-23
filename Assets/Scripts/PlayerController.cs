using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
   [SerializeField] private Tilemap groundTilemap;
   [SerializeField] private Tilemap collisionTilemap;
   [SerializeField] private int maxMoves = 10;
   public static Action OnPlayerMoved;

  


    private GridInputMovement controls;
    private int currentMovesRemaining;

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
        if (currentMovesRemaining <= 0)
        {
            return;
        }

       

        if(CanMove(direction))
        {
            currentMovesRemaining--;
        transform.position += (Vector3)direction;
        LoopManager.Instance.RecordPosition(transform.position);
        OnPlayerMoved?.Invoke();

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
     public void ResetMoves()
    {
        currentMovesRemaining = maxMoves;
    }

    public static void ClearMovementEvents()
    {
        OnPlayerMoved = null;
    }
}
