using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICoroutine : MonoBehaviour
{
	private static UICoroutine _instance = null;

	public static UICoroutine Singleton
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<UICoroutine>();
				if (_instance == null)
				{
					GameObject go = new GameObject("[UICoroutine]");
					DontDestroyOnLoad(go);
					_instance = go.AddComponent<UICoroutine>();
				}
			}
			return _instance;
		}
	}

	private void OnDestroy()
	{
		GameObject.Destroy(gameObject);
		_instance = null;
	}
}
