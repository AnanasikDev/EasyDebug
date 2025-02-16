using UnityEngine;
using EasyDebug.CommandLine;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
    }

    [Command("command_uno", ConsoleCommandType.Global)]
    public void MyCustomCommand1()
    {
        Debug.Log("Heh that's my first custom runtime console command:D");
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

    [Command("hello", ConsoleCommandType.Global)]
    public void MyCustomCommand2()
    {
        Debug.Log("Hullooo");
    }

    [Command("ohohoh", ConsoleCommandType.Global)]
    public void MyCustomCommand3()
    {
        Debug.Log("3 ohoh");
    }
}

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
