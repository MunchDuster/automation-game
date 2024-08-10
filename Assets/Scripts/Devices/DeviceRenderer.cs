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
        public Material[] materials;
        public Mesh mesh;
    }

    public DeviceRenderSettings[] devices;

    void Update()
    {
        foreach (var type in devices)   
        {
            if (!Device.Instances.ContainsKey(type.type))
            {
                Debug.Log("None of " + type.type + " found");
                continue;
            }
            
            List<Matrix4x4> matrices = new();
            foreach (Device device in Device.Instances[type.type])
            {
                matrices.Add(Matrix4x4.TRS(device.Position, device.Rotation, Vector3.one));
            }

            for(int i = 0; i < type.materials.Length; i++)
            {
                Graphics.DrawMeshInstanced(type.mesh, i, type.materials[i], matrices);
            }
        }
    }
}