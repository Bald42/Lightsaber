using Mirror;
using UnityEngine;
using Zenject;

namespace Lightsaber.Network
{
	public class CustomNetworkManager : NetworkManager
	{
		// Серверные события
		public event System.Action OnServerConnected;
		public event System.Action OnServerDisconnected;

		// Клиентские события (ДОБАВЛЯЕМ ИХ)
		public event System.Action OnClientConnected;
		public event System.Action OnClientDisconnected;

		[Inject] private DiContainer _container;

		[SerializeField] private GameObject _gameplayPlayerPrefab;
		[SerializeField] private string _gameSceneName = "GameScene";

		// Индекс следующего игрока в GameScene. Поле инстанса, не статик — иначе не разъедутся
		// значения между отдельными запусками/клонами редактора (ParrelSync) и между сценами.
		private int _nextPlayerIndex;

		public override void OnServerConnect(NetworkConnectionToClient conn)
		{
			base.OnServerConnect(conn);
			OnServerConnected?.Invoke();
		}

		public override void OnServerDisconnect(NetworkConnectionToClient conn)
		{
			base.OnServerDisconnect(conn);
			OnServerDisconnected?.Invoke();
		}

		// Mirror зовёт этот метод и при обычном коннекте, и САМ ПО СЕБЕ сразу после ServerChangeScene:
		// смена сцены убивает старый объект игрока (это обычная выгрузка сцены Unity), клиент видит
		// localPlayer == null и просит нового игрока. Поэтому здесь решаем по текущей сетевой сцене,
		// какой префаб поднимать: в Lobby — обычный playerPrefab (LobbyPlayerNetwork, логика не меняется),
		// в GameScene — gameplayPlayerPrefab (SaberPlayerNetwork).
		public override void OnServerAddPlayer(NetworkConnectionToClient conn)
		{
			if (networkSceneName != _gameSceneName)
			{
				base.OnServerAddPlayer(conn);
				return;
			}

			// Хост — это ПК-сервер, он не держит в руке телефон и не становится игровым объектом в GameScene.
			if (conn == NetworkServer.localConnection)
				return;

			GameObject player = Instantiate(_gameplayPlayerPrefab);
			player.name = $"{_gameplayPlayerPrefab.name} [connId={conn.connectionId}]";

			// Mirror создаёт объект через обычный Instantiate, Zenject о нём ничего не знает —
			// без этой строки все [Inject]-поля на игроке останутся пустыми.
			_container.InjectGameObject(player);

			if (player.TryGetComponent(out SaberPlayerNetwork saberPlayer))
				saberPlayer.ServerSetIndex(_nextPlayerIndex++);

			NetworkServer.AddPlayerForConnection(conn, player);
		}

		// Вызывается на стороне КЛИЕНТА при успешном подсоединении к серверу
		public override void OnClientConnect()
		{
			base.OnClientConnect();
			OnClientConnected?.Invoke();
		}

		// Вызывается на стороне КЛИЕНТА при отключении от сервера
		public override void OnClientDisconnect()
		{
			base.OnClientDisconnect();
			OnClientDisconnected?.Invoke();
		}
	}
}