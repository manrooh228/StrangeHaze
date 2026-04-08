using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Стандартная реализация загрузки сцен через Unity SceneManager.
    ///
    /// Как масштабировать:
    ///   - Замените AsyncOperation на Addressables.LoadSceneAsync для Asset Bundle сцен.
    ///   - Добавьте событие OnLoadProgress(float) чтобы дёргать прогресс-бар на Loading сцене.
    ///   - Добавьте очередь сцен (LoadSceneQueue) для последовательной загрузки нескольких сцен.
    /// </summary>
    public class SceneLoader : ISceneLoader
    {
        public IEnumerator LoadSceneAsync(string sceneName, Action onLoaded = null)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            yield return op;
            onLoaded?.Invoke();
        }

        public IEnumerator LoadSceneAsync(int sceneIndex, Action onLoaded = null)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
            yield return op;
            onLoaded?.Invoke();
        }
    }
}
