using UnityEngine;
public class MainExample : MonoBehaviour
{
    private void Start()
    {
        EasyDebug.Log(1, 2, 3);
        EasyDebug.LogWarning(1, 2, 3, "as", new int[4] { 1, 2, 4, 5 });
        EasyDebug.Log(new int[3] { 1, 2, 3 });
        EasyDebug.LogCollection(new int[3] { 1, 2, 3 });
        EasyDebug.LogErrorCollection(new int[4] { 0, -1, 2, 3 });

        EasyDebug.Log("halo");
        EasyDebug.LogWarningSep(", ", 1, 2, 3, 4, 5, 6, 7);
    }
}
