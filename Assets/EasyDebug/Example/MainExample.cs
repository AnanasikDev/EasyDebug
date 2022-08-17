using UnityEngine;
using EasyDebug;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
        EDebug.SetFloatDivider();
        
        EDebug.Commit("Hello", "World").Parse(" ").Do();
        EDebug.Commit(new int[3] { 1, 2, 3 }, 1, "HAlo").Parse(Parser.Deep, " ").Do();
        EDebug.Commit(new float[2] { 1.02f, 2.01f }).Parse(Parser.Deep).Do();

        EDebug.Dommit("hallo", 123, 1.0234, new A());
    }
}

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
