﻿using UnityEngine;
using EasyDebug;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
        QDebug.SetFloatDivider();
        //QDebug.tagsAllowed = Tag.Debug | Tag.Info;
        QDebug.defaultParser = QDebug.DeepParse;
        
        QDebug.Commit("Hello", "World").Parse(" ").Tag(Tag.Debug).Do();
        QDebug.Commit(new int[3] { 1, 2, 3 }, 1, "HAlo").Parse(Parser.Deep, " : ").Tag(Tag.Info).Do();
        QDebug.Commit(new float[2] { 1.02f, 2.01f }).Parse(Parser.Deep).Tag(Tag.Error).Do();

        QDebug.Commit("WARNs").Parse().Tag(Tag.Warning).Do();

        QDebug.Dommit("hallo", 123, 1.0234, new A());

        QDebug.Commit(123, 1.0, new System.Collections.Generic.List<int>() { 1, 12, 32 }).Parse().Do();
    }
}

public class A
{
    public override string ToString()
    {
        return "[A: ooo]";
    }
}
