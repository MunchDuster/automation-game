using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders all the currently existing items.
/// Needed as items are just classes, not gameobjects with meshrenderers. (gameobjects are too slow!)
/// TODO: Upgrade to *Indirect* instanced rendering.
/// </summary>
public class DeviceRenderer : MonoBehaviour
{
    [Serializable]
    public class DeviceRenderSettings
    {
        public int type;
        public Material material;
        public Mesh mesh;
    }

    public DeviceRenderSettings[] devices;

    void Update()
    {
        foreach (var type in devices)
        {
            List<Matrix4x4> matrices = new();
            foreach (Device device in Device.Instances[type.type])
            {
                matrices.Add(Matrix4x4.TRS(device.position, device.rotation, Vector3.one));
            }

            Graphics.DrawMeshInstanced(type.mesh, 0, type.material, matrices);
        }
    }
}