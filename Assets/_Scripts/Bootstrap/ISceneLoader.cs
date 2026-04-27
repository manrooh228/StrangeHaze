using System;
using System.Collections;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Контракт для загрузки сцен.
    /// Позволяет легко подменить реализацию (например, на Addressables) без изменения кода выше.
    /// </summary>
    public interface ISceneLoader
    {
        /// <summary>Загрузить сцену по имени. onLoaded вызывается когда сцена полностью готова.</summary>
        IEnumerator LoadSceneAsync(string sceneName, Action onLoaded = null);

        /// <summary>Загрузить сцену по индексу (из Build Settings).</summary>
        IEnumerator LoadSceneAsync(int sceneIndex, Action onLoaded = null);
    }
}
