using UnityEngine;
using EasyDebug.PipeConsole;
using EasyDebug.RuntimeConsole;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
    }

    [ConsoleCommand("command_uno", ConsoleCommandType.Global)]
    public void MyCustomCommand1()
    {
        Debug.Log("Heh that's my first custom runtime console command:D");
    }
}

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
