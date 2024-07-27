using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// For grouping devices together based on position to optimize searching.
    /// TODO: Upgrade to oct-trees because that's what all the cool youtubers are doing.
    /// </summary>
    public class Chunk
    {
        public static int ChunkSize = 16;
        private Vector3Int _position;
        private Dictionary<Vector3Int,Device> _devices = new();

        public Chunk(Vector3Int position)
        {
            _position = position;
        }

        public void AddItem(Device device)
        {
            _devices.Add(GetChunkLocalPosition(device.position), device);
        }
        public void RemoveItem(Device device)
        {
            _devices.Remove(GetChunkLocalPosition(device.position));
        }
        
        public List<Device> Items => _devices.Values.ToList();
        
        /// <summary>
        /// Checks if the POSITION is WITHIN the chunk.
        /// (doesn't check if is listed as part of chunk)
        /// </summary>
        public bool IsItemInChunk(Device device) => GetChunkLocalPosition(device.position) == _position;
       
        public bool TryGetItem(Vector3Int localPosition, out Device device) => _devices.TryGetValue(localPosition, out device);

        /// <summary>
        /// Checks if a local position within the chunk is ALREADY OCCUPIED
        /// </summary>
        public bool CheckOccupied(Vector3Int localPosition) => _devices.ContainsKey(localPosition);
        
        /// <summary>
        /// Gets WHICH CHUNK the position is in.
        /// </summary>
        public static Vector3Int GetChunkPosition(Vector3Int worldPosition) => worldPosition / ChunkSize;
        
        /// <summary>
        /// Gets the local position WITHIN a chunk.
        /// </summary>
        public static Vector3Int GetChunkLocalPosition(Vector3Int worldPosition) => new(
            worldPosition.x % ChunkSize,
            worldPosition.y % ChunkSize,
            worldPosition.z % ChunkSize
        );

    }
}