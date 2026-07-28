using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ForkliftInputs>().AsSingle();

        Container.Bind<ForkliftController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CargoFactory>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DashboardUI>().FromComponentInHierarchy().AsSingle();
    }
}