using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Service
{
    public class InputHandler : MonoBehaviour
    {
        private IInput _inputSource;

        public void Start()
        {
            _inputSource = new DesktopInput();//here we set device
        }

        public float GetHorizontal() => _inputSource.Horizontal;
        public float GetVertical() => _inputSource.Vertical;
        public bool GetShootPressed() => _inputSource.ShootPressed;

        public void ChangeBinding(string action, KeyCode key)
        {
            _inputSource.RemapKey(action, key);
        }
    }
}
