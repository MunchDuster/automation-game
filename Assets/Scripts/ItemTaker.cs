using DefaultNamespace;
using UnityEngine;

public abstract class ItemTaker : Device
{
    public ItemTaker receiver;

    public abstract void Take(Transform item, float startingDistance);
    public abstract bool CanTake(Transform item);
    
    protected bool CanGive(Transform item) => receiver && receiver.CanTake(item);

    protected void Give(Transform item, float startingDistance)
    {
        receiver.Take(item, startingDistance);
    }
}
