using UnityEngine;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// MonoBehaviour-оболочка для GameEntryPoint.
    /// Размести этот компонент на GameObject "Bootstrapper" в Bootstrap сцене.
    ///
    /// Выполняется РАНЬШЕ всех других MonoBehaviour ([DefaultExecutionOrder(-1000)]).
    /// Bootstrap должна быть первой сценой в Build Settings (индекс 0).
    ///
    /// Как настроить:
    ///   1. File → New Scene (пустая) → сохрани как "Bootstrap"
    ///   2. Создай пустой GameObject → назови "Bootstrapper"
    ///   3. Добавь этот компонент на него
    ///   4. Build Settings → перетащи Bootstrap.unity на индекс 0
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            // Объект живёт во всех сценах — нужен чтобы ServiceLocator не умер при переходе.
            DontDestroyOnLoad(gameObject);

            new GameEntryPoint().Run();
        }

        private void OnDestroy()
        {
            // Очищаем сервисы при выходе из игры (нормально срабатывает только в редакторе).
            ServiceLocator.Clear();
        }
    }
}
