using System.Collections.Generic;
using Zenject;

namespace Lightsaber.Cheats
{
	public class CheatController : IInitializable
	{
		[Inject] private IEnumerable<ICheatTab> _tabs;
		
		public void Initialize()
		{
			foreach (ICheatTab tab in _tabs)
			{
				tab.Initialize();
			}
		}
	}
}