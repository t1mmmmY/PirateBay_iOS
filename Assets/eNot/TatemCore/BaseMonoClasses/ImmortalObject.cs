using UnityEngine;
using System.Collections;

public class ImmortalObject : MonoBehaviour
{
	void Awake()
	{
		GameObject.DontDestroyOnLoad (gameObject);
	}
}
