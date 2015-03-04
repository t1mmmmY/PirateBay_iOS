using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseWindow : MonoBehaviour 
{
	public bool isShown { get; protected set; }
	
	virtual public void Show()
	{
		isShown = true;
		
		gameObject.SetActive(true);
	}
	
	virtual public void Hide()
	{
		if(isShown)
		{
			isShown = false;
			
			StartCoroutine(HideCoroutine());
		}
	}
	
	virtual protected void OnHide()
	{
		gameObject.SetActive(false);
	}
	
	IEnumerator HideCoroutine()
	{
		OnHide(); yield return null;
	}

	public T GetWindow<T>() where T : BaseWindow
	{
		return UIWindowsManager.GetWindow<T>();
	}
}
