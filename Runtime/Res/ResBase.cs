using System;
using UnityEngine.Profiling;
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
        private Action<ResBase> m_onLoadedEvent;
        private ResState m_loadState;
        private string m_assetPath;

        public UObject Asset { get; protected set; }
        public string Name { get; protected set; }

        public ResState State
        {
            get { return m_loadState; }
            set
            {
                m_loadState = value;
                if (m_loadState == ResState.LoadSuccess)
                {
                    m_onLoadedEvent?.Invoke(this);
                }
            }
        }

        public void AddOnLoadedEvent(Action<ResBase> onLoaded)
        {
            if (onLoaded != null)
                m_onLoadedEvent += onLoaded;
        }

        public void RemoveOnLoadedEvent(Action<ResBase> onLoaded)
        {
            m_onLoadedEvent -= onLoaded;
        }

        #region abstract

        public abstract bool LoadSync();
        public abstract void LoadAsync();
        #endregion

    }
}