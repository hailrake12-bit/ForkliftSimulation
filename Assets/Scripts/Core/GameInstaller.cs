using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ForkliftController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<ForkController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CargoFactory>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DashboardUI>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UnloadZone>().FromComponentInHierarchy().AsSingle();
    }
}