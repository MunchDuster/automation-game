using UnityEngine;

public abstract class ItemTaker : MonoBehaviour
{
    public ItemTaker receiver;

    public abstract void Take(Transform item, float startingDistance);
    public abstract bool CanTake(Transform item);
    
    public void SetupGive(ItemTaker taker)
    {
        receiver = taker;
    }
    
    protected bool CanGive(Transform item) => receiver && receiver.CanTake(item);

    protected void Give(Transform item, float startingDistance)
    {
        receiver.Take(item, startingDistance);
    }
}
