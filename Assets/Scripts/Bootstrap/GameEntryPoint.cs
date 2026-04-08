using UnityEngine;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Главный оркестратор запуска игры.
    /// Вызывается один раз из GameBootstrapper при старте Bootstrap сцены.
    ///
    /// Порядок инициализации:
    ///   1. Регистрация глобальных сервисов (ServiceLocator)
    ///   2. Применение глобальных настроек
    ///   3. Первый переход → Loading → MainMenu
    ///
    /// Как масштабировать:
    ///   - Добавляй новые сервисы в RegisterServices().
    ///   - Для асинхронной инициализации (авторизация, SDK) используй async/await перед GoTo.
    ///   - Чтобы менять стартовую сцену — измени вызов GoTo в Run().
    /// </summary>
    public class GameEntryPoint
    {
        public GameEntryPoint() { }

        /// <summary>Точка входа. Вызывается из GameBootstrapper.Awake().</summary>
        public void Run()
        {
            Debug.Log("[GameEntryPoint] Starting...");

            RegisterServices();
            ApplySettings();

            // Первый запуск всегда идёт в главное меню через Loading экран.
            // Поменяй SceneNames.MainMenu на что угодно, если нужно другое поведение.
            ServiceLocator.Get<IGameSceneManager>().GoTo(SceneNames.MainMenu);
        }

        // ── Регистрация глобальных сервисов ───────────────────────────────────
        private void RegisterServices()
        {
            // Навигация — зарегистрируй первым, остальные могут зависеть от него
            ServiceLocator.Register<IGameSceneManager>(new GameSceneManager());

            // Загрузка сцен — используется внутри GameSceneManager и LoadingEntryPoint
            ServiceLocator.Register<ISceneLoader>(new SceneLoader());

            // Добавляй новые глобальные сервисы здесь:
            // ServiceLocator.Register<IAudioService>(new AudioService());
            // ServiceLocator.Register<ISaveService>(new SaveService());
        }

        // ── Глобальные настройки приложения ───────────────────────────────────
        private void ApplySettings()
        {
            Application.targetFrameRate = 60;

            // Загружай настройки пользователя здесь:
            // QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 2));
            // AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
    }
}
