using UnityEngine;
using EasyDebug.CommandLine;
using System.Collections.Generic;
using System;
using EasyDebug.Prompts;

public class MainExample : MonoBehaviour
{
    private A a = new A(1, "ohohoh", 23);

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

    private void Update()
    {
        DrawArrow.ForDebug(transform.position, Vector3.up);

        PromptManager.UpdateArrowPrompt(gameObject, "fwd", transform.forward, Color.blue);
        PromptManager.UpdateArrowPrompt(gameObject, "up", transform.up, Color.green);
        PromptManager.UpdateArrowPrompt(gameObject, "right", transform.right, Color.red);
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
