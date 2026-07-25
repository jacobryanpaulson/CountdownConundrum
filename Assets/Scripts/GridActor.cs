using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridActor : MonoBehaviour
{
    private static readonly HashSet<GridActor> activeActors =
        new HashSet<GridActor>();

    private void OnEnable()
    {
        activeActors.Add(this);
    }

    private void OnDisable()
    {
        activeActors.Remove(this);
    }

    public static bool IsActorAtCell(
        Tilemap tilemap,
        Vector3Int cell
    )
    {
        activeActors.RemoveWhere(actor => actor == null);

        foreach (GridActor actor in activeActors)
        {
            Vector3Int actorCell =
                tilemap.WorldToCell(actor.transform.position);

            if (actorCell == cell)
            {
                return true;
            }
        }

        return false;
    }
}