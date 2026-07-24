using UnityEngine;
using UnityEngine.Tilemaps;

public static class GridMovement
{
    public static Vector2 GetCardinalDirection(Vector2 input)
    {
        float horizontalAmount = Mathf.Abs(input.x);
        float verticalAmount = Mathf.Abs(input.y);

        if (
            horizontalAmount == 0f &&
            verticalAmount == 0f
        )
        {
            return Vector2.zero;
        }

        if (horizontalAmount > verticalAmount)
        {
            return new Vector2(
                Mathf.Sign(input.x),
                0f
            );
        }

        return new Vector2(
            0f,
            Mathf.Sign(input.y)
        );
    }

    public static bool TryMoveActor(
        Transform actor,
        Vector2 direction,
        Tilemap groundTilemap,
        Tilemap collisionTilemap
    )
    {
        direction = GetCardinalDirection(direction);

        if (direction == Vector2.zero)
        {
            return false;
        }

        Vector3 targetWorldPosition =
            actor.position + (Vector3)direction;

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

        GridBox box =
            GridBox.GetBoxAtCell(
                groundTilemap,
                targetCell
            );

        if (box != null)
        {
            bool boxMoved = box.TryPush(direction);

            if (!boxMoved)
            {
                return false;
            }
        }

        actor.position = new Vector3(
            targetWorldPosition.x,
            targetWorldPosition.y,
            actor.position.z
        );

        return true;
    }
}