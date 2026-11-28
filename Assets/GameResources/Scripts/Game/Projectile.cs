using UnityEngine;
using Zenject;

namespace Lightsaber.Game
{
	// Снаряд дрона: Rigidbody (кинематический) + триггер-коллайдер — в Unity триггер не
	// сработает, если ни у одной из сторон нет Rigidbody (см. docs/features/6_DroneDuel.md,
	// грабля 3). Цель всегда фиксированная точка (посчитана один раз при вылете/отбитии, а не
	// отслеживается дальше), поэтому промах — это нормально, не баг, даже если дрон/игрок
	// после этого сместятся. Летит к игроку, пока не коснётся клинка (тег "Blade") — тогда
	// разворачивается на дрона. Урон наносит DroneDuelManager: отбитый снаряд бьёт дрона
	// (тег "Drone"), неотбитый — игрока (тег "Player"). Инжект работает только для объектов,
	// которых создаёт Zenject-фабрика или которых вручную инжектят через DiContainer.InjectGameObject
	// — см. DroneController.Fire().
	[RequireComponent(typeof(Rigidbody))]
	public class Projectile : MonoBehaviour
	{
		private const float HIT_DISTANCE = 0.2f;
		private const string BLADE_TAG = "Blade";
		private const string DRONE_TAG = "Drone";
		private const string PLAYER_TAG = "Player";

		[Inject] private IDroneDuelManager _duelManager;

		private Rigidbody _rigidbody;
		private Transform _droneTransform;
		private Vector3 _targetPosition;
		private float _speed;
		private float _reflectSpread;
		private bool _isReflected;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_rigidbody.isKinematic = true;
			_rigidbody.useGravity = false;
			_rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
		}

		private void FixedUpdate()
		{
			Vector3 toTarget = _targetPosition - transform.position;

			if (toTarget.magnitude <= HIT_DISTANCE)
			{
				Destroy(gameObject);
				return;
			}

			transform.rotation = Quaternion.LookRotation(toTarget);
			_rigidbody.MovePosition(transform.position + toTarget.normalized * _speed * Time.fixedDeltaTime);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_isReflected && other.CompareTag(BLADE_TAG))
			{
				_isReflected = true;
				_targetPosition = _droneTransform.position + Random.insideUnitSphere * _reflectSpread;
				return;
			}

			if (_isReflected && other.CompareTag(DRONE_TAG))
			{
				_duelManager.DamageDrone();
				Destroy(gameObject);
				return;
			}

			if (!_isReflected && other.CompareTag(PLAYER_TAG))
			{
				_duelManager.DamagePlayer();
				Destroy(gameObject);
			}
		}

		public void Launch(Transform playerTarget, Transform droneTransform, float speed, float reflectSpread,
			float aimSpread)
		{
			_droneTransform = droneTransform;
			_speed = speed;
			_reflectSpread = reflectSpread;
			_targetPosition = playerTarget.position + Random.insideUnitSphere * aimSpread;
			transform.rotation = Quaternion.LookRotation(_targetPosition - transform.position);
		}
	}
}
