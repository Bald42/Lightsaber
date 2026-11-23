using System;
using System.Collections.Generic;

namespace Lightsaber.Game
{
	// Реестр активных мечей на хосте: чит достаёт отсюда игроков, без статиков.
	// Сейчас игрок один, но интерфейс уже на N.
	public class SaberPlayerRegistry : ISaberPlayerRegistry
	{
		private readonly List<SaberView> _players = new List<SaberView>();

		public IReadOnlyList<SaberView> Players => _players;

		public event Action<SaberView> PlayerRegistered;
		public event Action<SaberView> PlayerUnregistered;

		public void Register(SaberView view)
		{
			if (_players.Contains(view))
			{
				return;
			}

			_players.Add(view);
			PlayerRegistered?.Invoke(view);
		}

		public void Unregister(SaberView view)
		{
			if (!_players.Remove(view))
			{
				return;
			}

			PlayerUnregistered?.Invoke(view);
		}
	}
}
