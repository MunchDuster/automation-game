using System;
using System.Collections.Generic;
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
    
    [SerializeField] protected Point start;
    [SerializeField] protected Point end;
    [SerializeField] private float speed = 2;
    [SerializeField] private float itemSize = 0.4f;
    [SerializeField] private bool blockTake = false;

    [Serializable]
    protected struct Point
    {
        public Vector3 localRotationEuler;
        public Vector3 localPosition;
        public Transform parent;
        public Quaternion localRotation => quaternion.Euler(localRotationEuler);
        public Quaternion rotation => parent.rotation * localRotation;
        public Vector3 position => parent.TransformPoint(localPosition);
    }
    // DEBUG
    [SerializeField] [Range(0f, 1f)] private float lerp = 0;
    
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

    protected override void Awake()
    {
        base.Awake();
        _freeLength = length;
    }

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

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_items.Count == 0)
        {
            return;
        }
        float deltaDistance = Time.fixedDeltaTime * speed;
        
        // Check if furthest item (will be) at end
        bool IsLastItemAtEnd() => _items[0].Distance + deltaDistance >= length;
        while(IsLastItemAtEnd())
        {
            if (CanGive(0))
            {
                Debug.Log("Giving item");
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
                Debug.Log($"Blocking, _freeLength: {_freeLength}");
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
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(CalculatePosition(lerp), 0.1f);
    }

    protected void DrawArrow(Point point)
    {
        Gizmos.DrawWireSphere(point.position, 0.03f);
        Gizmos.DrawLine(point.position, point.position +  point.rotation * (Vector3.forward * 0.3f));
    }
}
