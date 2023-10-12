namespace MZCommon;
public class Singleton<T> where T : new()
{
    private static Lazy<T> instance = new Lazy<T>(() => new T());
    Singleton() { }
    public static T Instance { get { return instance.Value; } }
}