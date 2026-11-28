namespace Lightsaber.Game
{
	public interface IDroneDuelManager : IDroneDuelManagerReadOnly
	{
		void DamageDrone();
		void DamagePlayer();
		void SetDroneHp(int value);
		void SetPlayerHp(int value);
		void ResetDuel();
	}
}
