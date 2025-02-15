﻿using UnityEngine;
using EasyDebug.CommandLine;

public class ConsoleCommandTest : MonoBehaviour
{
    [Command("testFunc", ConsoleCommandType.ObjectRelative)]
    private void myTestFunction()
    {
        Debug.Log("myTestFunction has been called on me! " + gameObject.name);
    }
}