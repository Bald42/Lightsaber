using Zenject;
using Lightsaber.Network;

namespace Lightsaber.Core
{
	public class GameInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			// 1. Биндим менеджер ролей (Zenject сам подтянет IInitializable)
			Container
				.BindInterfacesAndSelfTo<NetworkRoleManager>()
				.AsSingle();

			// 2. Биндим AppLauncherReadOnly ПОСЛЕДНИМ
			// Он гарантированно вызовется после того, как заинитятся все менеджеры выше
			Container
				.BindInterfacesAndSelfTo<AppLauncherReadOnly>()
				.AsSingle();
		}
	}
}