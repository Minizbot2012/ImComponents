namespace MZCommon;
public static class Singleton<T>
{
    private static Lazy<T> instance = new Lazy<T>(() => (T)Activator.CreateInstance(typeof(T), true)!);
    public static T Instance { get { return instance.Value; } }
}