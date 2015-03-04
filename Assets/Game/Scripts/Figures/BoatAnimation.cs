using UnityEngine;
using System.Collections;

public class BoatAnimation : MonoBehaviour 
{
	public System.Action OnResurrect;

	public void Resurrect()
	{
//		Debug.LogWarning("Resurrect");
		if (OnResurrect != null)
		{
			OnResurrect();
		}
	}
}
