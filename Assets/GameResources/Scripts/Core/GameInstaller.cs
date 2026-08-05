using System;
using System.Collections.Generic;
using System.Linq;
using Lightsaber.Cheats;
using Zenject;
using Lightsaber.Network;
using UnityEngine;

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