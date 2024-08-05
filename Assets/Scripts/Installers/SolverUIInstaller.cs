using UnityEngine;
using Zenject;

public class SolverUIInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        var obj = new GameObject(nameof(SolverUI));
        var ui = obj.AddComponent<SolverUI>();
        
        Container.BindInstance(ui);
        Container.QueueForInject(ui);
    }
}