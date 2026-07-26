using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicDoor : MonoBehaviour
{
    [Header("Door Setup")]
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private bool startsOpen;

    [Header("Occupancy Check")]
    [Range(0.1f, 1f)]
    [SerializeField] private float occupancyCheckSize = 0.8f;

    private Vector3Int doorCell;
    private TileBase closedDoorTile;
    private bool closeRequested;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (collisionTilemap == null)
        {
            Debug.LogError(
                $"{gameObject.name} is missing its Collision Tilemap."
            );

            enabled = false;
            return;
        }

        doorCell =
            collisionTilemap.WorldToCell(transform.position);

        closedDoorTile =
            collisionTilemap.GetTile(doorCell);

        if (closedDoorTile == null)
        {
            Debug.LogError(
                $"{gameObject.name} could not find a door tile " +
                $"on the Collision Tilemap at cell {doorCell}."
            );

            enabled = false;
            return;
        }

        if (startsOpen)
        {
            OpenDoor();
        }
        else
        {
            CloseDoorImmediately();
        }
    }

    private void Update()
    {
        if (closeRequested)
        {
            TryCloseDoor();
        }
    }

    public void SetOpen(bool shouldOpen)
    {
        if (!enabled || closedDoorTile == null)
        {
            return;
        }

        if (shouldOpen)
        {
            closeRequested = false;
            OpenDoor();
        }
        else
        {
            closeRequested = true;
            TryCloseDoor();
        }
    }

    private void TryCloseDoor()
    {
        if (IsDoorwayOccupied())
        {
            OpenDoor();
            return;
        }

        CloseDoorImmediately();
        closeRequested = false;
    }

    private bool IsDoorwayOccupied()
    {
        Vector3 doorCenter =
            collisionTilemap.GetCellCenterWorld(doorCell);

        Vector3 scaledCellSize =
            Vector3.Scale(
                collisionTilemap.cellSize,
                collisionTilemap.transform.lossyScale
            );

        Vector2 checkSize = new Vector2(
            Mathf.Abs(scaledCellSize.x) * occupancyCheckSize,
            Mathf.Abs(scaledCellSize.y) * occupancyCheckSize
        );

        Collider2D[] overlappingColliders =
            Physics2D.OverlapBoxAll(
                doorCenter,
                checkSize,
                0f
            );

        foreach (Collider2D overlappingCollider in overlappingColliders)
        {
            if (
                overlappingCollider.GetComponentInParent<GridActor>() != null ||
                overlappingCollider.GetComponentInParent<GridBox>() != null
            )
            {
                return true;
            }
        }

        return false;
    }

    private void OpenDoor()
    {
        collisionTilemap.SetTile(
            doorCell,
            null
        );

        IsOpen = true;
         
    }

    private void CloseDoorImmediately()
    {
        collisionTilemap.SetTile(
            doorCell,
            closedDoorTile
        );

        IsOpen = false;
        
       
    }

    private void OnDrawGizmosSelected()
    {
        if (collisionTilemap == null)
        {
            return;
        }

        Vector3Int previewDoorCell =
            collisionTilemap.WorldToCell(transform.position);

        Vector3 doorCenter =
            collisionTilemap.GetCellCenterWorld(previewDoorCell);

        Vector3 scaledCellSize =
            Vector3.Scale(
                collisionTilemap.cellSize,
                collisionTilemap.transform.lossyScale
            );

        Vector3 checkSize = new Vector3(
            Mathf.Abs(scaledCellSize.x) * occupancyCheckSize,
            Mathf.Abs(scaledCellSize.y) * occupancyCheckSize,
            0f
        );

        Gizmos.DrawWireCube(
            doorCenter,
            checkSize
        );
    }
}