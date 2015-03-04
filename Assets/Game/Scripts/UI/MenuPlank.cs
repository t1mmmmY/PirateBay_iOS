using UnityEngine;
using System.Collections;

public class MenuPlank : MonoBehaviour 
{
	[SerializeField] TweenPosition tween;

	bool forward = true;

	public void Trigger()
	{
		tween.Play(forward);
		forward = !forward;
	}

}
