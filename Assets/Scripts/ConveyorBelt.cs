using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes items from start to end,
/// If fails to Give() then items start to bunch up (block)
/// will not Take() if no space on conveyor
/// Assume straight line for now
///
/// </summary>
public abstract class ConveyorBelt : ItemTaker
{
    [SerializeField] protected Transform start;
    [SerializeField] protected Transform end;
    [SerializeField] private float length = 4;
    [SerializeField] private float speed = 2;
    [SerializeField] private float itemSize = 0.4f;
    [SerializeField] private bool blockTake = false;
    
    // DEBUG
    [SerializeField] [Range(0f, 1f)] private float lerp = 0;
    
    private class ConveyorItem
    {
        public Transform Item;
        public float Distance;

        public ConveyorItem(Transform item, float distance)
        {
            Item = item;
            Distance = distance;
        }
    }
    
    private List<ConveyorItem> _items = new();
    private float _freeLength; // Length that is not blocked

    private void Awake()
    {
        _freeLength = length;
    }

    public override void Take(Transform item, float startingDistance)
    {
        ConveyorItem conveyorItem = new(item, startingDistance);
        _items.Add(conveyorItem);
        item.SetParent(transform);
        UpdateItemTransform(conveyorItem);
    }

    public override bool CanTake(Transform item) => _freeLength > 0.01f || blockTake;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_items.Count == 0)
        {
            return;
        }
        float deltaDistance = Time.fixedDeltaTime * speed;
        
        // Check if furthest item (will be) at end
        _items[0].Distance += deltaDistance; // Check ahead
        bool lastItemIsAtEnd = _items[0].Distance >= length;
        if (lastItemIsAtEnd)
        {
            if (CanGive(0))
            {
                Debug.Log("Giving item");
                Give(0);
                // Now furthest item is not blocked (second furthest before = furthest now)
            }
        }
        else
        {
            _items[0].Distance -= deltaDistance; // Undo checking ahead
        }


        for (int i = 0; i < _items.Count; i++)
        {
            ConveyorItem item = _items[i];

            SetItemColor(item.Item, Color.red);

            item.Distance += deltaDistance;

            if (item.Distance >= _freeLength) // Remove if at/past end
            {
                Debug.Log($"Blocking {item.Item.gameObject.name} {_freeLength}");
                _freeLength = Mathf.Max(_freeLength - itemSize, 0);
            }

            UpdateItemTransform(item); // s l i d e
        }
    }

    private void UpdateItemTransform(ConveyorItem item)
    {
        item.Item.position = CalculatePosition(item.Distance / length);
        item.Item.rotation = CalculateRotation(item.Distance / length);
    }

    protected abstract Vector3 CalculatePosition(float lerp);
    protected abstract Quaternion CalculateRotation(float lerp);
    

    private void SetItemColor(Transform item, Color color)
    {
        item.GetChild(0).GetComponent<MeshRenderer>().material.color = color;
    }

    private bool CanGive(int index) => CanGive(_items[index].Item);
    private void Give(int index)
    {
        base.Give(_items[index].Item, _items[index].Distance - length);
        _items.RemoveAt(index);
        _freeLength = length;
    }

    private void OnDrawGizmosSelected()
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
        
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(CalculatePosition(lerp), 0.1f);
    }
}
