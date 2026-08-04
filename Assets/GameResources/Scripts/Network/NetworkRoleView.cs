using UnityEngine;

namespace Lightsaber.Network
{
	public class NetworkRoleView : MonoBehaviour
	{
		[SerializeField] private NetworkRoleManager roleManager;
		[SerializeField] private GameObject targetObject;
		[SerializeField] private NetworkRole targetRole = NetworkRole.Host;
		[SerializeField] private bool invertVisibility = false;

		private INetworkRoleManagerReadOnly _roleManagerReadOnly;

		private void Awake()
		{
			if (targetObject == null)
			{
				targetObject = gameObject;
			}

			_roleManagerReadOnly = roleManager;
		}

		private void OnEnable()
		{
			if (_roleManagerReadOnly != null)
			{
				_roleManagerReadOnly.OnRoleChanged += HandleRoleChanged;
				UpdateVisibility(_roleManagerReadOnly.CurrentRole);
			}
		}

		private void OnDisable()
		{
			if (_roleManagerReadOnly != null)
			{
				_roleManagerReadOnly.OnRoleChanged -= HandleRoleChanged;
			}
		}

		/// <summary>
		/// Временный сеттер до внедрения Zenject [Inject]
		/// </summary>
		public void Construct(INetworkRoleManagerReadOnly roleManagerReadOnly)
		{
			_roleManagerReadOnly = roleManagerReadOnly;
		}

		private void HandleRoleChanged(NetworkRole currentRole)
		{
			UpdateVisibility(currentRole);
		}

		private void UpdateVisibility(NetworkRole currentRole)
		{
			bool shouldBeActive = (currentRole == targetRole);

			if (invertVisibility)
			{
				shouldBeActive = !shouldBeActive;
			}

			if (targetObject != null)
			{
				targetObject.SetActive(shouldBeActive);
			}
		}
	}
}