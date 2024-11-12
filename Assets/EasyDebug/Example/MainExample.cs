using UnityEngine;
using EasyDebug.PipeConsole;
using EasyDebug.RuntimeConsole;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
        PipeConsole.SetFloatDivider();
        //QDebug.tagsAllowed = Tag.Debug | Tag.Info;
        PipeConsole.defaultParser = PipeConsole.DeepParse;
        
        PipeConsole.Commit("Hello", "World").Parse(" ").Tag(SeverityTag.Debug).Print();
        PipeConsole.Commit(new int[3] { 1, 2, 3 }, 1, "HAlo").Parse(Parser.Deep, " : ").Tag(SeverityTag.Info).Print();
        PipeConsole.Commit(new float[2] { 1.02f, 2.01f }).Parse(Parser.Deep).Tag(SeverityTag.Error).Print();
        
        PipeConsole.Commit("WARNs").Parse().Tag(SeverityTag.Warning).Print();

        PipeConsole.Print("hallo", 123, 1.0234, new A());

        PipeConsole.Commit(123, 1.0, new System.Collections.Generic.List<int>() { 1, 12, 32 }).Parse().Print();

        PipeConsole.ClearConsole();

        PipeConsole.Print("Hello world!".Colorify("#CCFF01"));
        PipeConsole.Commit("Bye world!").Tag(SeverityTag.Error).Parse().Colorify("#FFCC10").Print(this);

        PipeConsole.Commit("A".Colorify("#654321"), "B".Colorify("#765432"), "C".Colorify("#876543")).Parse("").Print(gameObject);
        PipeConsole.Print();
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
