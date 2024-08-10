using UnityEngine;

/// <summary>
/// 'Stores' (destroys) items it is given.
/// THIS DOES NOT GIVE ANY ITEMS (yet)
/// </summary>
public class Storage : ItemTaker
{
    protected override int typeIndex => 2;

    public override Vector3Int[] Inputs => new[] { GetLocalDirection(Vector3.forward) };

    public override void Take(Item item, float startingDistance) => item.Destroy();
    public override bool CanTake(Item item) => true;

    public Storage(Vector3Int position, Quaternion rotation, ItemTaker receiver) : base(position, rotation, receiver)
    {
    }
}
