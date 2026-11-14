Код стайл (рабочий) + правки проекта Lightsaber

Правки проекта помечены словом «ПРАВКА ПРОЕКТА». При конфликте этот файл главнее остальных правил.

Источник: код стайл компании (автор — техлид). Применяется на работе, в учебных проектах и в своих играх. При конфликте с любыми другими правилами (включая 03_naming.md) побеждает этот файл.

Наименование скриптов
Скрипты, управляющие несколькими однотипными объектами визуала: суффикс View / Window / Popup → ElevatorView.cs
Обработчики кнопок: содержат Buttons → TopLevelButtons.cs
Обработчики ввода: содержат Input → MainMenuInput.cs
Суффикс Helper для скриптов одного объекта — устарело, не используем.
Архитектура
Zenject — синглтоны мета-логики
Логика живёт в обычном C#-классе (не MonoBehaviour) с суффиксом Manager: PlayerManager, SkinManager.
Биндится в контекст:
SceneContext — менеджеры в рамках одной сцены (WindowManager);
ProjectContext — менеджеры на весь проект (EventManager).
ProjectContext инициализируется раньше SceneContext.
Менеджер прячется за интерфейсом, инжектится только через интерфейс.
Интерфейсы делятся на чтение и запись: IPlayerManager : IPlayerManagerReadOnly.
csharp
public interface IPlayerManagerReadOnly
{
    // Get-методы, свойства, события
}

public interface IPlayerManager : IPlayerManagerReadOnly
{
    // Set-методы
}

// где нужно только читать:
[Inject] private IPlayerManagerReadOnly _playerManager;

// где нужно менять:
[Inject] private IPlayerManager _playerManager;
AbilitySystem — игровая логика

Форки: UnitFramework, Lifetime, TimeManagement (опенсорс, форки, чтобы свободно править).

Character : Unit вешается на верхний объект игровой сущности.
Ниже по иерархии — пустой GameObject Abilities.
Внутри Abilities на каждую абилку свой GameObject со скриптом. Имя абилки обязательно с суффиксом Ability: HealthAbility.
Область видимости и namespace
Модификатор доступа указываем всегда (private тоже пишем явно).
Namespace — ПРАВКА ПРОЕКТА Lightsaber (отличается от рабочего стайла): используем и для своего кода. Строятся по логике (модулю), а не по папкам: Lightsaber.<Модуль>, максимум два уровня (Lightsaber.Network, Lightsaber.Network.UI). Файл может лежать в любой папке. Список модулей ведём в CLAUDE.md, новый модуль — по согласованию.
Naming convention
Что	Стиль	Пример
Приватные поля	_lowerCamelCase	private int _attack; private bool _isSpell;
Поля для инспектора	[SerializeField] private, lowerCamelCase без _	[SerializeField] private GameObject coinsRewardObject;
Публичные свойства	PascalCase, { get; private set; }	public bool IsSpell { get; private set; }
Методы (любые)	PascalCase	public void SomeMethod(), private void SomePrivateMethod()
Классы, enum, struct	PascalCase	public class Settings
Параметры	lowerCamelCase	SomeMethod(int someArg, float secondArg)
Константы	UPPER_SNAKE_CASE	private const int MAX_SLOTS = 10;
Публичных полей избегаем: вместо них свойства с private set. Исключение — сериализуемые подклассы-данные.
Поля для инспектора не делаем публичными, только [SerializeField].
Скобки

Фигурные скобки на новой строке, выровнены по вертикали, содержимое с одним отступом (стиль Allman).

csharp
public class Foo : Boo
{
    public void Bar()
    {
        // ...
    }
}
События
Глобальный EventManager с классами-событиями — DEPRECATED, в новом коде не используем (в старом коде встретится: классы XxxEvent, обработчики XxxHandler).
Сейчас: событие объявляется в самом менеджере и в его ReadOnly-интерфейсе:
csharp
public event Action<T> NameAction;

[Inject] private ISomeManagerReadOnly _someManager;
// ...
_someManager.NameAction += NameHandle;
Порядок объявления в классе

Члены:

публичные свойства и события (event);
зависимости ([Inject]);
поля для инспектора ([SerializeField]; публичные поля для инспектора — избегаем, но если есть, то здесь);
приватные поля.
csharp
public event Action OnPlayerSpawn;
public bool IsPlayerSpawned { get; private set; }
[field: SerializeField] public float Speed { get; private set; }

[Inject] private IPlayerManager _playerManager;

[SerializeField] private Character playerPrefab;

private float _currentHealth;

Методы:

Unity-методы и методы реализуемых интерфейсов (Initialize, Tick и т.п.);
публичные методы;
реакции на события (хендлеры);
приватные методы;
корутины.
Атрибуты
Дополнительные и нестандартные атрибуты пишем строкой выше.
[SerializeField] и [Inject] пишем в одну строку с полем.
[field: SerializeField] только на свойствах: короткое свойство в одну строку, полное — атрибут сверху.
Settings и SaveData
Настройки Zenject-менеджера — вложенный класс Settings в отдельном файле через partial → PlayerManager.Settings.
Сохранения — вложенный класс SaveData в отдельном файле через partial → PlayerManager.SaveData.
csharp
public partial class PlayerManager
{
    [Serializable]
    public class Settings
    {
    }
}
Enums
Используется только внутри класса → вложенный enum. Используется снаружи → в том же файле отдельно. Широко используется → отдельный файл, выше по иерархии папок, над зависимыми модулями.
Значения обязательно пронумерованы. Первое значение — None (null/nan/none). Исключение: ровно два значения (Open/Close, Active/NotActive).
csharp
public enum ItemType
{
    None = 0,
    Weapon = 1,
    Armor = 2,
}
Часто вместо enum лучше ScriptableObject: гибче.