using UnityEngine;
using System.Collections;

public class DifficultSelecter : MonoBehaviour 
{
	public DifficultIcon[] difficultIcons;
	public Transform selector;

	void Start()
	{
		switch (AIController.instance.difficult)
		{
		case DIFFICULT.NORMAL:
			selector.position = difficultIcons[0].transform.position;
			break;
		case DIFFICULT.INTERESTING:
			selector.position = difficultIcons[1].transform.position;
			break;
		}
		
	}

	public void MoveTo(DifficultIcon direction)
	{
		AIController.instance.SetDifficult(direction.difficult);
		iTween.MoveTo(selector.gameObject, direction.transform.position, 0.5f);
	}
}
