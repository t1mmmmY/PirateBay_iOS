using UnityEngine;
using System.Collections;

public static class GameController 
{
	private static TEAM _currentTeam;
	private static PLAYER_TYPE whitePlayerType;
	private static PLAYER_TYPE blackPlayerType;
	private static Player whitePlayer;
	private static Player blackPlayer;

	public static System.Action OnStartGame;
	public static System.Action OnRestart;
	public static System.Action OnEndTurn;
	public static System.Action<bool> OnEndGame;
	public static System.Action<TEAM> OnNextTurn;
	public static System.Action OnDisablePath;

	public static TEAM currentTeam
	{
		get { return _currentTeam; }
	}

	public static PLAYER_TYPE GetPlayerType(TEAM team)
	{
		return team == TEAM.WHITE_TEAM ? whitePlayerType : blackPlayerType;
	}

	public static void StartGame(PLAYER_TYPE whitePlayerT, PLAYER_TYPE blackPlayerT)
	{
		whitePlayerType = whitePlayerT;
		blackPlayerType = blackPlayerT;
		_currentTeam = TEAM.WHITE_TEAM;

		if (whitePlayerType == PLAYER_TYPE.USER)
		{
			whitePlayer = new HumanPlayer();
		}
		else
		{
			whitePlayer = new Bot((int)AIController.instance.difficult);
		}

		if (blackPlayerType == PLAYER_TYPE.USER)
		{
			blackPlayer = new HumanPlayer();
		}
		else
		{
			blackPlayer = new Bot((int)AIController.instance.difficult);
		}

		whitePlayer.Init(TEAM.WHITE_TEAM);
		blackPlayer.Init(TEAM.BLACK_TEAM);
		//whitePlayer = whitePlayerT == PLAYER_TYPE.USER ? new HumanPlayer() : new Bot();
		//blackPlayer = blackPlayerT == PLAYER_TYPE.USER ? new HumanPlayer() : new Bot();

		if (OnStartGame != null)
		{
			OnStartGame();
		}
	}

	public static void Restart(PLAYER_TYPE whitePlayerT, PLAYER_TYPE blackPlayerT)
	{
		//Board.UnselectFigure();
		StartGame(whitePlayerT, blackPlayerT);

		if (OnRestart != null)
		{
			OnRestart();
		}
	}

	public static void EndTurn()
	{
		if (OnEndTurn != null)
		{
			OnEndTurn();
		}
	}

	public static TEAM NextTurn()
	{
		_currentTeam = _currentTeam == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM; //Switch team
		if (OnNextTurn != null)
		{
			OnNextTurn(_currentTeam);
		}
		return _currentTeam;
	}

	public static void EndGame(bool winner)
	{
//		if (winner)
//		{
//			PlayerPrefs.SetInt("PointState_" + (Application.loadedLevel - 1).ToString(), 1);
//		}
		if (OnEndGame != null)
		{
			OnEndGame(winner);
		}
	}

	public static void DisablePath()
	{
		if (OnDisablePath != null)
		{
			OnDisablePath();
		}
	}


	public static bool CanLoadLevel(int levelNumber)
	{
		return levelNumber <= CONST.COUNT_LEVELS; 
	}

}
