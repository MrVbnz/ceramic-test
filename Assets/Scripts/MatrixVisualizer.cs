using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatrixVisualizer
{
    private readonly List<GameObject> prefabs;
    private readonly List<List<GameObject>> spawned;
    private readonly Transform parent;

    public MatrixVisualizer(IEnumerable<GameObject> prefabs)
    {
        this.prefabs = prefabs.ToList();
        spawned = Enumerable.Range(0, this.prefabs.Count).Select(_ => new List<GameObject>()).ToList();
        parent = new GameObject(nameof(MatrixVisualizer)).transform;
    }

    public void Spawn(IEnumerable<Matrix4x4> matrices, int prefabIndex)
    {
        foreach (var matrix in matrices)
        {
            var instance = Object.Instantiate(prefabs[prefabIndex], parent);
            var transform = instance.transform;
            transform.SetPositionAndRotation(matrix.GetPosition(), matrix.rotation);
            transform.localScale = matrix.lossyScale;
            spawned[prefabIndex].Add(instance);
        }
    }

    public void Clear(int prefabIndex)
    {
        foreach (var obj in spawned[prefabIndex])
            Object.Destroy(obj);
        spawned[prefabIndex].Clear();
    }
}