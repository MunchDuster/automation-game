using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// Takes items from start to end,
/// If fails to Give() then items start to bunch up (block)
/// will not Take() if no space on conveyor
/// Each track by default is assumed to be 1 unit long unless length overriden. (i.e corners)
/// </summary>
public abstract class ConveyorBelt : ItemTaker
{
    protected virtual float length => 1;
    
    public Point start;
    public Point end;
    private float speed = 2;
    private float itemSize = 0.4f;
    private bool blockTake = false;

    [Serializable]
    public struct Point
    {
        public Vector3 localRotationEuler;
        public Vector3 localPosition;
        public Transform parent;
        public Quaternion localRotation => quaternion.Euler(localRotationEuler);
        public Quaternion rotation => parent != null ? parent.rotation * localRotation : Quaternion.identity;
        public Vector3 position => parent != null ? parent.TransformPoint(localPosition) : Vector3.zero;
    }
    
    public Action OnTriedGive;
    private class ConveyorItem
    {
        public Item Item;
        public bool Blocked;
        public float Distance;

        public ConveyorItem(Item item, float distance)
        {
            Blocked = false;
            Item = item;
            Distance = distance;
        }
    }
    
    private List<ConveyorItem> _items = new();
    private float _freeLength; // Length that is not blocked


    public override void Take(Item item, float startingDistance)
    {
        if (item == null)
        {
            throw new Exception("Conveyor given null item!");
        }
        ConveyorItem conveyorItem = new(item, startingDistance);
        _items.Add(conveyorItem);
        UpdateItemTransform(conveyorItem);
    }

    public override bool CanTake(Item item) => _freeLength > 0.01f || blockTake;

    protected virtual void FixedUpdate()
    {
        if(_items.Count == 0)
        {
            return;
        }
        float deltaDistance = Time.fixedDeltaTime * speed;
        
        // Check if furthest item (will be) at end
        bool IsLastItemAtEnd() => _items.Count > 0 && _items[0].Distance + deltaDistance >= length;
        while(IsLastItemAtEnd())
        {
            if (CanGive(0))
            {
                Give(0);
            }
            OnTriedGive?.Invoke();
        }

        for (int i = 0; i < _items.Count; i++)
        {
            ConveyorItem item = _items[i];

            if (item.Blocked)
            {
                continue;
            }
            
            item.Distance += deltaDistance;

            if (item.Distance >= _freeLength) // Remove if at/past end
            {
                item.Distance = _freeLength;
                _freeLength = Mathf.Max(_freeLength - itemSize, 0);
                item.Blocked = true;
            }

            UpdateItemTransform(item); // s l i d e
        }
    }

    private void UpdateItemTransform(ConveyorItem item)
    {
        item.Item.Position = CalculatePosition(item.Distance / length);
        item.Item.Rotation = CalculateRotation(item.Distance / length);
    }

    protected abstract Vector3 CalculatePosition(float lerp);
    protected abstract Quaternion CalculateRotation(float lerp);
    

    private bool CanGive(int index) => CanGive(_items[index].Item);
    private void Give(int index)
    {
        base.Give(_items[index].Item, _items[index].Distance - length);
        _items.RemoveAt(index);
        _freeLength = length;

        // unblock all items
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].Blocked = false;
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Vector3 lastPoint = CalculatePosition(0);
        const float delta = 1f / 20f;
        for (float i = delta; i <= 1.001f; i += delta)
        {
            Gizmos.color = new Color(1-i,i,0);
            Vector3 pos = CalculatePosition(i);
            Gizmos.DrawLine(pos, lastPoint);
            lastPoint = pos;
        }
        
        Gizmos.color = Color.red;
        DrawArrow(start);
        DrawArrow(end);
    }

    protected void DrawArrow(Point point)
    {
        Gizmos.DrawWireSphere(point.position, 0.03f);
        Gizmos.DrawLine(point.position, point.position +  point.rotation * (Vector3.forward * 0.3f));
    }

    protected ConveyorBelt(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
    {
        _freeLength = length;
    }
}
