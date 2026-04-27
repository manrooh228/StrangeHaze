namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Контракт навигации между сценами.
    /// Используй ServiceLocator.Get&lt;IGameSceneManager&gt;() из любого скрипта.
    /// </summary>
    public interface IGameSceneManager
    {
        /// <summary>
        /// Перейти на указанную сцену через Loading экран.
        /// Пример: GoTo(SceneNames.Level1)
        /// </summary>
        void GoTo(string sceneName);

        /// <summary>
        /// Имя сцены, которую нужно загрузить после Loading.
        /// Читается из LoadingEntryPoint.
        /// </summary>
        string NextScene { get; }
    }
}
