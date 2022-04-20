using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

    public class KeyboardController : InputController
    {
        // Events
        public delegate void EscapeInputHandler();
        public static event EscapeInputHandler onEscapeInput;

        public delegate void VisibilityInputHandler();
        public static event VisibilityInputHandler onVisibilityInput;

        // Update is called once per frame
        void Update()
        {
            if(Keyboard.current.escapeKey.isPressed)
            {
                onEscapeInput?.Invoke();
            }

            if(Keyboard.current.leftCtrlKey.isPressed)
            {
                onVisibilityInput?.Invoke();
            }

        }
    }
