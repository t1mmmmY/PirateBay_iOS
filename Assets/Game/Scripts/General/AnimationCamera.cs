using UnityEngine;
using System.Collections;

/// <summary>
/// Catch events from camera animations
/// </summary>
public class AnimationCamera : MonoBehaviour 
{
	[SerializeField] private Collider blockerCollider;

	public void EnableInput()
	{
		blockerCollider.enabled = false;
	}

	public void DisableInput()
	{
		blockerCollider.enabled = true;
	}

	void OnGUI()
	{
		if (blockerCollider.enabled)
		{
			GUI.color = Color.red;
			GUI.Label(new Rect(Screen.width - 100, 10, 100, 30), "Disabled");
		}
	}

}
