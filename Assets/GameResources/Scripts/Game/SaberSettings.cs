using UnityEngine;

namespace Lightsaber.Game
{
	// Настройки меча: правятся в инспекторе и живьём через чит SaberCheatTab.
	// Один общий ассет на всех игроков, пока не понадобится разделение по игроку.
	// Рука пока не нужна: в фиче сейчас только правый хват (docs/features/4_SaberRotation.md),
	// выбор руки — отдельная история на будущее, не трогаем её сейчас.
	[CreateAssetMenu(fileName = "SaberSettings", menuName = "Lightsaber/Saber Settings")]
	public class SaberSettings : ScriptableObject
	{
		[field: SerializeField] public Vector3 GripCorrectionEuler { get; set; } = Vector3.zero;

		[field: SerializeField] public float NeutralTilt { get; set; }

		[field: SerializeField] public bool InvertX { get; set; }
		[field: SerializeField] public bool InvertY { get; set; }
		[field: SerializeField] public bool InvertZ { get; set; }
	}
}
