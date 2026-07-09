using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Service
{
    public class DesktopInput : IInput
    {
        private Dictionary<string, KeyCode> _keys = new Dictionary<string, KeyCode>()
        {
            { "Shoot", KeyCode.Mouse0 },
            { "Open Inventory" , KeyCode.I },
            { "Reload", KeyCode.R },
            { "SwitchWeapon", KeyCode.Q }
        };

        public float Horizontal => Input.GetAxisRaw("Horizontal");
        public float Vertical => Input.GetAxisRaw("Vertical");
        public bool ShootPressed => Input.GetKeyDown(_keys["Shoot"]);

        public bool ReloadPressed => Input.GetKeyDown(_keys["Reload"]);
        public bool InventoryPressed => Input.GetKeyDown(_keys["Open Inventory"]);
        public bool WeaponSwitchPressed => Input.GetKeyDown(_keys["SwitchWeapon"]);

        public void RemapKey(string actionName, KeyCode newKey)
        {
            if (_keys.ContainsKey(actionName))
                _keys[actionName] = newKey;
        }
    }
}
