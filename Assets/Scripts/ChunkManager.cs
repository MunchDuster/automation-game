using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// The 'world' manager, handles chunk creation and setting up interfacing of devices.
    /// THIS IS NOT TERRAIN RELATED.
    /// </summary>
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

        /// <summary>
        /// Go through each device and connect them together by where they expect inputs
        /// </summary>
        private void ConnectDevicesTogether(Device[] devices)
        {
            Debug.Log("Making connections between devices...");
            foreach (Device device in devices)
            {
                if (!(device is ItemTaker taker))
                {
                    continue;
                }
                
                if(device is Extractor)
                    continue; // extractors have no inputs
                
                foreach (Vector3Int inputOffset in taker.Inputs)
                {
                    Vector3Int inputPos = taker.position + inputOffset;
                    Vector3Int inputChunkPos = Chunk.GetChunkPosition(inputPos);
                    Vector3Int inputChunkLocalPos = Chunk.GetChunkLocalPosition(inputPos);

                    bool success = true;
                    
                    if(!_chunks.TryGetValue(inputChunkPos, out Chunk chunk))
                    {
                        success = false;
                        Debug.LogError("CHUNK DOESNT EXIST!");
                    }

                    if(!chunk.TryGetItem(inputChunkLocalPos, out Device inputDevice))
                    {
                        success = false;
                        Debug.LogError("POSITION IN CHUNK IS EMPTY!");
                    }

                    ItemTaker inputGiver = (ItemTaker)inputDevice;
                    if (inputGiver == null)
                    {
                        success = false;
                        Debug.LogError("DEVICE AT INPUT PLACE IS NOT ITEM TAKER!");
                    }

                    if (!success)
                    {
                        // Log everything needed to understand why input wasnt found.
                        Debug.Log($"Couldn't find connection for {device.gameObject.name}:  at {device.position}, local pos: {Chunk.GetChunkLocalPosition(device.position)}.");
                        Debug.Log($"Looking for input at {inputPos}, local pos: {inputChunkLocalPos}, chunk pos: {inputChunkPos}");
                        continue;
                    }

                    inputGiver.receiver = taker;
                }
            }
            Debug.Log("Connections complete.");
        }

        /// <summary>
        /// Puts a device in the appropriate chunk, if no chunk exists then a new one is created.
        /// </summary>
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