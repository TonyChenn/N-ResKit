using System;
using System.Collections.Generic;
using System.IO;
using N_AssetBundle.Runtime;
using UObject = UnityEngine.Object;

namespace ResKit
{
    public class ResLoader
    {
        // AB包
        private Dictionary<string, ResBase> mCachedRes = new Dictionary<string, ResBase>(8);

        #region API
        public T LoadSync<T>(string assetBundleName, string assetName) where T : UObject
        {
            return DoLoadSync<T>(assetName, assetBundleName);
        }
		
        public T LoadSync<T>(string assetName) where T : UObject
        {
            return DoLoadSync<T>(assetName);
        }

        public void LoadAsync<T>(string assetName, Action<T> onLoaded) where T : UObject
        {
            DoLoadAsync(assetName, null, onLoaded);
        }

        public void LoadAsync<T>(string assetBundleName, string assetName, Action<T> onLoaded) where T : UObject
        {
            DoLoadAsync(assetName, assetBundleName, onLoaded);
        }
        public void ReleaseAll()
        {
            foreach (var item in mCachedRes)
                item.Value.RemoveRef();

            mCachedRes.Clear();
        }

        #endregion

        
        private T DoLoadSync<T>(string assetName, string bundleName=null ) where T : UObject
        {
            ResBase res = GetRes(assetName);
            if (res != null)
            {
                if (res.State == ResState.LoadSuccess)
                    return res.Asset as T;
                if(res.State==ResState.Loading)
                    throw new Exception(string.Format("请不要在异步加载资源 {0} 时，进行 {0} 的同步加载", res.Name));
            }

            res = CreateRes(assetName, bundleName);
            res.LoadSync();

            return res.Asset as T;
        }
        
        private void DoLoadAsync<T>(string assetName, string bundleName, Action<T> onLoaded) where T : UObject
        {
            ResBase res = GetRes(assetName);
            Action<ResBase> onResLoaded = null;
            onResLoaded = (loadRes) =>
            {
                onLoaded(loadRes.Asset as T);
                res.RemoveOnLoadedEvent(onResLoaded);
            };
            if (res != null)
            {
                if (res.State == ResState.LoadSuccess)
                    onLoaded(res.Asset as T);
                else if (res.State == ResState.Loading)
                    res.AddOnLoadedEvent(onResLoaded);
                return;
            }

            res = CreateRes(assetName, bundleName);
            res.AddOnLoadedEvent(onResLoaded);
            res.LoadAsync();
        }
        // 获取资源
        private ResBase GetRes(string assetName)
        {
            if (mCachedRes.ContainsKey(assetName))
                return mCachedRes[assetName];

            if (ResManager.AllLoadedRes.ContainsKey(assetName))
                return ResManager.AllLoadedRes[assetName];

            return null;
        }

        // 创建Bundle资源
        private ResBase CreateRes(string assetName,string bundleName = null)
        { 
            ResBase res = ResFactory.Create(assetName, bundleName);

            ResManager.AllLoadedRes[assetName] = res;
            mCachedRes[assetName] = res;
            res.AddRef();
            
            return res;
        }
    }
}