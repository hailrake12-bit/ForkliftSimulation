using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CargoFactory>().FromComponentInHierarchy().AsSingle();
        Container.BindInterfacesAndSelfTo<CargoAnimator>().FromComponentInHierarchy().AsSingle();

        Container.Bind<ForkliftInputs>().AsSingle();
        Container.Bind<ForkliftController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DashboardUI>().FromComponentInHierarchy().AsSingle();
    }
}