using UnityEngine;
using System.Collections;

public class DebugDisabler : MonoBehaviour
{
	void Awake ()
	{
		#if !UNITY_DEBUG && !UNITY_EDITOR
		gameObject.SetActive(false);
		#endif
	}
}
