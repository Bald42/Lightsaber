using System;
using UnityEngine;
using Zenject;

namespace Lightsaber.Core
{
	public class AppLauncherReadOnly : IInitializable, IAppLauncherReadOnly
	{
		public event Action GameReadyEvent;
		public bool IsGameReady { get; private set; }

		public void Initialize()
		{
			// Телефон не должен засыпать посреди игры/калибровки — на ПК это поле просто не на что влиять.
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			IsGameReady = true;
			GameReadyEvent?.Invoke();
		}
	}
}