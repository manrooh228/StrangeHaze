using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Центральная точка навигации между сценами.
    /// Все переходы идут через Loading сцену, которая плавно подгружает нужное.
    ///
    /// Примеры использования из любого скрипта:
    ///   var nav = ServiceLocator.Get&lt;IGameSceneManager&gt;();
    ///   nav.GoTo(SceneNames.Level1);    // перейти на уровень 1
    ///   nav.GoTo(SceneNames.MainMenu);  // вернуться в главное меню
    ///   nav.GoTo(SceneNames.Shop);      // открыть магазин
    ///
    /// Как масштабировать:
    ///   - Добавь стек сцен (Stack&lt;string&gt; _history) чтобы делать GoBack().
    ///   - Добавь передачу параметров (object payload) для передачи данных между сценами.
    ///   - Добавь событие OnSceneChanged для реакции UI.
    /// </summary>
    public class GameSceneManager : IGameSceneManager
    {
        public string NextScene { get; private set; }

        /// <summary>
        /// Запросить переход на targetScene через Loading экран.
        /// Загрузка Loading сцены синхронная (она маленькая),
        /// а уже внутри неё происходит асинхронная загрузка целевой сцены.
        /// </summary>
        public void GoTo(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[GameSceneManager] Имя сцены пустое!");
                return;
            }

            Debug.Log($"[GameSceneManager] GoTo: {sceneName}");
            NextScene = sceneName;

            // Переходим на Loading сцену — она сама загрузит NextScene
            SceneManager.LoadScene(SceneNames.Loading);
        }
    }
}
