using System;
using System.Collections.Generic;
using System.Linq;
using Lightsaber.Cheats;
using Lightsaber.Game;
using Lightsaber.Network;
using Mirror;
using UnityEngine;
using Zenject;

namespace Lightsaber.Core
{
	public class GameInstaller : MonoInstaller
	{
		[SerializeField] private SaberSettings _saberSettings;
		[SerializeField] private DroneSettings _droneSettings;

		public override void InstallBindings()
		{
			// 1. Биндим менеджер ролей (Zenject сам подтянет IInitializable)
			Container
				.BindInterfacesAndSelfTo<NetworkRoleManager>()
				.AsSingle();

			// 2. Биндим контроллер подключения (закроет INetworkConnectionController, INetworkConnectionControllerReadOnly и IDisposable)
			Container
				.BindInterfacesAndSelfTo<NetworkConnectionController>()
				.AsSingle();
			
			Container.Bind<NetworkManager>()
				.To<CustomNetworkManager>()
				.FromComponentInHierarchy()
				.AsSingle();
			
			// Настройки меча и реестр активных мечей (Lightsaber.Game)
			Container.BindInstance(_saberSettings).AsSingle();
			Container.BindInterfacesAndSelfTo<SaberPlayerRegistry>().AsSingle();

			// Настройки дуэли с дроном (Lightsaber.Game)
			Container.BindInstance(_droneSettings).AsSingle();
			Container.BindInterfacesAndSelfTo<DroneDuelManager>().AsSingle();

			// 4. Биндим AppLauncherReadOnly ПОСЛЕДНИМ
			// Он гарантированно вызовется после того, как заинитятся все менеджеры выше
			Container
				.BindInterfacesAndSelfTo<AppLauncherReadOnly>()
				.AsSingle();

			BindCheats();
		}

		private void BindCheats()
		{
			if (!Debug.isDebugBuild)
				return;

			SRDebug.Init();

			IEnumerable<Type> tabs = AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(asm => asm
					.GetTypes()
					.Where(type => type.IsSubclassOf(typeof(CheatTab))));

			foreach (Type type in tabs)
				Container.BindInterfacesAndSelfTo(type).AsSingle();

			Container.BindInterfacesAndSelfTo<CheatController>().AsSingle();
		}
	}
}