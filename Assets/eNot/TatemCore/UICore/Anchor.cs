using UnityEngine;
using System.Collections;

namespace TatemGames.UI
{
	[ExecuteInEditMode]
	public class Anchor : MonoBehaviour
	{
		public BaseLayout layout;
		public Vector2 anchorDistance = Vector2.right;

		void Start()
		{
			LateUpdate ();
		}
		
		void LateUpdate ()
		{
			if(layout == null)
			{
				//enabled = false;
				
				return;
			}

			var localCenter = layout.bounds.center;
			var extends = new Vector3
			(
				anchorDistance.x * layout.bounds.extents.x,
				anchorDistance.y * layout.bounds.extents.y,
				0
			);


			localCenter = layout.transform.TransformPoint(localCenter + extends);
			localCenter = transform.parent.InverseTransformPoint(localCenter);


			
			//extends = layout.transform.TransformPoint(extends);
			//extends = transform.parent.InverseTransformPoint(extends);

			var newPos = new Vector3
			(
				localCenter.x,// + extends.x,
				localCenter.y,// + extends.y,
				transform.localPosition.z
			);

			if(newPos != transform.localPosition)
			{
				transform.localPosition = newPos;
			}
		}
	}
}