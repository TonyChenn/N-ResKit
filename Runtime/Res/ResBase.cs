using System;
using UObject = UnityEngine.Object;

namespace ResKit
{
    public interface IHandler
    {
        void OnLoadedComplete(ResBase self);
        void OnLoadedFailed(ResBase self);
    }

    public enum ResState
    {
        Waiting,
        Loading,
        Downloading,
        LoadSuccess,
        LoadFail,
        Cancel,
        Dispose
    }

    public abstract class ResBase : SimpleRC
    {
        private Action<ResBase> mOnLoadedEvent;
        private ResState mLoadState;
        private string mAssetPath;

        public UObject Asset { get; protected set; }
        public string Name { get; protected set; }

        public ResState State
        {
            get { return mLoadState; }
            set
            {
                mLoadState = value;
                if (mLoadState == ResState.LoadSuccess)
                {
                    mOnLoadedEvent?.Invoke(this);
                }
            }
        }

        public void AddOnLoadedEvent(Action<ResBase> onLoaded)
        {
            if (onLoaded != null)
                mOnLoadedEvent += onLoaded;
        }

        public void RemoveOnLoadedEvent(Action<ResBase> onLoaded)
        {
            mOnLoadedEvent -= onLoaded;
        }

        #region abstract

        public abstract bool LoadSync();
        public abstract void LoadAsync();
        #endregion
        
    }
}