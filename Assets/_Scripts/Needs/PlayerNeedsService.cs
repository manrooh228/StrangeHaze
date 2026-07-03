using UnityEngine;

namespace Assets.Scripts.Needs
{
    /// <summary>
    /// Реализация выживания игрока.
    ///
    /// Правила:
    ///   - Голод и сон убывают со временем сами по себе.
    ///   - Если голод или сон падают ниже порога — стамина начинает утекать.
    ///   - Стамина НИКОГДА не восстанавливается сама по себе — только через RestAtHome().
    ///   - От текущей стамины (0..1) зависят множители скорости/урона/переносимого лута:
    ///     на 100% стамины множитель = 1, на 0% стамины множитель = "пол" (floor),
    ///     который можно прокачать (Upgrade*), тем самым подняв минимально возможные значения.
    /// </summary>
    public class PlayerNeedsService : IPlayerNeedsService
    {
        // ── Стамина ────────────────────────────────────────────────────────
        public float Stamina { get; private set; }
        public float MaxStamina { get; } = 100f;

        // ── Голод ──────────────────────────────────────────────────────────
        public float Hunger { get; private set; }
        public float MaxHunger { get; } = 100f;
        private const float HungerDecayPerSecond = 100f / 900f; // полный голод за ~15 минут
        private const float LowHungerThreshold = 30f;

        // ── Сон ────────────────────────────────────────────────────────────
        public float Sleep { get; private set; }
        public float MaxSleep { get; } = 100f;
        private const float SleepDecayPerSecond = 100f / 1200f; // полная усталость за ~20 минут
        private const float LowSleepThreshold = 30f;

        // ── Расход стамины при низком голоде/сне ──────────────────────────
        private const float StaminaDrainFromHunger = 3f; // ед/сек, пока голод <= порога
        private const float StaminaDrainFromSleep = 3f;  // ед/сек, пока сон <= порога

        // ── Порог стамины для навыка ловкости ─────────────────────────────
        private const float AgilityStaminaThresholdPercent = 20f;

        // ── Прокачиваемые "полы" (минимум множителя при 0 стамины) ────────
        public int MinDamageLevel { get; private set; }
        public int MinCarryLevel { get; private set; }
        public int MinSpeedLevel { get; private set; }
        public int MaxUpgradeLevel { get; } = 5;

        private const float FloorStepPerLevel = 0.1f; // каждый уровень прокачки = +10% к полу

        private const float BaseFloorDamage = 0.3f; // без прокачки: 30% урона на нуле стамины
        private const float BaseFloorCarry = 0.3f;  // 30% переносимого лута
        private const float BaseFloorSpeed = 0.4f;  // 40% скорости

        public PlayerNeedsService()
        {
            Stamina = MaxStamina;
            Hunger = MaxHunger;
            Sleep = MaxSleep;
        }

        public void Tick(float deltaTime)
        {
            Hunger = Mathf.Clamp(Hunger - HungerDecayPerSecond * deltaTime, 0f, MaxHunger);
            Sleep = Mathf.Clamp(Sleep - SleepDecayPerSecond * deltaTime, 0f, MaxSleep);

            float drainPerSecond = 0f;
            if (Hunger <= LowHungerThreshold) drainPerSecond += StaminaDrainFromHunger;
            if (Sleep <= LowSleepThreshold) drainPerSecond += StaminaDrainFromSleep;

            if (drainPerSecond > 0f)
                Stamina = Mathf.Clamp(Stamina - drainPerSecond * deltaTime, 0f, MaxStamina);

            // Стамина сама по себе не восстанавливается — только RestAtHome().
        }

        public void Eat(float amount)
        {
            Hunger = Mathf.Clamp(Hunger + amount, 0f, MaxHunger);
        }

        public void RestAtHome()
        {
            Stamina = MaxStamina;
            Sleep = MaxSleep;
            Debug.Log("[PlayerNeedsService] Отдых дома: стамина и сон восстановлены.");
        }

        public void ApplyFatigue(float amount)
        {
            Stamina = Mathf.Clamp(Stamina - amount, 0f, MaxStamina);
        }

        public float GetSpeedMultiplier() => ComputeMultiplier(BaseFloorSpeed, MinSpeedLevel);
        public float GetDamageMultiplier() => ComputeMultiplier(BaseFloorDamage, MinDamageLevel);
        public float GetCarryMultiplier() => ComputeMultiplier(BaseFloorCarry, MinCarryLevel);

        public bool CanUseAgilitySkill()
        {
            float staminaPercent = MaxStamina > 0f ? Stamina / MaxStamina * 100f : 0f;
            return staminaPercent >= AgilityStaminaThresholdPercent;
        }

        public void UpgradeMinDamage()
        {
            if (MinDamageLevel < MaxUpgradeLevel) MinDamageLevel++;
        }

        public void UpgradeMinCarry()
        {
            if (MinCarryLevel < MaxUpgradeLevel) MinCarryLevel++;
        }

        public void UpgradeMinSpeed()
        {
            if (MinSpeedLevel < MaxUpgradeLevel) MinSpeedLevel++;
        }

        private float ComputeMultiplier(float baseFloor, int level)
        {
            float floor = Mathf.Clamp01(baseFloor + level * FloorStepPerLevel);
            float staminaPercent = MaxStamina > 0f ? Stamina / MaxStamina : 1f;
            return Mathf.Lerp(floor, 1f, staminaPercent);
        }
    }
}
