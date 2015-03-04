using UnityEngine;
using System.Collections;

public class HumanPlayer : Player 
{

	public override void Init (TEAM playerTeam)
	{
		base.Init (playerTeam);
	}

	public override void Destroy ()
	{
		base.Destroy ();
	}

	protected override void OnStartGame ()
	{
		base.OnStartGame ();
	}

	protected override void OnEndTurn ()
	{
		base.OnEndTurn ();
	}

	protected override void OnNextTurn (TEAM nextTeam)
	{
		if (GameController.GetPlayerType(nextTeam) == PLAYER_TYPE.USER)
		{
			int countPawns = AIController.instance.GetPawnsCount(nextTeam, Board.GetBoardState());
			if (countPawns == 0)
			{
				GameController.EndGame(false);
			}
			//Debug.Log("User turn");
		}
		//Debug.Log("HumanNextTurn");
		base.OnNextTurn (nextTeam);
	}

}
