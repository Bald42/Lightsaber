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
		public bool IsCalibrated { get; private set; }

		[Inject] private SaberSettings _settings;
		[Inject] private ISaberPlayerRegistry _registry;

		// Не цель поворота — нужен только чтобы не прятать клинок при "Show phone model".
		[SerializeField] private Transform _bladeTransform;

		private const float TRANSIENT_HINT_SECONDS = 2f;
		private const string NOT_CALIBRATED_HINT = "Не откалибровано: телефон на стол экраном вверх, " +
			"динамиком к монитору, пробел (кликни в окно Game) или Calibrate в читах";
		private const string FAILED_HINT = "Калибровка не прошла: клинок почти вертикальный, положи телефон на стол";
		private const string SUCCESS_HINT = "Откалибровано";

		private SaberRotationProcessor _processor;
		private GUIStyle _hintStyle;
		private string _transientHint;
		private float _transientHintUntil;

		private void OnDestroy()
		{
			_registry?.Unregister(this);
		}

		// Временное упрощение теста калибровки: пробел на хосте — тот же эффект, что кнопка
		// "Calibrate" в чите. Лежит здесь, а не в SaberPlayerNetwork, т.к. SaberView существует
		// только на хосте (есть физическая клавиатура), а SaberPlayerNetwork — сетевой компонент
		// телефона, его Update обрабатывает ввод именно на клиенте.
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				Calibrate();
			}
		}

		// Отладочная подсказка на экране хоста: калибровка сбрасывается при каждом старте Play,
		// без надписи легко забыть её сделать. Пока игрок один — у N игроков надписи наложатся.
		private void OnGUI()
		{
			string hint = CurrentHint();
			if (hint == null)
			{
				return;
			}

			_hintStyle ??= new GUIStyle(GUI.skin.label)
			{
				fontSize = 28,
				alignment = TextAnchor.UpperCenter,
				wordWrap = true
			};

			Rect rect = new Rect(120f, 20f, Screen.width - 240f, 160f);

			// Чёрная тень на 2 пикселя — чтобы текст читался и на небе, и на полу.
			_hintStyle.normal.textColor = Color.black;
			GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), hint, _hintStyle);
			_hintStyle.normal.textColor = Color.yellow;
			GUI.Label(rect, hint, _hintStyle);
		}

		// Зовёт SaberPlayerNetwork сразу после Instantiate + InjectGameObject —
		// до этого момента [Inject]-поля ещё не заполнены (Awake уже отработал бы с пустыми).
		public void Register()
		{
			_registry.Register(this);
		}

		public void Calibrate()
		{
			_processor ??= new SaberRotationProcessor(_settings);

			if (_processor.Calibrate(LastRawAttitude))
			{
				IsCalibrated = true;
				ShowTransientHint(SUCCESS_HINT);
				return;
			}

			ShowTransientHint(FAILED_HINT);
			Debug.LogWarning("[SaberView] Калибровка пропущена: клинок почти вертикален. " +
				"Положи телефон на стол экраном вверх, динамиком к монитору.");
		}

		public void ResetCalibration()
		{
			_processor?.ResetCalibration();
			IsCalibrated = false;
			_transientHintUntil = 0f;
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

		private void ShowTransientHint(string hint)
		{
			_transientHint = hint;
			_transientHintUntil = Time.time + TRANSIENT_HINT_SECONDS;
		}

		// Короткое сообщение (успех/неудача) важнее постоянного; без калибровки висит инструкция.
		private string CurrentHint()
		{
			if (Time.time < _transientHintUntil)
			{
				return _transientHint;
			}

			return IsCalibrated ? null : NOT_CALIBRATED_HINT;
		}
	}
}
