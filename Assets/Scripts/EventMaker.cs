using UnityEngine;
using UnityEngine.Events;

public class EventMaker : MonoBehaviour
{
    public UnityEvent theEvent;

    public void RunEvent() => theEvent.Invoke();
}
