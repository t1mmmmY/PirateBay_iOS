using UnityEngine;
using System.Collections;

public class StateMachine : MonoBehaviour 
{
	[SerializeField] private PLAYER_TYPE playerWhiteType = PLAYER_TYPE.USER;
	[SerializeField] private PLAYER_TYPE playerBlackType = PLAYER_TYPE.USER;
	[SerializeField] private UIButton backButton;
	[SerializeField] GameObject winnerPanel;
	[SerializeField] GameObject loserPanel;

	bool possibleBack = false;

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);

		StartCoroutine("StartGameTest");
		Loom.Initialize();
	}

	void OnEnable()
	{
		GameHistory.OnAddTurn += OnAddTurn;
		GameHistory.OnRevertLastTurn += OnRevertLastTurn;
		GameController.OnEndGame += OnEndGame;
		DisableButton();
	}

	void OnDisable()
	{
		GameHistory.OnAddTurn -= OnAddTurn;
		GameHistory.OnRevertLastTurn -= OnRevertLastTurn;
		GameController.OnEndGame -= OnEndGame;
		GameHistory.Destroy();
	}

	void OnAddTurn()
	{
		if (GameHistory.countTurns % 2 == 0) //Wait until bot turn
		{
			StartCoroutine("PossibleBackTimer", 3);
		}
		else
		{
			DisableButton();
		}
	}

	void OnRevertLastTurn()
	{
		if (GameHistory.countTurns > 0) //Wait until bot turn
		{
			StartCoroutine("PossibleBackTimer", 3);
		}
		else
		{
			DisableButton();
		}
	}

	void OnEndGame(bool winner)
	{
		if (winner) //User wins
		{
			Loom.QueueOnMainThread( () =>
			                       {
				PlayerPrefs.SetInt("PointState_" + (Application.loadedLevel - 1).ToString(), 1);
				NGUITools.SetActive(winnerPanel, true);
			}
			);
//			winnerPanel.gameObject.SetActive(true);
		}
		else
		{
			Loom.QueueOnMainThread( () =>
			                       {
				//PlayerPrefs.SetInt("PointState_" + (Application.loadedLevel - 1).ToString(), 1);
				NGUITools.SetActive(loserPanel, true);
			}
			);
		}
	}

	IEnumerator StartGameTest()
	{
		yield return new WaitForSeconds(1.0f);
		
		GameController.StartGame(playerWhiteType, playerBlackType);
	}


	public void Back()
	{
		if (possibleBack)
		{
			GameHistory.RevertLastTurn();
			//StartCoroutine("PossibleBackTimer", 3);
		}
	}

	public void Restart()
	{ 
		ResetState();
		Application.LoadLevel(Application.loadedLevel);
	}

	public void ToTheLobby()
	{
		ResetState();
		Application.LoadLevel(0);
	}

	public void NextLevel()
	{
		ResetState();
		if (GameController.CanLoadLevel(Application.loadedLevel+1))
		{
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}

	private void ResetState()
	{
		Loom.Destroy();
		StopCoroutine("PossibleBackTimer");
		GameController.Restart(playerWhiteType, playerBlackType);
	}

	IEnumerator PossibleBackTimer(int waitTime)
	{
		DisableButton();
		yield return new WaitForSeconds(waitTime);
		EnableButton();

	}

	private void DisableButton()
	{
		possibleBack = false;
		backButton.state = UIButtonColor.State.Disabled;
		backButton.GetComponent<Collider>().enabled = false;
	}

	private void EnableButton()
	{
		possibleBack = true;
		backButton.state = UIButtonColor.State.Normal;
		backButton.GetComponent<Collider>().enabled = true;
	}

}
