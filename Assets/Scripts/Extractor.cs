using System;
using UnityEngine;

/// <summary>
/// These are the source of items, the equivalent of drills and miners in other games.
/// THESE DO NOT TAKE IN ITEMS, ONLY GIVE.
/// </summary>
public class Extractor : ItemTaker
{
    [SerializeField] private float extractionSpeed = 2;
    
    private float _timeSinceLastExtraction;
    private Item _item;
    private int _itemNum;
    
    protected virtual int typeIndex => 1;
    
    public override void Take(Item item, float startingDistance)
    {
        throw new Exception("Extractor is being given item!");
    }

    public override bool CanTake(Item item)
    {
        throw new Exception("Extractor is being asked if can take item!");
    }

    private void FixedUpdate()
    {
        if (_item)
        {
            if (CanGive(_item))
            {
                Debug.Log("Extractor cant give! Receiver full!");
                Give(_item, 0);
                _item = null;
            }
            return;
        }
        
        _timeSinceLastExtraction += Time.fixedDeltaTime;
        if (_timeSinceLastExtraction < 1f / extractionSpeed)
        {
           return;
        }

        _timeSinceLastExtraction = 0;
        _item = new Item(transform.position, transform.rotation);
        _itemNum++;
    }
}
