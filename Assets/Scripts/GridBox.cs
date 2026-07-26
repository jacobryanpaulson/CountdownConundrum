using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GridBox : MonoBehaviour
{
    private static readonly HashSet<GridBox> activeBoxes =
        new HashSet<GridBox>();

    [Header("Grid References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    

    private Vector3Int startingCell;
    private Rigidbody2D boxBody;

    private void Awake()
    {
       
        boxBody = GetComponent<Rigidbody2D>();

        if (groundTilemap == null || collisionTilemap == null)
        {
            Debug.LogError(
                $"{gameObject.name} is missing a Tilemap reference."
            );

            enabled = false;
            return;
        }

        startingCell =
            groundTilemap.WorldToCell(transform.position);

        MoveToCell(startingCell);
    }

    private void OnEnable()
    {
        activeBoxes.Add(this);
    }

    private void OnDisable()
    {
        activeBoxes.Remove(this);
    }

    public bool TryPush(Vector2 direction)
    {
         BoxSound boxSound  = GetComponent<BoxSound>();
        Vector3 targetWorldPosition =
            transform.position + (Vector3)direction;

        Vector3Int targetCell =
            groundTilemap.WorldToCell(targetWorldPosition);

        if (!groundTilemap.HasTile(targetCell))
        {
            return false;
        }

        if (collisionTilemap.HasTile(targetCell))
        {
            return false;
        }

        GridBox otherBox =
            GetBoxAtCell(groundTilemap, targetCell);

        if (otherBox != null && otherBox != this)
        {
            return false;
        }

        if (GridActor.IsActorAtCell(groundTilemap, targetCell))
        {
            return false;
        }
        boxSound.BoxPushSound();

        MoveToCell(targetCell);
        return true;
    }

    public void ResetBox()
    {
        MoveToCell(startingCell);
    }

    public static GridBox GetBoxAtCell(
        Tilemap tilemap,
        Vector3Int cell
    )
    {
        activeBoxes.RemoveWhere(box => box == null);

        foreach (GridBox box in activeBoxes)
        {
            if (box.groundTilemap != tilemap)
            {
                continue;
            }

            Vector3Int boxCell =
                tilemap.WorldToCell(box.transform.position);

            if (boxCell == cell)
            {
                return box;
            }
        }

        return null;
    }

    public static void ResetAllBoxes()
    {
        activeBoxes.RemoveWhere(box => box == null);

        foreach (GridBox box in activeBoxes)
        {
            box.ResetBox();
        }
    }

    private void MoveToCell(Vector3Int cell)
    {
        
        Vector3 cellCenter =
            groundTilemap.GetCellCenterWorld(cell);

        boxBody.position = new Vector2(
            cellCenter.x,
            cellCenter.y
        );
        
    }
}