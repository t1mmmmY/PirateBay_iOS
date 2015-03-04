using UnityEngine;
using System.Collections;

public class Point : MonoBehaviour 
{
	public int levelNumber = 1;
	public GameObject cross;

	private MapController mapController;

	public void Init(MapController map)
	{
		mapController = map;
	}

	public void Click()
	{
//		Debug.Log("Click");
		StartCoroutine("LoadLevel");
		cross.SetActive(true);

	}

	IEnumerator LoadLevel()
	{
		if (!cross.activeSelf)
		{
			yield return new WaitForSeconds(0.5f);
		}
		mapController.LoadLevel(levelNumber);

		yield break;
		//Application.LoadLevel(levelNumber);
		//yield return Application.LoadLevelAsync(levelNumber);
	}

	public void SetState(bool state)
	{
		cross.SetActive(state);
	}
}
