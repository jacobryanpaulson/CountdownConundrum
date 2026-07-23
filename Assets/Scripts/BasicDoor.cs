using UnityEngine;
using UnityEngine.Tilemaps;

public class BasicDoor : MonoBehaviour
{
    [Header("Door Setup")]
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private bool startsOpen;

    private Vector3Int doorCell;
    private TileBase closedDoorTile;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        doorCell = collisionTilemap.WorldToCell(transform.position);
        closedDoorTile = collisionTilemap.GetTile(doorCell);

        if (closedDoorTile == null)
        {
            Debug.LogError(
                $"{gameObject.name} could not find a door tile " +
                $"on the Collision Tilemap at cell {doorCell}."
            );

            return;
        }

        SetOpen(startsOpen);
    }

    public void SetOpen(bool shouldOpen)
    {
        if (closedDoorTile == null)
        {
            return;
        }

        IsOpen = shouldOpen;

        if (IsOpen)
        {
            collisionTilemap.SetTile(doorCell, null);
        }
        else
        {
            collisionTilemap.SetTile(doorCell, closedDoorTile);
        }
    }
}