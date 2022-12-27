using System;
using UObject = UnityEngine.Object;

namespace ResKit
{
    public interface IRefCounter
    {
        int RefCount { get; }
        void AddRef(UObject uObject = null);
        void RemoveRef(UObject uObject = null);
    }

    public abstract class SimpleRC : IRefCounter
    {
        public SimpleRC()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        /// <summary>
        /// 添加引用
        /// </summary>
        public void AddRef(UObject uObject = null)
        {
            ++RefCount;
        }

        /// <summary>
        /// 减少引用
        /// </summary>
        public void RemoveRef(UObject uObject = null)
        {
            --RefCount;
            if (RefCount == 0)
                OnReleaseRes();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected abstract void OnReleaseRes();
    }
}