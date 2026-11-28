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

		// Сглаживание (SaberView.FixedUpdate): максимальная угловая скорость доворота к новой
		// цели, градусов в секунду. Срезает дребезг от неравномерных пакетов сети, но ниже
		// скорости реального взмаха мечом быть не должна — иначе появится вязкость/задержка.
		// Подбирается вживую.
		[field: SerializeField] public float SmoothingDegreesPerSecond { get; set; } = 720f;

		// Предсказание (SaberRotationProcessor): на сколько секунд вперёд доворачиваем сырой
		// attitude по текущей угловой скорости (rotationRate) — компенсация сетевой задержки
		// (40-70мс, см. DEVLOG). 0 — предсказание выключено. Подбирается вживую.
		[field: SerializeField] public float PredictionSeconds { get; set; } = 0.05f;
	}
}
