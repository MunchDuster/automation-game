using UnityEngine;

/// <summary>
/// 'Stores' (destroys) items it is given.
/// THIS DOES NOT GIVE ANY ITEMS (yet)
/// </summary>
public class Storage : ItemTaker
{
    public override Vector3Int[] Inputs => new[] { GetLocalDirection(Vector3.forward) };

    public override void Take(Item item, float startingDistance) => item.Destroy();
    public override bool CanTake(Item item) => true;
}
