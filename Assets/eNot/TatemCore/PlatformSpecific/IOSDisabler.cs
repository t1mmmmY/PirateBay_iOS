using UnityEngine;
using System.Collections;

public class IOSDisabler : MonoBehaviour
{
	[SerializeField] bool onlyDisable;
	[SerializeField] bool isIphone;
	void Awake ()
	{
		if(!isIphone)
		{
			#if !UNITY_IOS && !UNITY_ANDROID
			if(onlyDisable)
			{
				gameObject.SetActive(false);
			}
			else
			{
				Destroy(gameObject);
			}
			#endif
		}
		else
		{
			#if UNITY_IOS || UNITY_ANDROID
			if(onlyDisable)
			{
				gameObject.SetActive(false);
			}
			else
			{
				Destroy(gameObject);
			}
			#endif
		}
	}
}
