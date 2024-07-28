using System.Collections.Generic;
using UnityEngine;

public abstract class Device : MonoBehaviour
{
    public static Dictionary<int, List<Device>> Instances = new();

    protected virtual int typeIndex => -1;
    
    [HideInInspector] public Vector3Int position;
    [HideInInspector] public Quaternion rotation;
    
    public virtual Vector3Int[] Inputs => new[] {GetLocalDirection(Vector3.back)};

    protected virtual void Awake()
    {
        Vector3 worldPos = transform.position;
        position = FromV3(worldPos);

        if (!Instances.TryGetValue(typeIndex, out List<Device> list))
        {
            list = new();
            Instances.Add(typeIndex, list);
        }
        Instances[typeIndex].Add(this);
    }

    protected Vector3Int GetLocalDirection(Vector3 direction)
    {
        Vector3 localDirection = transform.rotation * direction;
        return FromV3(localDirection);
    }

    public static Vector3Int FromV3(Vector3 pos) => new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
}