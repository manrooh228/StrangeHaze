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
            { "Open Inventory" , KeyCode.Mouse1 }
        };

        public float Horizontal => Input.GetAxisRaw("Horizontal");
        public float Vertical => Input.GetAxisRaw("Vertical");
        public bool ShootPressed => Input.GetKeyDown(_keys["Shoot"]);

        public void RemapKey(string actionName, KeyCode newKey)
        {
            if (_keys.ContainsKey(actionName))
                _keys[actionName] = newKey;
        }
    }
}
