using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : InputController
{
    public delegate void ClickInputHandler();
    public static event ClickInputHandler onClickInput;


    // Update is called once per frame
    void Update()
    {
        Debug.Log("is this thing working?");
        
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("clicked");
            onClickInput?.Invoke();
        }

    }
}
