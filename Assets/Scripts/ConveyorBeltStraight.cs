using UnityEngine;

namespace DefaultNamespace
{
    public class ConveyorBeltStraight : ConveyorBelt
    {
        protected virtual int typeIndex => 11;

        protected override Vector3 CalculatePosition(float lerp)
        {
            return Vector3.Lerp(start.position, end.position, lerp);
        }

        protected override Quaternion CalculateRotation(float lerp)
        {
            return transform.rotation;
        }
    }
}