using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIWindowsManager : BaseSingleton<UIWindowsManager>
{
	[SerializeField] BaseWindow[] windows;
	public			 Camera  	  camera;

	Dictionary<Type, BaseWindow> windowsDictionary = new Dictionary<Type, BaseWindow>();
	
	override protected void Awake()
	{
		for(int i = 0; i < windows.Length; i++)
		{
			windowsDictionary.Add(windows[i].GetType(), windows[i]);
		}

		camera = transform.parent.GetComponent<Camera>();

		base.Awake();
	}
	
	public static T GetWindow<T>() where T : BaseWindow
	{
		return Instance.windowsDictionary[typeof(T)] as T;
	}
}
