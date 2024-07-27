using UnityEngine;

public class Storage : ItemTaker
{
    public override Vector3Int[] Inputs => new[] { GetLocalDirection(Vector3.forward) };

    public override void Take(Transform item, float startingDistance) => Destroy(item.gameObject);
    public override bool CanTake(Transform item) => true;
}
