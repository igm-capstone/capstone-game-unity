using UnityEngine;
using System.Collections;

public class SingletonConsole : MonoBehaviour {
    
    [SerializeField]
    private bool isSingleton = true;

    private static SingletonConsole instance;

    void Awake()
    {
        if (isSingleton && instance != null) {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        instance = this;
    }
}
