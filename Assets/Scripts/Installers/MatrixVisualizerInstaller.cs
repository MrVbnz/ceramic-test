using UnityEngine;
using Zenject;

public class MatrixVisualizerInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] prefabs;

    public override void InstallBindings()
    {
        Container.BindInstance(new MatrixVisualizer(prefabs));
    }
}