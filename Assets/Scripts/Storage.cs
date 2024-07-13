using UnityEngine;

public class Storage : ItemTaker
{
    public override void Take(Transform item, float startingDistance) => Destroy(item.gameObject);
    public override bool CanTake(Transform item) => true;
}
