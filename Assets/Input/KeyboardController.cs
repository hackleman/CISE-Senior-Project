using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class KeyboardController : InputController
    {
        // Events
        public static event MoveInputHandler onMoveInput;
        public static event RotateInputHandler onRotateInput;

        public delegate void EscapeInputHandler();
        public static event EscapeInputHandler onEscapeInput;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                onMoveInput?.Invoke(Vector3.up);
            }
            if (Input.GetKey(KeyCode.S))
            {
                onMoveInput?.Invoke(-Vector3.up);
            }
            if (Input.GetKey(KeyCode.A))
            {
                onMoveInput?.Invoke(-Vector3.right);
            }
            if (Input.GetKey(KeyCode.D))
            {
                onMoveInput?.Invoke(Vector3.right);
            }

            if (Input.GetKey(KeyCode.E))
            {
                onRotateInput?.Invoke(-1f);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                onRotateInput?.Invoke(1f);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                onMoveInput?.Invoke(Vector3.forward);
            }
            if (Input.GetKey(KeyCode.X))
            {
                onMoveInput?.Invoke(-Vector3.forward);
            }

            if(Input.GetKey(KeyCode.Escape))
            {
                onEscapeInput?.Invoke();
            }

        }
    }
