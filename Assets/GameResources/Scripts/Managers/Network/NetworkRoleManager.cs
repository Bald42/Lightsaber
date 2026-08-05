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
            OnRoleChanged?.Invoke(CurrentRole);
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

        private void DetermineRole()
        {
#if UNITY_ANDROID || UNITY_IOS
                CurrentRole = NetworkRole.Client;
#else
                CurrentRole = NetworkRole.Host;
#endif
        }
    }
}