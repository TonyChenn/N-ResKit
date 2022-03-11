using System.Collections.Generic;
using System.IO;
using NCore;
using ResKit;
using UnityEngine;

namespace N_AssetBundle.Runtime
{
    public class ResManager:MonoSinglton<ResManager>,ISingleton
    {
        private static bool mUseLocalAsset = true;
        private static bool mHotUpdate = false;
        
        public static Dictionary<string, ResBase> AllLoadedRes = new Dictionary<string, ResBase>(32);
        public void InitSingleton()
        {
            gameObject.name = "[ResMger]";
        }
        
        /// <summary>
        /// 是否使用本地资源(不使用AssetBundle)
        /// 只能在Editor模式下可以更改
        /// </summary>
        public static bool UseLocalAsset
        {
            get => Platform.IsEditor && mUseLocalAsset;
            set
            {
                if (Platform.IsEditor)
                    mUseLocalAsset = value;
            }
        }

        /// <summary>
        /// 是否启用HotUpdate
        /// </summary>
        public static bool HotUpdate
        {
            get
            {
                if (Platform.IsEditor) return mHotUpdate;
                return true;
            }
            set
            {
                if (Platform.IsEditor)
                    mHotUpdate = value;
            }
        }
        

        private void OnGUI()
        {
            if (Platform.IsEditor)
            {
                GUILayout.BeginVertical("box");
                foreach (var item in AllLoadedRes)
                {
                    GUILayout.Label($"AssetName: {item.Key}");
                }
                GUILayout.EndVertical();
            }
        }
    }
}