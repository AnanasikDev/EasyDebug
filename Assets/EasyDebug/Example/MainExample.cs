using UnityEngine;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
        QDebug.SetFloatDivider();
        //QDebug.tagsAllowed = Tag.Debug | Tag.Info;
        QDebug.defaultParser = QDebug.DeepParse;
        
        QDebug.Commit("Hello", "World").Parse(" ").Tag(Dag.Debug).Do();
        QDebug.Commit(new int[3] { 1, 2, 3 }, 1, "HAlo").Parse(Parser.Deep, " : ").Tag(Dag.Info).Do();
        QDebug.Commit(new float[2] { 1.02f, 2.01f }).Parse(Parser.Deep).Tag(Dag.Error).Do();
        
        QDebug.Commit("WARNs").Parse().Tag(Dag.Warning).Do();

        QDebug.Dommit("hallo", 123, 1.0234, new A());

        QDebug.Commit(123, 1.0, new System.Collections.Generic.List<int>() { 1, 12, 32 }).Parse().Do();

        QDebug.formatFunction = CustomFormat;

        QDebug.ClearConsole();

        QDebug.Dommit("Hello world!".Color("#CCFF01"));
        QDebug.Commit("Bye world!").Tag(Dag.Error).Parse().Color("#FFCC10").Do(this);

        QDebug.Commit("A".Color("#654321"), "B".Color("#765432"), "C".Color("#876543")).Parse("").Do(gameObject);
        QDebug.Dommit();
    }

    private string CustomFormat(EasyDebug.Entity entity)
    {
        return $"[{System.DateTime.Now}][{entity.tag}] : {entity.value}";
    }
}

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
