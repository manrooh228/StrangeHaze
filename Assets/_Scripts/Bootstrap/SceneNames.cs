namespace StrangeHaze.Bootstrap
{
    /// <summary>
    /// Единственное место, где хранятся имена всех сцен.
    /// Меняй только здесь — всё остальное подтянется автоматически.
    ///
    /// ВАЖНО: имя константы должно точно совпадать с именем .unity файла.
    ///
    /// Как добавить новую сцену:
    ///   1. Создай .unity файл (например, "Level3.unity")
    ///   2. Добавь её в Build Settings
    ///   3. Добавь константу ниже: public const string Level3 = "Level3";
    /// </summary>
    public static class SceneNames
    {
        public const string Bootstrap  = "Bootstrap";
        public const string Loading    = "LoadingScene";   // ← поменяй на своё имя
        public const string MainMenu   = "MainMenu";       // ← поменяй на своё имя

        // ── Уровни ────────────────────────────────────────────────────────────
        public const string Level1     = "Level1";
        public const string Level2     = "Level2";

        // ── Особые сцены ──────────────────────────────────────────────────────
        public const string Shop       = "Shop";

        // Добавляй новые сцены сюда:
        // public const string Cutscene = "Cutscene";
        // public const string Boss     = "BossArena";
    }
}
