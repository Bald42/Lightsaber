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
			Debug.LogError($"<color=yellow>AppLauncherReadOnly Initialize</color>");
			IsGameReady = true;
			GameReadyEvent?.Invoke();
		}
	}
}