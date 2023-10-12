namespace ImComponents;
public static class Singleton<T> where T : class, new()
{
    private static Lazy<T> instance = new Lazy<T>(() => new T());
    public static T Instance { get { return instance.Value; } }
}