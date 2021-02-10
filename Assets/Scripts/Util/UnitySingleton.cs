using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A class for creating a singleton that is designed to exist within an empty <c>GameObject</c> that exists within each scene
/// </summary>
/// <typeparam name="T">The class extending <c>UnitySingleton</c></typeparam>
public abstract class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
{
    public static T instance;
    protected void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = (T) this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
