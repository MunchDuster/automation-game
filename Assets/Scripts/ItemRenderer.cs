using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Renders all the currently existing items.
/// Needed as items are just classes, not gameobjects with meshrenderers. (gameobjects are too slow!)
/// TODO: Upgrade to *Indirect* instanced rendering.
/// </summary>
public class ItemRenderer : MonoBehaviour
{
    private const float ItemScale = 0.4f;
    
    public Material material;
    public Mesh mesh;


    void Update()
    {
        List<Matrix4x4> matrices = new();
        foreach (Item instance in Item.Instances)
        {
            matrices.Add(Matrix4x4.TRS(instance.Position + Vector3.up*ItemScale/2f, instance.Rotation, Vector3.one * ItemScale));
        }

        Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }
}