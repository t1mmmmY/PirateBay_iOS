using UnityEngine;
using System.Collections;

public class GameLogicVisual : MonoBehaviour 
{
	public AnimationCamera animationCamera;

	void Start()
	{
		GameController.OnStartGame += OnStartGame;
		GameController.OnRestart += OnDestroy;
		GameController.OnEndTurn += OnEndTurn;
		GameController.OnNextTurn += OnNextTurn;
	}

	void OnDestroy()
	{
		GameController.OnStartGame -= OnStartGame;
		GameController.OnRestart -= OnDestroy;
		GameController.OnEndTurn -= OnEndTurn;
		GameController.OnNextTurn -= OnNextTurn;
	}

	void OnStartGame()
	{
		CameraAnimationStart();
	}

	void OnEndTurn()
	{
		DisableInput();
		//Wait until ship animation will be finished
	}

	void OnNextTurn(TEAM nextTeam)
	{
		if (GameController.GetPlayerType(nextTeam) == PLAYER_TYPE.USER)
		{
			//Move camera to the next player. If another player is not a bot... maybe
			MoveCameraToPlayer(GameController.currentTeam);
		}
		else
		{
			//Bot turn
		}
	}

#region Visual part

	void CameraAnimationStart()
	{
		//there should be some cool cinematic camera animation

		//Move camera to player white when animation is done. It's better to add this event in animation
		MoveCameraToPlayer(GameController.currentTeam);
	}

	void MoveCameraToPlayer(TEAM team)
	{
		//Move camera to player and begin the game
		EnableInput();
	}

//It would be great to show some animation when input enabled / disabled

	void EnableInput()
	{
		animationCamera.EnableInput();
	}
	
	void DisableInput()
	{
		animationCamera.DisableInput();
	}

#endregion
}
