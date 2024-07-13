using System;
using UnityEngine;

public class Extractor : ItemTaker
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float extractionSpeed = 2;
    
    private float _timeSinceLastExtraction;
    private Transform _item;
    private int _itemNum;
    
    public override void Take(Transform item, float startingDistance)
    {
        throw new Exception("Extractor is being given item!");
    }

    public override bool CanTake(Transform item)
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
                return;
            }

        }
        
        _timeSinceLastExtraction += Time.fixedDeltaTime;
        if (_timeSinceLastExtraction < 1f / extractionSpeed)
        {
           return;
        }

        _timeSinceLastExtraction = 0;
        _item = Instantiate(itemPrefab, transform).transform;
        _item.gameObject.name += $" {_itemNum}";
        _itemNum++;
    }
}
