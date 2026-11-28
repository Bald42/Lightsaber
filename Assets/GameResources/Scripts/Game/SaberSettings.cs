using UnityEngine;

namespace Lightsaber.Game
{
	// Настройки меча: правятся в инспекторе.
	// Один общий ассет на всех игроков, пока не понадобится разделение по игроку.
	// Цепочка (SaberRotationProcessor): калибровка * поправкаМира * конвертация(raw) * поправкаХвата.
	[CreateAssetMenu(fileName = "SaberSettings", menuName = "Lightsaber/Saber Settings")]
	public class SaberSettings : ScriptableObject
	{
		// Поворот в системе самого телефона, умножается справа. (0, 180, 0) разворачивает
		// модель так, чтобы грань ScreenFace была со стороны настоящего экрана.
		// Зависит от модели: при смене рукояти может понадобиться другой.
		[field: SerializeField] public Vector3 GripCorrectionEuler { get; set; } = new Vector3(0f, 180f, 0f);

		// Куда смотрит клинок после калибровки, градусы вокруг вертикали. 0 — в экран, от камеры.
		[field: SerializeField] public float CalibrationTargetYaw { get; set; }
	}
}
