using Zenject;

public class CargoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SlotTrigger>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CargoTrigger>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CargoFactory>().FromComponentInHierarchy().AsSingle();
    }
}