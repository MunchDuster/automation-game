using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Device
{
    public static Dictionary<int, List<Device>> Instances = new();

    protected virtual int typeIndex => -1;
    
    [HideInInspector] public Vector3Int Position;
    [HideInInspector] public Quaternion Rotation;
    
    public virtual Vector3Int[] Inputs => new[] {GetLocalDirection(Vector3.back)};

    public Device(Vector3Int position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;  
        
        if (!Instances.TryGetValue(typeIndex, out List<Device> list))
        {
            list = new();
            Instances.Add(typeIndex, list);
        }
        Instances[typeIndex].Add(this);
    }

    public void Destroy()
    {
        Instances[typeIndex].Remove(this);
    }

    protected Vector3Int GetLocalDirection(Vector3 direction)
    {
        Vector3 localDirection = Rotation * direction;
        return FromV3(localDirection);
    }

    public static Vector3Int FromV3(Vector3 pos) => new(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    
    public static implicit operator bool(Device item) => item != null; // For inner peace
}