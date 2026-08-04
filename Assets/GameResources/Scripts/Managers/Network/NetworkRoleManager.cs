using System;
using UnityEngine;

namespace Lightsaber.Network
{
    public partial class NetworkRoleManager : MonoBehaviour, INetworkRoleManager
    {
        public event Action<NetworkRole> OnRoleChanged;

        [SerializeField] private Settings settings;

        public NetworkRole CurrentRole { get; private set; }

        private void Awake()
        {
            DetermineRole();
        }

        private void Start()
        {
            OnRoleChanged?.Invoke(CurrentRole);
        }

        private void Update()
        {
            HandleCheatInputs();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                DetermineRole();
                OnRoleChanged?.Invoke(CurrentRole);
            }
        }

        public void SetRole(NetworkRole newRole)
        {
            if (CurrentRole == newRole)
            {
                return;
            }

            CurrentRole = newRole;
            OnRoleChanged?.Invoke(CurrentRole);
        }

        public void ToggleRole()
        {
            NetworkRole nextRole = CurrentRole == NetworkRole.Host ? NetworkRole.Client : NetworkRole.Host;
            SetRole(nextRole);
        }

        private void DetermineRole()
        {
            if (settings != null && settings.UseTestOverride)
            {
                CurrentRole = settings.TestRole;
            }
            else
            {
#if UNITY_ANDROID || UNITY_IOS
                CurrentRole = NetworkRole.Client;
#else
                CurrentRole = NetworkRole.Host;
#endif
            }
        }

        private void HandleCheatInputs()
        {
            if (settings == null || !settings.EnableCheatHotkeys)
            {
                return;
            }

            if (Input.GetKeyDown(settings.HostCheatKey))
            {
                SetRole(NetworkRole.Host);
                Debug.Log("[Cheat] Network role forced to HOST via hotkey.");
            }
            else if (Input.GetKeyDown(settings.ClientCheatKey))
            {
                SetRole(NetworkRole.Client);
                Debug.Log("[Cheat] Network role forced to CLIENT via hotkey.");
            }
        }
    }
}