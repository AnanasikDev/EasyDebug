using UnityEngine;
public class MainExample : MonoBehaviour
{
    private void Start()
    {
        EasyDebug.Log(1, 2, 3);
        EasyDebug.LogWarning(1, 2, 3, "as", new int[4] { 1, 2, 4, 5 });
    }
}
