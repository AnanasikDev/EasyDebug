using UnityEngine;
using EasyDebug.RuntimeConsole;

public class ConsoleCommandTest : MonoBehaviour
{
    [ConsoleCommand("testFunc", ConsoleCommandType.ObjectRelative)]
    private void myTestFunction()
    {
        Debug.Log("myTestFunction has been called on me! " + gameObject.name);
    }
}