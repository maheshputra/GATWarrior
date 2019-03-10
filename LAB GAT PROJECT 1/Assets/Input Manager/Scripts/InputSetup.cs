﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSetup : MonoBehaviour
{
    public KeyCode select;
    public KeyCode back;
    public KeyCode deleteSave;
    public KeyCode interact;
    public KeyCode continueTalk;
    // Start is called before the first frame update

    void Start()
    {
        select = KeyCode.Joystick1Button0;
        back = KeyCode.Joystick1Button1;
        deleteSave = KeyCode.Joystick1Button2;

        interact = KeyCode.Joystick1Button1;
        continueTalk = KeyCode.Joystick1Button1;
    }
}