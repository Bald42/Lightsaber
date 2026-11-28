using UnityEngine;
using Zenject;

namespace Lightsaber.Game
{
	// Простое (не физическое) перемещение дрона: рандомная точка внутри зоны вокруг стартовой
	// позиции, долетел — новая точка. Зона — куб размером DroneSettings.MoveZoneSize, центр —
	// позиция дрона в момент Awake. Живёт в GameScene как обычный GameObject (не NetworkBehaviour).
	public class DroneController : MonoBehaviour
	{
		// Для чита: пауза останавливает и полёт, и стрельбу.
		public bool IsPaused { get; set; }

		private const float ARRIVAL_DISTANCE = 0.05f;

		[Inject] private DroneSettings _settings;
		[Inject] private DiContainer _container;

		// Куда летят снаряды — ориентир игрока (пока камера хоста, см. docs/features/6_DroneDuel.md).
		[SerializeField] private Transform _playerTarget;
		[SerializeField] private Projectile _projectilePrefab;

		// Точка вылета снаряда — дочерний объект Emitter. Весь дрон разворачивается к игроку
		// в Update(), поэтому эмиттер (он смотрит вперёд по локальному Z) всегда оказывается
		// на стороне, обращённой к игроку.
		[SerializeField] private Transform _emitterPoint;

		private Vector3 _zoneCenter;
		private Vector3 _targetPoint;
		private float _timeUntilNextShot;

		private void Awake()
		{
			_zoneCenter = transform.position;
			_targetPoint = RandomPointInZone();
		}

		private void Update()
		{
			if (IsPaused)
			{
				return;
			}

			transform.position = Vector3.MoveTowards(transform.position, _targetPoint,
				_settings.MoveSpeed * Time.deltaTime);

			if (Vector3.Distance(transform.position, _targetPoint) <= ARRIVAL_DISTANCE)
			{
				_targetPoint = RandomPointInZone();
			}

			transform.rotation = Quaternion.LookRotation(_playerTarget.position - transform.position);

			_timeUntilNextShot -= Time.deltaTime;
			if (_timeUntilNextShot <= 0f)
			{
				Fire();
				_timeUntilNextShot = _settings.FireInterval;
			}
		}

		// Зона видна в редакторе (выдели объект), чтобы настраивать границы полёта без Play.
		private void OnDrawGizmosSelected()
		{
			Vector3 size = _settings != null ? _settings.MoveZoneSize : Vector3.one;
			Vector3 center = Application.isPlaying ? _zoneCenter : transform.position;

			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(center, size);
		}

		// Для чита «Сбросить дуэль»: дрон обратно в зону, на новую случайную точку.
		public void ResetPosition()
		{
			transform.position = _zoneCenter;
			_targetPoint = RandomPointInZone();
		}

		// Projectile спавнится обычным Instantiate, не через Zenject-фабрику — без ручного
		// InjectGameObject его [Inject]-поля (IDroneDuelManager) останутся пустыми.
		private void Fire()
		{
			Projectile projectile = Instantiate(_projectilePrefab, _emitterPoint.position, Quaternion.identity);
			_container.InjectGameObject(projectile.gameObject);
			projectile.Launch(_playerTarget, transform, _settings.ProjectileSpeed, _settings.ReflectSpread,
				_settings.AimSpread);
		}

		private Vector3 RandomPointInZone()
		{
			Vector3 size = _settings.MoveZoneSize;
			Vector3 offset = new Vector3(
				Random.Range(-size.x / 2f, size.x / 2f),
				Random.Range(-size.y / 2f, size.y / 2f),
				Random.Range(-size.z / 2f, size.z / 2f));

			return _zoneCenter + offset;
		}
	}
}
