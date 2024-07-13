using UnityEngine;

namespace DefaultNamespace
{
    public class ConveyorBeltTurn : ConveyorBelt
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private float angleMultiplier = 1f;
        protected override Vector3 CalculatePosition(float lerp)
        {
            float angle = angleMultiplier * lerp * 90f;
            
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            
            Vector3 appliedRotation = rotation * (start.position - pivot.position);
            Vector3 position = pivot.position + appliedRotation;
            return position;
        }

        protected override Quaternion CalculateRotation(float lerp)
        {
            return Quaternion.Slerp(start.rotation, end.rotation, lerp); 
        }
    }
}