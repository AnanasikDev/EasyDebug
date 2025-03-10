﻿using UnityEngine;
using EasyDebug.CommandLine;
using System.Collections.Generic;
using System;
using EasyDebug.Prompts;
using EasyDebug;

public class MainExample : MonoBehaviour
{
    private A a = new A(1, "ohohoh", 23);
    private B b = new B();

    public Dictionary<int, string> mydict = new Dictionary<int, string>()
    {
        { 1, "hello" },
        { 2, "hola" },
        { 3, "привет" },
    };

    public A[] aas = new A[4]
    {
        new A(1, "furries", 9.123f),
        new A(0, "are", -9.0f),
        new A(-1231231, "best", 3.3333333f),
        new A(222, "!!!", 0)
    };

    public Color color = new Color(0.4f, 1.0f, 0.9f, 1.0f);

    private void Start()
    {
        var v = aas[0];
    }

    bool state = true;

    private void Update()
    {
        RuntimeGizmos.DrawArrow(transform.position, Vector3.up);

        if (Input.GetKeyDown(KeyCode.Q)) state = !state;

        if (state)
        {
            PromptManager.UpdateArrowPrompt(gameObject, "fwd", transform.forward, transform.position, Color.blue);
            PromptManager.UpdateArrowPrompt(gameObject, "up", transform.up, transform.position, Color.green);
            PromptManager.UpdateArrowPrompt(gameObject, "right", transform.right, transform.position, Color.red);
            PromptManager.UpdateSpherePrompt(gameObject, "sphere", Vector3.zero, 0.5f, Color.red);
            PromptManager.UpdateBoxPrompt(gameObject, "box1", transform.position / 2.0f, 1, Color.white);
        }

        RuntimeGizmos.DrawCircle(transform.position, transform.forward, transform.right, 3, Color.white);
    }

    [Command("funwithargs", ConsoleCommandType.Global)]
    public void MyCustomCommand1(float arg1, int arg2 = 0)
    {
        Debug.Log($"ARGUMENTED FUNCTION: float arg1 = {arg1}; int arg2 = {arg2}");
    }

    [Command("translate", ConsoleCommandType.ObjectRelative)]
    public void Translate(Vector3 delta)
    {
        transform.position += delta;
    }

    [Command("command_2", ConsoleCommandType.Global)]
    public void MyCustomCommand12()
    {
        Debug.Log("Heh that's 2:D");
    }

    [Command("command_gfgdf", ConsoleCommandType.Global)]
    public void MyCustomCommand13()
    {
        Debug.Log("Heh that's 3:D");
    }

    [Command("hello", ConsoleCommandType.ObjectRelative, "manager")]
    public void MyCustomCommand2()
    {
        Debug.Log("Hullooo");
    }

    [Command("ohohoh", ConsoleCommandType.ObjectRelative, "manager")]
    public void MyCustomCommand3()
    {
        Debug.Log("3 ohoh");
    }

    [Command("a", ConsoleCommandType.ObjectRelative)]
    public void _1()
    {
        Debug.Log("1");
    }

    [Command("ab", ConsoleCommandType.ObjectRelative)]
    public void _2()
    {
        Debug.Log("2");
    }

    [Command("abc", ConsoleCommandType.ObjectRelative)]
    public void _3()
    {
        Debug.Log("3");
    }

    [Command("abcd", ConsoleCommandType.ObjectRelative)]
    public void _4()
    {
        Debug.Log("4");
    }

    [Command("abcde", ConsoleCommandType.ObjectRelative)]
    public void _5()
    {
        Debug.Log("5");
    }

    [Command("abcdf", ConsoleCommandType.ObjectRelative)]
    public void _6()
    {
        Debug.Log("6");
    }

    [Command("abcdfg", ConsoleCommandType.ObjectRelative)]
    public void _7()
    {
        Debug.Log("7");
    }

    [Command("abcdfgf", ConsoleCommandType.ObjectRelative)]
    public void _8()
    {
        Debug.Log("7");
    }

    [Command("abcdfgfd", ConsoleCommandType.ObjectRelative)]
    public void _9()
    {
        Debug.Log("7");
    }

    [Command("abcdfgfd2", ConsoleCommandType.ObjectRelative)]
    public void _10()
    {
        Debug.Log("7");
    }
}

[Serializable]
public class A
{
    public int myint;
    public string mystring;
    public float myfloat;

    public A(int _int, string _string, float _float)
    {
        myint = _int;
        mystring = _string;
        myfloat = _float;
    }
}

[Serializable]
public class B
{
    public A a1 = new A(42, "heh", 2);
    public A a2 = new A(2, "heh", 24);
}

public static class Hehe
{
    public static B b = new B();
    public static string heh = "1231LSKFAHJF;LAKDSJFHA;SJKDHFASKJDHFKLJHS";
    private static A a { get; set; } = new A(1, "e", 2);
    private static double pi = 3.09123019;
}