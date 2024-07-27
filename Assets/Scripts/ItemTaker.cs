using DefaultNamespace;

public abstract class ItemTaker : Device
{
    public ItemTaker receiver;

    public abstract void Take(Item item, float startingDistance);
    public abstract bool CanTake(Item item);
    
    protected bool CanGive(Item item) => receiver && receiver.CanTake(item);

    protected void Give(Item item, float startingDistance)
    {
        receiver.Take(item, startingDistance);
    }
}
