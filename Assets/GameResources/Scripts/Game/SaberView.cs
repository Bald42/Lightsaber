using UnityEngine;
using Zenject;

namespace Lightsaber.Game
{
	// Телефон и клинок — одна деталь (рукоять + лезвие), крутится целиком по пересчитанному
	// повороту. Живёт на хосте: SaberPlayerNetwork инстанциирует этот объект и сразу после
	// инъекции зовёт Register().
	public class SaberView : MonoBehaviour
	{
		public Quaternion LastRawAttitude { get; private set; } = Quaternion.identity;
		public Quaternion LastProcessedRotation { get; private set; } = Quaternion.identity;
		public bool IsFrozen { get; set; }

		[Inject] private SaberSettings _settings;
		[Inject] private ISaberPlayerRegistry _registry;

		// Не цель поворота — нужен только чтобы не прятать клинок при "Show phone model".
		[SerializeField] private Transform _bladeTransform;

		private SaberRotationProcessor _processor;

		private void OnDestroy()
		{
			_registry?.Unregister(this);
		}

		// Зовёт SaberPlayerNetwork сразу после Instantiate + InjectGameObject —
		// до этого момента [Inject]-поля ещё не заполнены (Awake уже отработал бы с пустыми).
		public void Register()
		{
			_registry.Register(this);
		}

		public void ApplyRawAttitude(Quaternion rawAttitude)
		{
			_processor ??= new SaberRotationProcessor(_settings);

			LastRawAttitude = rawAttitude;
			LastProcessedRotation = _processor.Process(rawAttitude);

			if (IsFrozen)
			{
				return;
			}

			// Телефон и меч — один жёсткий объект, крутим всю модельку целиком.
			transform.rotation = LastProcessedRotation;
		}

		// Прячет визуал "телефонной" части (корпус + ScreenFace); клинок остаётся виден
		// и крутится вместе с остальным объектом в любом случае — это чисто про видимость.
		public void SetPhoneModelVisible(bool isVisible)
		{
			foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
			{
				if (_bladeTransform != null && renderer.transform == _bladeTransform)
				{
					continue;
				}

				renderer.enabled = isVisible;
			}
		}
	}
}
