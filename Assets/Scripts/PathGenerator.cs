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

    private int maxTries = 10;
    
    private void GeneratePath(int tryNo = 1)
    {
        ChunkManager manager = ChunkManager.Instance;
        
        Vector3 curPosition = GetNearestUnoccupiedPosition(transform.position);
        Quaternion curRotation = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
        Extractor extractor = Instantiate(extractorPrefab, curPosition, curRotation, transform);
        ItemTaker lastItem = extractor;
        ItemTaker lastlastItem = lastItem;
        Quaternion lastRotation = curRotation;
        manager.AddDevice(lastItem);
        
        int conveyorsPlaced = 0;
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
                conveyorsPlaced++;
            }
        }
        
        // replace last item with extractor as we are sure that place was not colliding with something
        manager.RemoveDevice(lastItem);
        DestroyImmediate(lastItem.gameObject);

        if (conveyorsPlaced <= 1) // if one conveyor then extractor and storage will be directly face to face 
        {
            Debug.LogWarning("Did not place any conveyors, this path has failed.");
            if (conveyorsPlaced == 1)
            {
                manager.RemoveDevice(extractor);
                DestroyImmediate(extractor.gameObject);

                if (tryNo >= maxTries)
                {
                    return;
                }
                GeneratePath(tryNo++);
            }
            return;
        }
        lastRotation *= Quaternion.Euler(0, 180, 0);
        Storage storage = Instantiate(storagePrefab, curPosition, lastRotation, transform);
        lastlastItem.receiver = storage;
        manager.AddDevice(storage);
    }

    private Vector3 GetNearestUnoccupiedPosition(Vector3 targetPos)
    {
        while (ChunkManager.Instance.CheckOccupied(Device.FromV3(targetPos)))
        {
            targetPos += RandomDirection();
        }

        return targetPos;
    }

    private Vector3Int RandomDirection() => new(Random.Range(-1, 2), 0, Random.Range(-1, 2));
}
