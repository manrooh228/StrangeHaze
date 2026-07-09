using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Service
{
    public interface IInput
    {
        float Horizontal { get; }
        float Vertical { get; }
        bool ShootPressed { get; }
        bool ReloadPressed { get; }
        bool InventoryPressed { get; }
        bool WeaponSwitchPressed { get; }

        void RemapKey(string key, KeyCode newKey);
    }
}
