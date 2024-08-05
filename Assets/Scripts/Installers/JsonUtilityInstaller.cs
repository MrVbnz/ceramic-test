using UnityEngine;
using Zenject;

public class JsonUtilityInstaller: MonoInstaller
{
    [SerializeField] private TextAsset modelAsset;
    [SerializeField] private TextAsset spaceAsset;
    
    public override void InstallBindings()
    {
        Container.BindInstance(new JsonUtility(modelAsset, spaceAsset));
    }
}