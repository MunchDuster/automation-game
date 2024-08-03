using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// Conveyor belt around a corner.
    /// Although this is sufficent to technically work.
    /// Each type of turn should be a separate inheritor to have unique typeIndexes.
    /// </summary>
    public abstract class ConveyorBeltTurn : ConveyorBelt
    {
        
        protected override float length => Vector3.Distance(pivot.position, start.position) * Mathf.PI / 2f; // 90-degree angle is 1/4 of 2*pi*r = r*pi/2
        
        [SerializeField] private Point pivot;
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
        
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            
            Gizmos.color = Color.green;
            DrawArrow(pivot);
        }

        protected ConveyorBeltTurn(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
        {
        }
    }
    
}
