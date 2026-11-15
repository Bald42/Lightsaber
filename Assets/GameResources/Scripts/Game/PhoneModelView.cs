using UnityEngine;

namespace Lightsaber.Game
{
	// Отладочная модель «телефон»: подставляет сырой кватернион прямо в transform.rotation,
	// без конвертации осей. Нужна, чтобы руками сверить оси по таблице поз
	// (docs/features/4_SaberRotation.md) — это не финальная модель меча.
	public class PhoneModelView : MonoBehaviour
	{
		public void SetRawAttitude(Quaternion attitude)
		{
			transform.rotation = attitude;
		}
	}
}
