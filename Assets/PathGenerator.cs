using DefaultNamespace;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public int length = 20;
    public Extractor extractorPrefab;
    public Storage storagePrefab;
    public ConveyorBelt[] conveyorPrefabs;

    private void Start()
    {
        GeneratePath();
    }

    private void GeneratePath()
    {
        ChunkManager manager = ChunkManager.Instance;
        
        Vector3 curPosition = transform.position;
        Quaternion curRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
        ItemTaker lastItem = Instantiate(extractorPrefab, curPosition, curRotation, transform);
        ItemTaker lastlastItem = lastItem;
        Quaternion lastRotation = curRotation;
        manager.AddDevice(lastItem);
        for (int i = 0; i < length; i++)
        {
            ConveyorBelt conveyorPrefab = conveyorPrefabs[Random.Range(0, conveyorPrefabs.Length)];
            Vector3 nextPosition = curPosition - curRotation * conveyorPrefab.Inputs[0];
            
            if (!manager.CheckOccupied(Device.FromV3(nextPosition)))
            {
                curPosition -= curRotation * conveyorPrefab.Inputs[0];
                ConveyorBelt conveyorInstance = Instantiate(conveyorPrefab, curPosition, curRotation, transform);
                lastRotation = curRotation;
                curRotation = conveyorPrefab.end.localRotation * curRotation;

                lastItem.receiver = conveyorInstance;
                lastlastItem = lastItem;
                lastItem = conveyorInstance;
                manager.AddDevice(lastItem);
            }
        }
        
        // replace last item with extractor as we are sure that place was not colliding with something
        manager.RemoveDevice(lastItem);
        DestroyImmediate(lastItem.gameObject);
        
        lastRotation *= Quaternion.Euler(0, 180, 0);
        Storage storage = Instantiate(storagePrefab, curPosition, lastRotation, transform);
        lastlastItem.receiver = storage;
        manager.AddDevice(storage);
    }

    private Vector3Int RandomDirection() => new(Random.Range(-1, 2), Random.Range(-1, 2), Random.Range(-1, 2));
}
