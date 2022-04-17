using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class HandController : InputController
{
    private ActionBasedController controller;
    // Events
    public delegate void ActivateInputHandler();
    public static event ActivateInputHandler onActivateInput;

    // Update is called once per frame
    void Update()
    {

    }
}
