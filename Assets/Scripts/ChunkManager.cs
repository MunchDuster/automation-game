using System;
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
        public static ChunkManager Instance;
        
        public Transform deviceContainer;

        private readonly Dictionary<Vector3Int, Chunk> _chunks = new();
        [SerializeField] private bool initializeOnStart = true;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            if (initializeOnStart)
            {
                InitialiseExistingDevices(deviceContainer, true);
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        /// <summary>
        /// Adds all Devices in container to chunks and can connect them together
        /// </summary>
        public void InitialiseExistingDevices(Transform container, bool autoConnect)
        {
            Device[] devices = container.GetComponentsInChildren<Device>();

            // Add each device existing to database
            foreach (Device device in devices)
            {
                AddDevice(device);
            }

            if (autoConnect)
            {
                ConnectDevicesTogether(devices);
            }
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
                    Vector3Int inputPos = taker.Position + inputOffset;
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
                        Debug.Log($"Couldn't find connection for {device}:  at {device.Position}, local pos: {Chunk.GetChunkLocalPosition(device.Position)}.");
                        Debug.Log($"Looking for input at {inputPos}, local pos: {inputChunkLocalPos}, chunk pos: {inputChunkPos}");
                        continue;
                    }

                    inputGiver.SetReceiver(taker);
                }
            }
            Debug.Log("Connections complete.");
        }

        /// <summary>
        /// Puts a device in the appropriate chunk, if no chunk exists then a new one is created.
        /// </summary>
        public void AddDevice(Device device)
        {
            Vector3Int chunkPos = Chunk.GetChunkPosition(device.Position);
            if (!_chunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                chunk = new Chunk(chunkPos);
                _chunks.Add(chunkPos, chunk);
            }

            Vector3Int localPos = Chunk.GetChunkLocalPosition(device.Position);
            if (chunk.TryGetItem(localPos, out Device device2))
            {
                Debug.LogError($"Cant add device {device} to local pos {localPos} as place is already taken by {device2}");
                return;
            }
            chunk.AddItem(device);
        }

        public void RemoveDevice(Device device)
        {
            Vector3Int chunkPos = Chunk.GetChunkPosition(device.Position);
            if (!_chunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                chunk = new Chunk(chunkPos);
                _chunks.Add(chunkPos, chunk);
            }

            chunk.RemoveItem(device);
        }

        public bool CheckOccupied(Vector3Int position)
        {
            Vector3Int chunkPos = Chunk.GetChunkPosition(position);
            Vector3Int localPos = Chunk.GetChunkLocalPosition(position);
            return _chunks.TryGetValue(chunkPos, out Chunk chunk) && chunk.CheckOccupied(localPos);
        }
    }
}