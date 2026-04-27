using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Точка входа Loading сцены.
    /// Размести этот компонент на GameObject в Loading сцене.
    ///
    /// Что делает:
    ///   1. Читает из GameSceneManager — куда нужно идти
    ///   2. Параллельно запускает таймер минимального времени и асинхронную загрузку сцены
    ///   3. Переходит только когда выполнены оба условия (сцена готова И прошло минимальное время)
    ///
    /// Как добавить UI прогресса:
    ///   - Назначь _progressBar (Slider или Image с fillAmount) через Inspector
    ///   - Используй _loadProgress (0..1) для обновления своего UI в Update()
    /// </summary>
    public class LoadingEntryPoint : MonoBehaviour
    {
        [SerializeField] private float _minDisplayTime = 2f; // минимальное время на Loading экране

        // Текущий прогресс загрузки (0..1) — используй для UI прогресс-бара
        public float LoadProgress { get; private set; }

        private void Start()
        {
            IGameSceneManager sceneManager = ServiceLocator.Get<IGameSceneManager>();

            string target = sceneManager.NextScene;

            if (string.IsNullOrEmpty(target))
            {
                // Запасной вариант — если попали на Loading напрямую, идём в главное меню
                Debug.LogWarning("[LoadingEntryPoint] NextScene не задан, переход в MainMenu.");
                target = SceneNames.MainMenu;
            }

            StartCoroutine(LoadWithMinTime(target));
        }

        private IEnumerator LoadWithMinTime(string target)
        {
            Debug.Log($"[LoadingEntryPoint] Загружаю: {target}");

            // Запускаем загрузку сцены, но не активируем её сразу
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(target);
            loadOp.allowSceneActivation = false;

            float elapsed = 0f;

            // Ждём пока не выполнятся оба условия:
            //   - прошло минимальное время (_minDisplayTime)
            //   - сцена загружена (progress достигает 0.9 — Unity-специфика)
            while (elapsed < _minDisplayTime || loadOp.progress < 0.9f)
            {
                elapsed += Time.deltaTime;

                // Прогресс: берём максимум из двух условий чтобы бар не прыгал назад
                float timeProgress  = Mathf.Clamp01(elapsed / _minDisplayTime);
                float sceneProgress = Mathf.Clamp01(loadOp.progress / 0.9f);
                LoadProgress = Mathf.Min(timeProgress, sceneProgress);

                yield return null;
            }

            LoadProgress = 1f;

            // Оба условия выполнены — активируем сцену
            loadOp.allowSceneActivation = true;
        }
    }
}
