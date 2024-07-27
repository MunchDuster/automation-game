using UnityEngine;

namespace DefaultNamespace
{
    public abstract class Device : MonoBehaviour
    {
        public Vector3Int position;
        public virtual Vector3Int[] Inputs => new[] {GetLocalDirection(Vector3.back)};

        protected virtual void Awake()
        {
            Vector3 worldPos = transform.position;
            position = FromV3(worldPos);
        }

        protected Vector3Int GetLocalDirection(Vector3 direction)
        {
            Vector3 localDirection = transform.rotation * direction;
            return FromV3(localDirection);
        }

        private Vector3Int FromV3(Vector3 pos) => new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }
}