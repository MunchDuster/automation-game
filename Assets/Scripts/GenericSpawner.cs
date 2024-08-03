using UnityEngine;

public class GenericSpawner : MonoBehaviour
{
    public int min;
    public int max;
    public int count;
    public GameObject prefab;
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 position = new(
                Random.Range(min, max + 1),
                0,
                Random.Range(min, max + 1)
            );
            Instantiate(prefab, position, Quaternion.identity, transform);
        }
    }
}
