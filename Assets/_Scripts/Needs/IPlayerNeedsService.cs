namespace Assets.Scripts.Needs
{
    /// <summary>
    /// Состояние выживания игрока: голод, сон, стамина.
    /// Регистрируется в ServiceLocator (см. GameEntryPoint), поэтому переживает
    /// переход между сценами (Level1 → house → Level1 и т.д.).
    /// </summary>
    public interface IPlayerNeedsService
    {
        float Stamina { get; }
        float MaxStamina { get; }
        float Hunger { get; }
        float MaxHunger { get; }
        float Sleep { get; }
        float MaxSleep { get; }

        int MinDamageLevel { get; }
        int MinCarryLevel { get; }
        int MinSpeedLevel { get; }
        int MaxUpgradeLevel { get; }

        /// <summary>Вызывать каждый кадр (см. PlayerNeedsTicker).</summary>
        void Tick(float deltaTime);

        /// <summary>Восстановить голод (еда).</summary>
        void Eat(float amount);

        /// <summary>Отдых дома — единственный способ восстановить стамину (и заодно сон).</summary>
        void RestAtHome();

        /// <summary>Прямой расход стамины от произвольных причин ("прочие приколы": холод, ранение и т.п.).</summary>
        void ApplyFatigue(float amount);

        float GetSpeedMultiplier();
        float GetDamageMultiplier();
        float GetCarryMultiplier();

        /// <summary>Хватает ли стамины на навык ловкости.</summary>
        bool CanUseAgilitySkill();

        void UpgradeMinDamage();
        void UpgradeMinCarry();
        void UpgradeMinSpeed();
    }
}
