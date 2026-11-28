using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Lightsaber.Game
{
	// Фидбек по урону без цифр (см. docs/features/6_DroneDuel.md, п.7): дрон мигает красным,
	// у игрока — красная виньетка на экране. Конца дуэли нет — это чисто "попадание засчиталось".
	public class DroneDuelView : MonoBehaviour
	{
		private const float DRONE_FLASH_SECONDS = 0.15f;
		private const float VIGNETTE_FADE_SECONDS = 0.4f;
		private const float VIGNETTE_PEAK_ALPHA = 0.45f;

		[Inject] private IDroneDuelManagerReadOnly _duelManager;

		[SerializeField] private Renderer[] _droneRenderers;
		[SerializeField] private Material _droneHitMaterial;
		[SerializeField] private Image _playerVignetteImage;

		private Material[] _droneOriginalMaterials;
		private Coroutine _droneFlashRoutine;
		private Coroutine _vignetteRoutine;

		private void Awake()
		{
			_droneOriginalMaterials = new Material[_droneRenderers.Length];
			for (int i = 0; i < _droneRenderers.Length; i++)
			{
				_droneOriginalMaterials[i] = _droneRenderers[i].sharedMaterial;
			}
		}

		private void OnEnable()
		{
			_duelManager.DroneDamaged += DroneDamagedHandle;
			_duelManager.PlayerDamaged += PlayerDamagedHandle;
		}

		private void OnDisable()
		{
			_duelManager.DroneDamaged -= DroneDamagedHandle;
			_duelManager.PlayerDamaged -= PlayerDamagedHandle;
		}

		private void DroneDamagedHandle()
		{
			if (_droneFlashRoutine != null)
			{
				StopCoroutine(_droneFlashRoutine);
			}

			_droneFlashRoutine = StartCoroutine(DroneFlashRoutine());
		}

		private void PlayerDamagedHandle()
		{
			if (_vignetteRoutine != null)
			{
				StopCoroutine(_vignetteRoutine);
			}

			_vignetteRoutine = StartCoroutine(VignetteFadeRoutine());
		}

		private IEnumerator DroneFlashRoutine()
		{
			for (int i = 0; i < _droneRenderers.Length; i++)
			{
				_droneRenderers[i].sharedMaterial = _droneHitMaterial;
			}

			yield return new WaitForSeconds(DRONE_FLASH_SECONDS);

			for (int i = 0; i < _droneRenderers.Length; i++)
			{
				_droneRenderers[i].sharedMaterial = _droneOriginalMaterials[i];
			}

			_droneFlashRoutine = null;
		}

		private IEnumerator VignetteFadeRoutine()
		{
			float elapsed = 0f;
			Color color = _playerVignetteImage.color;

			while (elapsed < VIGNETTE_FADE_SECONDS)
			{
				elapsed += Time.deltaTime;
				color.a = Mathf.Lerp(VIGNETTE_PEAK_ALPHA, 0f, elapsed / VIGNETTE_FADE_SECONDS);
				_playerVignetteImage.color = color;
				yield return null;
			}

			color.a = 0f;
			_playerVignetteImage.color = color;
			_vignetteRoutine = null;
		}
	}
}
