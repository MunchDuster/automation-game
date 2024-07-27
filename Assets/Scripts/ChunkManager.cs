using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class ChunkManager  : MonoBehaviour
    {
        public Transform deviceContainer;

        private readonly Dictionary<Vector3Int, Chunk> _chunks = new();

        private void Start()
        {
            InitialiseExistingDevices();
        }

        private void InitialiseExistingDevices()
        {
            Device[] devices = deviceContainer.GetComponentsInChildren<Device>();

            // Add each device existing to database
            foreach (Device device in devices)
            {
                AddDevice(device);
            }

            ConnectDevicesTogether(devices);
        }

        private void ConnectDevicesTogether(Device[] devices)
        {
            Debug.Log("Making connections between devices...");
            foreach (Device device in devices)
            {
                if (!(device is ItemTaker taker))
                {
                    continue;
                }
                
                foreach (Vector3Int inputOffset in taker.Inputs)
                {
                    Vector3Int inputPos = taker.position + inputOffset;
                    Vector3Int inputChunkPos = Chunk.GetChunkPosition(inputPos);
                    Vector3Int inputChunkLocalPos = Chunk.GetChunkLocalPosition(inputPos);
                    if (_chunks.TryGetValue(inputChunkPos, out Chunk chunk) &&
                        chunk.TryGetItem(inputChunkLocalPos, out Device inputDevice) &&
                        inputDevice is ItemTaker inputGiver)
                    {
                        inputGiver.receiver = taker;
                        Debug.Log($"Successful connection from {inputGiver.gameObject.name} to {device.gameObject.name}");
                    }
                    else
                    {
                        Debug.Log($"Couldn't find giver for {device.gameObject.name}");
                    }
                }
            }
            Debug.Log("Connections complete.");
        }

        public void AddDevice(Device device)
        {
            Vector3Int chunkPos = Chunk.GetChunkPosition(device.position);
            Chunk chunk;
            if (!_chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk = new Chunk(chunkPos);
                _chunks.Add(chunkPos, chunk);
            }

            Vector3Int localPos = Chunk.GetChunkLocalPosition(device.position);
            if (chunk.TryGetItem(localPos, out Device device2))
            {
                Debug.LogError($"Cant add device {device.gameObject.name} to local pos {localPos} as place is already taken by {device2.gameObject.name}");
            }
            chunk.AddItem(device);
        }
    }
}