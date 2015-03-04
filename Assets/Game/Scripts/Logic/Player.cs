using UnityEngine;
using System.Collections;

public abstract class Player 
{
	public TEAM playerTeam;
//	public Player()
//	{
//	}

	public virtual void Init(TEAM playerTeam)
	{
		this.playerTeam = playerTeam;

		GameController.OnStartGame += OnStartGame;
		GameController.OnRestart += Destroy;
		GameController.OnEndTurn += OnEndTurn;
		GameController.OnNextTurn += OnNextTurn;
	}
	
	public virtual void Destroy()
	{
		GameController.OnStartGame -= OnStartGame;
		GameController.OnRestart -= Destroy;
		GameController.OnEndTurn -= OnEndTurn;
		GameController.OnNextTurn -= OnNextTurn;
	}
	
	protected virtual void OnStartGame()
	{
	}
	
	protected virtual void OnEndTurn()
	{
	}
	
	protected virtual void OnNextTurn(TEAM nextTeam)
	{

	}

}
