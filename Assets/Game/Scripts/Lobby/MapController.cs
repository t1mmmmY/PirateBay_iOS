using UnityEngine;
using System.Collections;

public class MapController : MonoBehaviour 
{
	public Point[] points;
	public Animator lightAnimator;

	private bool[] states;

	void Start()
	{
		LoadLevelState();
		ApplyLevelState();
	}

	/// <summary>
	/// Loads the state of the level from PlayerPrefs
	/// </summary>
	private void LoadLevelState()
	{
		states = new bool[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			states[i] = PlayerPrefs.GetInt("PointState_" + i, 0) == 0 ? false : true;
		}
	}

	private void ApplyLevelState()
	{
		for (int i = 0; i < states.Length; i++)
		{
			points[i].Init(this);
			points[i].SetState(states[i]);
		}
	}

	public void LoadLevel(int levelNumber)
	{
		StartCoroutine(_LoadLevel(levelNumber));
	}

	IEnumerator _LoadLevel(int levelNumber)
	{
		lightAnimator.SetTrigger("TurnOff");
		yield return new WaitForSeconds(2.0f);
		Application.LoadLevel(levelNumber);
//		yield return Application.LoadLevelAsync(levelNumber);
	}
}
