using System;
using System.Collections.Generic;
using Lightsaber.Network.Utility;
using Mirror;
using UnityEngine;
using Zenject;

namespace Lightsaber.Network
{
	public struct ServerStartedEvent
	{
		public string IpAddress;
		public string PinCode;
	}

	public class NetworkConnectionController : INetworkConnectionController, IInitializable, IDisposable
	{
		private readonly NetworkManager _mirrorNetworkManager;
		private readonly INetworkRoleManager _networkRoleManager;

		private readonly List<string> _defaultSubnets = new List<string>
		{
			"192.168.1.",
			"192.168.0.",
			"192.168.43.",
			"172.20.10."
		};

		public bool IsConnecting { get; private set; }
		public bool IsServerStarted { get; private set; }
		public string CurrentPinCode { get; private set; }
		public int ConnectedPlayersCount { get; private set; }

		public event Action<ServerStartedEvent> OnServerStartedEvent;
		public event Action OnClientConnectedEvent;
		public event Action OnClientDisconnectedEvent;
		public event Action<int> OnPlayerCountChangedEvent;
		public event Action<string> OnConnectionFailedEvent;

		public NetworkConnectionController(
			NetworkManager mirrorNetworkManager,
			INetworkRoleManager networkRoleManager)
		{
			_mirrorNetworkManager = mirrorNetworkManager;
			_networkRoleManager = networkRoleManager;
		}

		public void Initialize()
		{
			if (_mirrorNetworkManager is CustomNetworkManager customManager)
			{
				customManager.OnServerConnected += OnPlayerConnected;
				customManager.OnServerDisconnected += OnPlayerDisconnected;
			}
			
			if (_networkRoleManager.CurrentRole == NetworkRole.Host)
			{
				StartHost();
			}
		}

		public void StartHost()
		{
			if (IsServerStarted)
			{
				return;
			}

			_networkRoleManager.SetRole(NetworkRole.Host);
			_mirrorNetworkManager.StartHost();

			ConnectedPlayersCount = 0;

			string localIp = NetworkCodeUtility.GetLocalIPAddress();
			CurrentPinCode = NetworkCodeUtility.IpToPin(localIp);
			IsServerStarted = true;

			OnServerStartedEvent?.Invoke(new ServerStartedEvent
			{
				IpAddress = localIp,
				PinCode = CurrentPinCode
			});
		}

		public void ConnectByPin(string pin)
		{
			if (_networkRoleManager.CurrentRole != NetworkRole.None || IsConnecting)
			{
				return;
			}

			if (string.IsNullOrEmpty(pin) || pin.Length != 4)
			{
				OnConnectionFailedEvent?.Invoke("Невалидная длина PIN-кода");
				return;
			}

			string targetIp = FindValidIpForPin(pin);

			if (string.IsNullOrEmpty(targetIp))
			{
				OnConnectionFailedEvent?.Invoke("Не удалось расшифровать PIN-код");
				return;
			}

			IsConnecting = true;
			_networkRoleManager.SetRole(NetworkRole.Client);

			_mirrorNetworkManager.networkAddress = targetIp;
			_mirrorNetworkManager.StartClient();
		}

		public void Disconnect()
		{
			if (_networkRoleManager.CurrentRole == NetworkRole.None && !IsConnecting)
			{
				return;
			}

			if (_networkRoleManager.CurrentRole == NetworkRole.Host)
			{
				_mirrorNetworkManager.StopHost();
			}
			else if (_networkRoleManager.CurrentRole == NetworkRole.Client)
			{
				_mirrorNetworkManager.StopClient();
			}

			_networkRoleManager.SetRole(NetworkRole.None);
			IsConnecting = false;
			IsServerStarted = false;
			CurrentPinCode = string.Empty;
			ConnectedPlayersCount = 0;

			OnClientDisconnectedEvent?.Invoke();
		}

		public void OnPlayerConnected()
		{
			ConnectedPlayersCount++;
			OnPlayerCountChangedEvent?.Invoke(ConnectedPlayersCount);
		}

		public void OnPlayerDisconnected()
		{
			ConnectedPlayersCount = Mathf.Max(0, ConnectedPlayersCount - 1);
			OnPlayerCountChangedEvent?.Invoke(ConnectedPlayersCount);
		}

		public void Dispose()
		{
			if (_mirrorNetworkManager is CustomNetworkManager customManager)
			{
				customManager.OnServerConnected -= OnPlayerConnected;
				customManager.OnServerDisconnected -= OnPlayerDisconnected;
			}
			Disconnect();
		}

		private string FindValidIpForPin(string pin)
		{
			string selfIp = NetworkCodeUtility.GetLocalIPAddress();

			if (!string.IsNullOrEmpty(selfIp) && selfIp.Contains("."))
			{
				int lastDotIndex = selfIp.LastIndexOf('.');
				string currentSubnet = selfIp.Substring(0, lastDotIndex + 1);

				string candidateIp = NetworkCodeUtility.PinToIp(pin, currentSubnet);
				if (!string.IsNullOrEmpty(candidateIp))
				{
					return candidateIp;
				}
			}

			foreach (string prefix in _defaultSubnets)
			{
				string candidateIp = NetworkCodeUtility.PinToIp(pin, prefix);
				if (!string.IsNullOrEmpty(candidateIp))
				{
					return candidateIp;
				}
			}

			return null;
		}
	}
}