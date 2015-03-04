using UnityEngine;
using System.Collections;

namespace TatemGames.UI
{
	[ExecuteInEditMode]
	public class UICamera : BaseLayoutSingleton<UICamera>
	{
		public static Camera Camera
		{
			get
			{
				return Instance.GetComponent<Camera>();
			}
		}

		override public Bounds bounds
		{
			get
			{
				return new Bounds(Vector3.zero, new Vector3(GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect * 2, GetComponent<Camera>().orthographicSize * 2, 0));
			}
			
			set
			{
				
			}
		}

		void OnDrawGizmos()
		{
			Gizmos.DrawWireCube(transform.position, new Vector3(GetComponent<Camera>().orthographicSize * GetComponent<Camera>().aspect * 2, GetComponent<Camera>().orthographicSize * 2, 0));
		}

		void Update()
		{
			float newOrto = Mathf.Max(3.2f, Screen.height * 0.5f * 0.01f);

			if(GetComponent<Camera>().orthographicSize != newOrto)
			{
				GetComponent<Camera>().orthographicSize = newOrto;
			}
		}
	}
}