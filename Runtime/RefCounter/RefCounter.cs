using System;
using UObject = UnityEngine.Object;


public interface IRefCounter
{
    int RefCount { get; }
    void AddRef();
    void RemoveRef();
}

public abstract class RefCounter : IRefCounter
{
    public RefCounter()
    {
        RefCount = 0;
    }

    public int RefCount { get; private set; }

    /// <summary>
    /// 添加引用
    /// </summary>
    public virtual void AddRef()
    {
        ++RefCount;
    }

    /// <summary>
    /// 减少引用
    /// </summary>
    public virtual void RemoveRef()
    {
        --RefCount;
        if (RefCount == 0)
            OnZeroRef();
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    protected abstract void OnZeroRef();
}
