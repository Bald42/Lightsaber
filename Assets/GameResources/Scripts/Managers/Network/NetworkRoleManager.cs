using System;
using UnityEngine;
using Zenject;

namespace Lightsaber.Network
{
    public partial class NetworkRoleManager : INetworkRoleManager, IInitializable
    {
        public event Action<NetworkRole> OnRoleChanged;
        public NetworkRole CurrentRole { get; private set; }
        
        public void Initialize()
        {
            DetermineRole();
        }

        public void SetRole(NetworkRole newRole)
        {
            if (CurrentRole == newRole)
            {
                return;
            }

            CurrentRole = newRole;
            OnRoleChanged?.Invoke(CurrentRole);
            Debug.LogError($"<color=green>SetRole {CurrentRole}</color>");
        }

        private void DetermineRole()
        {
#if UNITY_EDITOR
            SetRole(NetworkRole.Host);
#elif UNITY_ANDROID || UNITY_IOS
            SetRole(NetworkRole.Client);
#else
            SetRole(NetworkRole.Host);
#endif
        }
    }
}