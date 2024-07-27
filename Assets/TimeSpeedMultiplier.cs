using UnityEngine;

public class TimeSpeedMultiplier : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private float speed = 1;

    void Update()
    {
        Time.timeScale = speed;
    }
}
