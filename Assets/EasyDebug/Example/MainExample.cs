using UnityEngine;
using EasyDebug.CommandLine;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
    }

    [Command("funwithargs", ConsoleCommandType.Global)]
    public void MyCustomCommand1(float arg1, int arg2 = 0)
    {
        Debug.Log($"ARGUMENTED FUNCTION: float arg1 = {arg1}; int arg2 = {arg2}");
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

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
