using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlacedObject : MonoBehaviour 
{
	public TEAM team;
	
	Cell oldCell;

#if UNITY_EDITOR

	private void Update()
	{
		if (!Application.isPlaying)
		{
			PlaceToTheCell();
		}
	}
#endif

	protected void PlaceToTheCell()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position + Vector3.up * 20, Vector3.down, out hit))
		{
			Cell hitCell = hit.collider.gameObject.GetComponent<Cell>();
			if (hitCell != null)
			{
				if (hitCell.PlaceObject(this))
				{
					if (oldCell != null)
					{
						oldCell.DisplaceObject();
					}
				}
				oldCell = hitCell;
			}
			else
			{
				if (oldCell != null)
				{
					oldCell.DisplaceObject();
				}
			}

		}
	}



}
