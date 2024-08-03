using DefaultNamespace;
using UnityEngine;

public class ConveyorBeltTurnLeft : ConveyorBeltTurn
{
    public const int TypeIndex = 13;
    protected override int typeIndex => TypeIndex;

    public ConveyorBeltTurnLeft(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
    {
    }
}
