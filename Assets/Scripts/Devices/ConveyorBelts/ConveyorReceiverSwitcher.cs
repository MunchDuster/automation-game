using UnityEngine;

public class ConveyorReceiverSwitcher : MonoBehaviour
{
    [SerializeField] private ConveyorBelt belt;

    [SerializeField] private ItemTaker[] receivers;

    private int _index;

    private void Awake()
    {
        belt.OnTriedGive += SwitchReceiver;
    }

    private void SwitchReceiver()
    {
        _index++;
        _index %= receivers.Length;
        belt.SetReceiver(receivers[_index]);
    }
}
