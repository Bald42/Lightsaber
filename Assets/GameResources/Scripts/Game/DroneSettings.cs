using UnityEngine;

namespace Lightsaber.Game
{
	// Настройки дуэли с дроном: правятся в инспекторе. Один общий ассет, пока дрон один.
	[CreateAssetMenu(fileName = "DroneSettings", menuName = "Lightsaber/Drone Settings")]
	public class DroneSettings : ScriptableObject
	{
		// Размер зоны полёта (куб вокруг стартовой позиции дрона), метры.
		[field: SerializeField] public Vector3 MoveZoneSize { get; set; } = new Vector3(3f, 2f, 3f);

		// Скорость перемещения дрона между точками, м/с.
		[field: SerializeField] public float MoveSpeed { get; set; } = 2f;

		// Интервал между выстрелами, секунды.
		[field: SerializeField] public float FireInterval { get; set; } = 2f;

		// Скорость снаряда, м/с.
		[field: SerializeField] public float ProjectileSpeed { get; set; } = 4f;

		// Случайный разброс точки прицеливания отбитого снаряда вокруг дрона, метры.
		// Чем больше — тем больше шанс промаха при отбитии.
		[field: SerializeField] public float ReflectSpread { get; set; } = 0.5f;

		// Случайный разброс прицела при обычном выстреле по игроку, метры. Без него снаряд летит
		// точно в камеру и визуально выглядит неподвижной точкой, а не летящим объектом.
		[field: SerializeField] public float AimSpread { get; set; } = 0.2f;

		// HP дрона и игрока на старт дуэли.
		[field: SerializeField] public int DroneHp { get; set; } = 3;
		[field: SerializeField] public int PlayerHp { get; set; } = 3;

		// Урон за одно попадание.
		[field: SerializeField] public int DamageToDrone { get; set; } = 1;
		[field: SerializeField] public int DamageToPlayer { get; set; } = 1;
	}
}
