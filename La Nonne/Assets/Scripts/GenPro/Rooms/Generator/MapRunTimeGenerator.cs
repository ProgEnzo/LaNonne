using UnityEngine;
using UnityEngine.Events;

public class MapRunTimeGenerator : MonoBehaviour
{
    public UnityEvent OnStart;
    void Start()
    {
        OnStart?.Invoke();
        Debug.Log("ca fonctionne ?");
    }
}
