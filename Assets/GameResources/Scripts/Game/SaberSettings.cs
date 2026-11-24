using UnityEngine;

namespace Lightsaber.Game
{
	// Настройки меча: правятся в инспекторе и живьём через чит SaberCheatTab.
	// Один общий ассет на всех игроков, пока не понадобится разделение по игроку.
	// Цепочка (SaberRotationProcessor): калибровка * поправкаМира * конвертация(raw) * поправкаХвата.
	[CreateAssetMenu(fileName = "SaberSettings", menuName = "Lightsaber/Saber Settings")]
	public class SaberSettings : ScriptableObject
	{
		// Поворот мира, умножается слева. У гироскопа Android небо по оси Z, у Unity по Y:
		// (90, 0, 0) ставит небо вверх.
		[field: SerializeField] public Vector3 WorldCorrectionEuler { get; set; } = new Vector3(90f, 0f, 0f);

		// Поворот в системе самого телефона, умножается справа. (0, 180, 0) разворачивает
		// модель так, чтобы грань ScreenFace была со стороны настоящего экрана.
		[field: SerializeField] public Vector3 GripCorrectionEuler { get; set; } = new Vector3(0f, 180f, 0f);

		// Куда смотрит клинок после калибровки, градусы вокруг вертикали. 0 — в экран, от камеры.
		[field: SerializeField] public float CalibrationTargetYaw { get; set; }

		[field: SerializeField] public float NeutralTilt { get; set; }

		[field: SerializeField] public bool InvertX { get; set; }
		[field: SerializeField] public bool InvertY { get; set; }
		[field: SerializeField] public bool InvertZ { get; set; }
	}
}
