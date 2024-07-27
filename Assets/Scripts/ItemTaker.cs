using DefaultNamespace;

/// <summary>
/// Main class for exchange of items.
/// TODO: Merge with device?
/// </summary>
public abstract class ItemTaker : Device
{
    public ItemTaker receiver;

    public abstract void Take(Item item, float startingDistance);
    public abstract bool CanTake(Item item);
    
    /// <summary>
    /// Check whether handing an item over to receiver would succeed.
    /// </summary>
    protected bool CanGive(Item item) => receiver && receiver.CanTake(item);

    /// <summary>
    /// Hand over item to receiver.
    /// Check first with CanGive()!
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startingDistance"></param>
    protected void Give(Item item, float startingDistance)
    {
        receiver.Take(item, startingDistance);
    }
}
