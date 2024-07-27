using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public static List<Item> Instances = new();
    
    public Vector3 Position;
    public Quaternion Rotation;

    public Item(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
        Instances.Add(this);
    }

    public void Destroy()
    {
        Instances.Remove(this);
    }

    public static implicit operator bool(Item item) => item != null; // For inner peace
}
