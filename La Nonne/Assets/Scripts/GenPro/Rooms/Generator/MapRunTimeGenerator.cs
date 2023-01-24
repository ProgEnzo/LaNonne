using UnityEngine;
using UnityEngine.Events;

public class MapRunTimeGenerator : MonoBehaviour
{
    public UnityEvent OnStart;

    private void Start()
    {
        OnStart?.Invoke();
    }
}
