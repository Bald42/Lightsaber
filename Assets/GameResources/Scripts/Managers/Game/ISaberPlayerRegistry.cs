namespace Lightsaber.Game
{
	public interface ISaberPlayerRegistry : ISaberPlayerRegistryReadOnly
	{
		void Register(SaberView view);
		void Unregister(SaberView view);
	}
}
