using UnityEngine;

namespace Core.Scripts.Utils
{
    public class MonoSingleton<T> : MonoBehaviour
    {
        public static T instance;

        private void Awake()
        {
            if (instance is not null)
            {
                Debug.Log("Destroyed re instantiated singleton");
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = GetComponent<T>();
        }
    }
}