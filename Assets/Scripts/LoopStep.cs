using UnityEngine;

public enum LoopStepType
{
    Move,
    Teleport
}

public struct LoopStep
{
    public LoopStepType StepType;
    public Vector2 Direction;
    public Vector3 Destination;

    public static LoopStep CreateMove(Vector2 direction)
    {
        return new LoopStep
        {
            StepType = LoopStepType.Move,
            Direction = direction
        };
    }

    public static LoopStep CreateTeleport(Vector3 destination)
    {
        return new LoopStep
        {
            StepType = LoopStepType.Teleport,
            Destination = destination
        };
    }
}