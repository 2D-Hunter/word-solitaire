// Create a new script called ListPool.cs
using System.Collections.Generic;

public static class ListPool<T>
{
    private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_ListPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_ListPool.Release(toRelease);
    }
}

public class ObjectPool<T> where T : new()
{
    private readonly System.Action<T> m_ActionOnGet;
    private readonly System.Action<T> m_ActionOnRelease;
    private readonly Stack<T> m_Stack = new Stack<T>();

    public ObjectPool(System.Action<T> actionOnGet, System.Action<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = new T();
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }
}