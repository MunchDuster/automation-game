using UnityEngine;

namespace DefaultNamespace
{
    public class ConveyorBeltStraight : ConveyorBelt
    {
        public const int TypeIndex = 11;
        protected override int typeIndex => TypeIndex;

        public ConveyorBeltStraight(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
        {
        }

        protected override Vector3 CalculatePosition(float lerp)
        {
            return Vector3.Lerp(start.position, end.position, lerp);
        }

        protected override Quaternion CalculateRotation(float lerp)
        {
            return Rotation;
        }
    }
}