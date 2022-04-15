using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    public delegate void MoveInputHandler(Vector3 movement);
    public delegate void RotateInputHandler(float rotation);
}
