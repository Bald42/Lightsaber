using UnityEngine;
using Zenject;

namespace Lightsaber.Network
{
	public class NetworkRoleActivator : MonoBehaviour
	{
		[Inject] private INetworkRoleManagerReadOnly _networkRoleManager;
		[SerializeField] private NetworkRole targetRole = NetworkRole.Host;

		private void Awake()
		{
			_networkRoleManager.OnRoleChanged += HandleRoleChanged;
			UpdateVisibility(_networkRoleManager.CurrentRole);
		}

		private void OnDestroy()
		{
			_networkRoleManager.OnRoleChanged -= HandleRoleChanged;
		}

		private void HandleRoleChanged(NetworkRole currentRole)
		{
			UpdateVisibility(currentRole);
		}

		private void UpdateVisibility(NetworkRole currentRole)
		{
			gameObject.SetActive(currentRole == targetRole);
		}
	}
}