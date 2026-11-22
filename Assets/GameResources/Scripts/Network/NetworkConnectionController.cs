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
			"172.20.10.",
			"127.0.0."
		};

		// Разделенные флаги состояний
		public bool IsServerStarted { get; private set; }
		public bool IsClientConnecting { get; private set; }
		public bool IsClientConnected { get; private set; }

		public string CurrentPinCode { get; private set; }
		public string CurrentIpAddress { get; private set; }
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
				// Серверные подписки
				customManager.OnServerConnected += OnPlayerConnected;
				customManager.OnServerDisconnected += OnPlayerDisconnected;

				// Клиентские подписки
				customManager.OnClientConnected += OnClientConnectHandler;
				customManager.OnClientDisconnected += OnClientDisconnectHandler;
			}
			
			AddCurrentPinCode();
			
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
			
			IsServerStarted = true;

			OnServerStartedEvent?.Invoke(new ServerStartedEvent
			{
				IpAddress = CurrentIpAddress,
				PinCode = CurrentPinCode
			});
		}

		private void AddCurrentPinCode()
		{
			CurrentIpAddress = NetworkCodeUtility.GetLocalIPAddress();
			CurrentPinCode = NetworkCodeUtility.IpToPin(CurrentIpAddress);
		}
		
		public void ConnectByPin(string pin)
		{
			string upperPin = pin.Trim().ToUpper();

			if (string.IsNullOrEmpty(upperPin) || upperPin.Length != 4)
			{
				OnConnectionFailedEvent?.Invoke("Invalid PIN length");
				return;
			}

			string targetIp = FindValidIpForPin(upperPin);

			if (string.IsNullOrEmpty(targetIp))
			{
				OnConnectionFailedEvent?.Invoke("Failed to decode PIN");
				return;
			}

			ConnectToTargetIp(targetIp);
		}

		// Запасной путь для кнопки "не подключилось" в лобби: если автоопределение подсети
		// промахнулось (PIN на вид верный, но хост не найден) — подставляем первые два числа
		// IP хоста руками, минуя FindValidIpForPin целиком.
		public void ConnectByPinWithSubnet(string pin, string subnetPrefix)
		{
			string upperPin = pin.Trim().ToUpper();

			if (string.IsNullOrEmpty(upperPin) || upperPin.Length != 4)
			{
				OnConnectionFailedEvent?.Invoke("Invalid PIN length");
				return;
			}

			string targetIp = NetworkCodeUtility.PinToIp(upperPin, subnetPrefix);

			if (string.IsNullOrEmpty(targetIp))
			{
				OnConnectionFailedEvent?.Invoke("Failed to decode PIN");
				return;
			}

			ConnectToTargetIp(targetIp);
		}

		public string GetTargetIp()
		{
			return _mirrorNetworkManager != null ? _mirrorNetworkManager.networkAddress : string.Empty;
		}

		// Подсказка для поля ручного ввода подсети: что мы и так уже пробуем автоматически,
		// чтобы человек правил цифры, а не вводил их с нуля.
		public string GetDefaultSubnetGuess()
		{
			string selfIp = NetworkCodeUtility.GetLocalIPAddress();

			if (!string.IsNullOrEmpty(selfIp) && !selfIp.StartsWith("127."))
			{
				string[] parts = selfIp.Split('.');
				if (parts.Length >= 2)
				{
					return $"{parts[0]}.{parts[1]}";
				}
			}

			return _defaultSubnets[0].TrimEnd('.');
		}

		private void ConnectToTargetIp(string targetIp)
		{
			// Проверка: если клиент уже в процессе коннекта — игнорим повторные клики
			if (IsClientConnecting)
			{
				Debug.LogWarning("[NetworkConnectionController] Connection attempt already in progress...");
				return;
			}

			// Если был старый коннект — сбрасываем
			if (IsClientConnected)
			{
				_mirrorNetworkManager.StopClient();
			}

			IsClientConnecting = true;
			IsClientConnected = false;
			_networkRoleManager.SetRole(NetworkRole.Client);

			_mirrorNetworkManager.networkAddress = targetIp;
			Debug.Log($"<color=green>[NetworkConnectionController]</color> Client connecting to {targetIp}...");

			_mirrorNetworkManager.StartClient();
		}

		public void Disconnect()
		{
			if (_networkRoleManager.CurrentRole == NetworkRole.None)
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
			
			IsServerStarted = false;
			IsClientConnecting = false;
			IsClientConnected = false;
			
			CurrentPinCode = string.Empty;
			CurrentIpAddress = string.Empty;
			ConnectedPlayersCount = 0;

			OnClientDisconnectedEvent?.Invoke();
		}

		private void OnClientConnectHandler()
		{
			IsClientConnecting = false;
			IsClientConnected = true;
			Debug.Log($"<color=green>[NetworkConnectionController]</color> Client connected!");
			OnClientConnectedEvent?.Invoke();
		}

		private void OnClientDisconnectHandler()
		{
			IsClientConnecting = false;
			IsClientConnected = false;
			Debug.Log($"<color=orange>[NetworkConnectionController]</color> Client disconnected.");
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

				customManager.OnClientConnected -= OnClientConnectHandler;
				customManager.OnClientDisconnected -= OnClientDisconnectHandler;
			}
			Disconnect();
		}

		private string FindValidIpForPin(string pin)
		{
			string selfIp = NetworkCodeUtility.GetLocalIPAddress();

			// "127." — это заглушка GetLocalIPAddress на случай, если адаптер вообще не нашёлся.
			// Строить из неё подсеть бессмысленно, сразу уходим к списку дефолтных подсетей.
			if (!string.IsNullOrEmpty(selfIp) && selfIp.Contains(".") && !selfIp.StartsWith("127."))
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