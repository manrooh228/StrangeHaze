using System;
using System.Collections.Generic;
using UnityEngine;

namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Глобальный контейнер сервисов (Service Locator).
    ///
    /// Использование:
    ///   // Регистрация (обычно в GameEntryPoint):
    ///   ServiceLocator.Register<ISceneLoader>(new SceneLoader());
    ///
    ///   // Получение из любого места:
    ///   var loader = ServiceLocator.Get<ISceneLoader>();
    ///
    /// Как масштабировать:
    ///   - Добавьте сфокусированные контейнеры (SceneServiceLocator) для сервисов уровня сцены.
    ///   - Замените на DI-фреймворк (Zenject / VContainer) когда зависимостей станет много.
    ///   - Добавьте TryGet<T>(out T service) для опциональных зависимостей.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        /// <summary>Зарегистрировать сервис. Повторная регистрация перезаписывает предыдущий.</summary>
        public static void Register<T>(T service) where T : class
        {
            _services[typeof(T)] = service;
            Debug.Log($"[ServiceLocator] Registered: {typeof(T).Name}");
        }

        /// <summary>Получить сервис. Бросает исключение если не зарегистрирован.</summary>
        public static T Get<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out object service))
                return (T)service;

            throw new InvalidOperationException(    
                $"[ServiceLocator] Service '{typeof(T).Name}' not registered. " +
                $"Зарегистрируй его в GameEntryPoint.");
        }

        /// <summary>Сбросить все сервисы (например, при выходе из игры или в тестах).</summary>
        public static void Clear()
        {
            _services.Clear();
            Debug.Log("[ServiceLocator] Cleared all services.");
        }
    }
}
