using System;

namespace MZCommon;
public static class Singleton<T>
{
    private static Lazy<T> instance = new(() => (T)Activator.CreateInstance(typeof(T), true)!);
    public static T Instance { get { return instance.Value; } }
}