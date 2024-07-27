using UnityEngine;

public class TimeSpeedMultiplier : MonoBehaviour
{
    [SerializeField, Range(0, 10)] private float speed = 1;
    [SerializeField, Range(5, 144)] private float targetFramerate = 60;
    void Update()
    {
        Time.timeScale = speed;
        Time.fixedDeltaTime = 1f / (targetFramerate / speed);
        // Application.targetFrameRate = (int)(targetFramerate / speed);
    }
}
