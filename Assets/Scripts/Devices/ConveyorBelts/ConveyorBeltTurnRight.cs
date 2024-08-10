using DefaultNamespace;
using UnityEngine;

public class ConveyorBeltTurnRight : ConveyorBeltTurn
{
    public const int TypeIndex = 12;
    protected override int typeIndex => TypeIndex;

    public ConveyorBeltTurnRight(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
    {
    }
}
