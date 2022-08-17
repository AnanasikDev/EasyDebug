using UnityEngine;
using EasyDebug;

public class MainExample : MonoBehaviour
{
    private void Start()
    {
        EDebug.ParseFunction = EDebug.DeepParse;
        
        EDebug.Commit("Hello", "World").Parse(" ").Do();
        EDebug.Commit(new int[3] { 1, 2, 3 }, 1, "HAlo").Parse(" ").Do();
    }
}
