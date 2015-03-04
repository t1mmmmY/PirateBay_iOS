using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameHistory 
{
	private static List<OneTurn> turns;
	
	public static int countTurns
	{
		get { return turns.Count; }
	}
	
	public static System.Action OnAddTurn; 
	public static System.Action OnRevertLastTurn; 
	
	public static void Init()
	{
		turns = new List<OneTurn>();
		GameController.OnRestart += OnRestart;
	}
	
	public static void Destroy()
	{
		if (turns != null)
		{
			turns.Clear();
		}
		GameController.OnRestart -= OnRestart;
	}
	
	private static void OnRestart()
	{
		turns.Clear();
	}
	
	public static void AddTurn(OneTurn turn)
	{
		if (turns == null)
		{
			turns = new List<OneTurn>();
		}
		
		turns.Add(turn);
		
		if (OnAddTurn != null)
		{
			OnAddTurn();
		}
		
		//		foreach (OneTurn t in turns)
		//		{
		//			Debug.Log(string.Format("{0} from {1},{2} to {3},{4}", t.team, t.from.x, t.from.y, t.to.x, t.to.y));
		//		}
	}
	
	public static OneTurn GetLastTurn()
	{
		if (turns == null)
		{
			return OneTurn.Empty;
		}
		
		return turns[turns.Count-1];
	}
	
	public static void RevertLastTurn()
	{
		if (turns == null) //It's a first turn
		{
			return;
		}
		else if (turns.Count == 0) //It's a first turn
		{
			return;
		}
		
		if (GameController.GetPlayerType(GameController.currentTeam) != PLAYER_TYPE.USER) //Bot turn
		{
			return;
		}
		
		//Revert bot turn
		Board.RevertAction(turns[turns.Count-1]);
		turns.RemoveAt(turns.Count-1);
		
		//		if (turns.Count == 0) //It's a first turn
		//		{
		//			return;
		//		}
		//Revert human turn
		Board.RevertAction(turns[turns.Count-1]);
		turns.RemoveAt(turns.Count-1);
		
		if (OnRevertLastTurn != null)
		{
			OnRevertLastTurn();
		}
	}
	
}
