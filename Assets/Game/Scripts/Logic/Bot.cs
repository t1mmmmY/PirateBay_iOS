using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct Pawn
{
	public PAWN_TYPE pawnType;
	public Position position;
	public List<Position> possibleMove;
	public List<Position> possibleHit;
	
}


public class Bot : Player 
{
	public Bot _current;
	public int botBrain = 4;
	TEAM botTeam;
	List<Pawn> botPawns;
	private List<OneTurn> possiblePaths;

	int pathNumber = 0;
	CELL_TYPE[,] tableState;
	OneTurn bestTurn = OneTurn.Empty;
	TreeOfTurns treeOfTurns;

	public Bot(int brain)
	{
		botBrain = brain;

	}

	public override void Init (TEAM playerTeam)
	{
		possiblePaths = new List<OneTurn>();
		AIController.OnChangeDifficult += OnChangeDifficult;
		
		base.Init (playerTeam);
	}
	
	public override void Destroy ()
	{
		AIController.OnChangeDifficult -= OnChangeDifficult;
		base.Destroy ();
	}

	private void OnChangeDifficult(DIFFICULT difficult)
	{
		botBrain = (int)difficult;
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
		if (GameController.GetPlayerType(nextTeam) == PLAYER_TYPE.AI)
		{
			Debug.Log("Bot turn");

			botTeam = nextTeam;


			AIController.instance.RunAsync(FindNextPath);
		}
		
		base.OnNextTurn (nextTeam);
	}



	void FindNextPath()
	{
		treeOfTurns = new TreeOfTurns();

		tableState = Board.GetBoardState(); //Init start state

		pathNumber = 0;
		int deep = botBrain;
		int direction = 1;
		TEAM team = botTeam;

		treeOfTurns.CreateTree(deep, team, direction);
		bestTurn = treeOfTurns.FindBestTurn(team, direction, true);
		if (bestTurn == OneTurn.Empty)
		{
			Debug.LogWarning("Game over");
			return;
		}

		Debug.Log("Best turn mass = " + bestTurn.mass);

		Loom.QueueOnMainThread(MakeAction); 


	}


	private void MakeAction()
	{
//		GameHistory.AddTurn(bestTurn);
		Board.MakeAction(bestTurn.from, bestTurn.to);
	}

}