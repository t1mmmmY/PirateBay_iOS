//using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// There are stored all information about pathfinding
/// </summary>
public static class Pathfinder
{
	/// <summary>
	/// Current table state.
	/// 0 - empty cell
	/// 1-2 - some type of pawn
	/// 71 - current pawn
	/// 255 - possible path
	/// 202 - inactive cell
	/// </summary>
	private static CELL_TYPE[,] currentTableState;
	
	/// <summary>
	/// Used for AI
	/// </summary>
	private static CELL_TYPE[,] tempTableState;


	public static CELL_TYPE[,] Encode(Cell[,] cells)
	{
		CELL_TYPE[,] tableState = new CELL_TYPE[cells.GetLength(0), cells.GetLength(1)];

		for (int i = 0; i < cells.GetLength(0); i++)
		{
			for (int j = 0; j < cells.GetLength(1); j++)
			{
				tableState[i,j] = GetCellType(cells[i,j]);
			}
		}

		//Encode to table
		return tableState;
	}

	public static CELL_TYPE GetCellType(Cell cell)
	{
		if (!cell.isActive)
		{
			return CELL_TYPE.INACTIVE_CELL;
		}
		else if (cell.isEmpty)
		{
			return CELL_TYPE.EMPTY;
		}
		else
		{
			if (cell.selected)
			{
				return CELL_TYPE.SELECTED_FIGURE;
			}
			else
			{
				switch (cell.placedShip.team)
				{
				case TEAM.WHITE_TEAM:
					switch (cell.placedShip.pawnType)
					{
					case PAWN_TYPE.PAWN:
						return CELL_TYPE.WHITE_PAWN;
					case PAWN_TYPE.DIAGONAL:
						return CELL_TYPE.WHITE_DIAGONAL;
					case PAWN_TYPE.KING:
						return CELL_TYPE.WHITE_KING;
					}
					break;
				case TEAM.BLACK_TEAM:
					switch (cell.placedShip.pawnType)
					{
					case PAWN_TYPE.PAWN:
						return CELL_TYPE.BLACK_PAWN;
					case PAWN_TYPE.DIAGONAL:
						return CELL_TYPE.BLACK_DIAGONAL;
					case PAWN_TYPE.KING:
						return CELL_TYPE.BLACK_KING;
					}
					break;
				}
			}
		}
		return CELL_TYPE.INACTIVE_CELL;
	}

	public static bool SaveTableState(CELL_TYPE[,] tableState, bool isTemp)
	{
		if (!isTemp)
		{
			currentTableState = tableState;
		}
		else
		{
			tempTableState = tableState;
		}

		//Print(tableState);

		return true;
	}


	public static PathData FindPossiblePathData(PAWN_TYPE pawnType, TEAM team, CELL_TYPE[,] tableState, Position selectedPos)
	{
		//		UnityEngine.Debug.Log(selectedPos.x + " " + selectedPos.y);
		//Position selectedPos = GetSelectedPos(tableState);
		if (selectedPos != new Position(-1, -1))
		{
			PathData pathData = Paths.GetPath(pawnType, team,
			                                  new Position(tableState.GetLength(0), tableState.GetLength(1)), 
			                                  selectedPos,
			                                  tableState);

			return pathData;
			//tableState = AddPathDataToTable(tableState, pathData, team);
			//Print(tableState);
			
//			return tableState;
		}
		else
		{
			return null;
		}
	}

	public static CELL_TYPE[,] FindPossiblePath(PAWN_TYPE pawnType, TEAM team, CELL_TYPE[,] tableState, Position selectedPos)
	{
//		UnityEngine.Debug.Log(selectedPos.x + " " + selectedPos.y);
		//Position selectedPos = GetSelectedPos(tableState);
		if (selectedPos != new Position(-1, -1))
		{
			PathData pathData = Paths.GetPath(pawnType, team,
			                                  new Position(tableState.GetLength(0), tableState.GetLength(1)), 
			                                  selectedPos,
			                                  tableState);
//			foreach (Position pos in pathData.move)
//			{
//				UnityEngine.Debug.Log("path: " + pos.x + " " + pos.y);
//			}
			tableState = AddPathDataToTable(tableState, pathData, team);
			//Print(tableState);
			
			return tableState;
		}
		else
		{
			return tableState;
		}
	}

	/// <summary>
	/// Finds possible path for the current pawn. Current pawn declared in table state
	/// </summary>
	/// <returns>Table with possible path. 255 - path</returns>
	/// <param name="pawnType">Pawn type.</param>
	/// <param name="tableState">Current table state.</param>
	public static CELL_TYPE[,] FindPossiblePath(PAWN_TYPE pawnType, TEAM team, CELL_TYPE[,] tableState)
	{

		Position selectedPos = GetSelectedPos(tableState);
		if (selectedPos != new Position(-1, -1))
		{
			PathData pathData = Paths.GetPath(pawnType, team,
		    	                              new Position(tableState.GetLength(0), tableState.GetLength(1)), 
			                                  selectedPos,
			                                  tableState);

			tableState = AddPathDataToTable(tableState, pathData, team);
			//Print(tableState);

			return tableState;
		}
		else
		{
			return tableState;
		}
	}

	public static bool IsMyTeam(CELL_TYPE type, TEAM team)
	{
		//UnityEngine.Debug.Log((int)type);
		if ((int)type >= (team == TEAM.BLACK_TEAM ? (int)CELL_TYPE.BLACK_PAWN : (int)CELL_TYPE.WHITE_PAWN) &&
		    (int)type <= (team == TEAM.BLACK_TEAM ? (int)CELL_TYPE.BLACK_KING : (int)CELL_TYPE.WHITE_KING))
		{
			return true;
		}
		else 
		{
			return false;
		}
	}

	public static bool IsEnemyTeam(CELL_TYPE type, TEAM team)
	{
		if ((int)type >= (team == TEAM.WHITE_TEAM ? (int)CELL_TYPE.BLACK_PAWN : (int)CELL_TYPE.WHITE_PAWN) &&
		    (int)type <= (team == TEAM.WHITE_TEAM ? (int)CELL_TYPE.BLACK_KING : (int)CELL_TYPE.WHITE_KING))
		{
			return false;
		}
		else 
		{
			return true;
		}
	}


	private static Position GetSelectedPos(CELL_TYPE[,] tableState)
	{
		for (int i = 0; i < tableState.GetLength(0); i++)
		{
			for (int j = 0; j < tableState.GetLength(1); j++)
			{
				if (tableState[i,j] == CELL_TYPE.SELECTED_FIGURE)
				{
//					UnityEngine.Debug.Log(i + " " + j);
					return new Position(i, j);
				}
			}
		}

		return new Position(-1, -1);
	}

	private static CELL_TYPE[,] AddPathDataToTable(CELL_TYPE[,] tableState, PathData pathData, TEAM team)
	{
		foreach (Position move in pathData.move)
		{
			//if (tableState[move.x, move.y] == CELL_TYPE.EMPTY)
			{
				tableState[move.x, move.y] = CELL_TYPE.POSSIBLE_PATH;
			}
		}
		foreach (Position hit in pathData.hit)
		{
			//if ((int)tableState[hit.x, hit.y] >= (team == TEAM.WHITE_TEAM ? (int)CELL_TYPE.BLACK_PAWN : (int)CELL_TYPE.WHITE_PAWN) &&
			//    (int)tableState[hit.x, hit.y] <= (team == TEAM.WHITE_TEAM ? (int)CELL_TYPE.BLACK_KING : (int)CELL_TYPE.WHITE_KING))
			{
				tableState[hit.x, hit.y] = CELL_TYPE.POSSIBLE_HIT;
			}
		}

		return tableState;
	}

	public static void Print(CELL_TYPE[,] tableState)
	{
		string line = string.Empty;
		for (int i = 0; i < tableState.GetLength(0); i++)
		{
			for (int j = 0; j < tableState.GetLength(1); j++)
			{
				line += string.Format("{0,-8}", (int)tableState[j,i]);
			}
			line += '\n';
		}
		UnityEngine.Debug.Log(line);
	}

}

[System.Serializable]
public struct Position
{
	public int x;
	public int y;

	public Position(int x, int y)
	{
		this.x = x;
		this.y = y;
	}



	public static bool operator ==(Position position1, Position position2)
	{
		return (position1.x == position2.x ? true : false) && (position1.y == position2.y ? true : false);
	}

	public static bool operator !=(Position position1, Position position2)
	{
		return !(position1 == position2);
	}

}

public class PathData
{
	public List<Position> move;
	public List<Position> hit;

	public PathData()
	{
		move = new List<Position>();
		hit = new List<Position>();
	}

	public void AddMove(Position pos)
	{
		move.Add(pos);
	}

	public void AddHit(Position pos)
	{
		hit.Add(pos);
	}

	public bool Contains(Position pos)
	{
		foreach (Position p in move)
		{
			if (pos == p)
			{
				return true;
			}
		}
		foreach (Position p in hit)
		{
			if (pos == p)
			{
				return true;
			}
		}
		return false;
	}
}


public static class Paths
{
	public static PathData GetPath(PAWN_TYPE type, TEAM team, Position boardSize, Position pawnPos, CELL_TYPE[,] tableState)
	{
		switch (type)
		{
		case PAWN_TYPE.PAWN:
			return PawnPath(team, boardSize, pawnPos, tableState);
		case PAWN_TYPE.DIAGONAL:
			return DiagonalPath(team, boardSize, pawnPos, tableState);
		case PAWN_TYPE.KING:
			return KingPath(team, boardSize, pawnPos, tableState);
		}
		
		return new PathData();
	}
	
	private static PathData PawnPath(TEAM team, Position boardSize, Position pawnPos, CELL_TYPE[,] tableState)
	{
		PathData pathData = new PathData();
		
		int direction = 0; 
		if (team == TEAM.WHITE_TEAM)
		{
			direction = -1; // Forward for white team
		}
		else
		{
			direction = 1; //Backward for black team
		}
		
		Position pos = new Position();
		
		//Pawn formula
//		for (int i = -2; i <= 2; i++)
//		{
//			pos = pawnPos;
//			pos.x += i;
//
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
////			else 
////			{
////				break;
////			}
//
//			pos.x = pawnPos.x;
//			pos.y += i;
//
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
////			{
////				break;
////			}
//		}

		//Right
		for (int i = -1; i >= -1; i--)
		{
			pos = pawnPos;
			pos.x += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else 
			{
				break;
			}
		}

		//Left
		for (int i = 1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else 
			{
				break;
			}

		}

		//Back
		for (int i = -1; i >= -1; i--)
		{
			pos = pawnPos;
			pos.y += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}

		//Forward
		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.y += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}

		
		return pathData;
	}

	private static PathData DiagonalPath(TEAM team, Position boardSize, Position pawnPos, CELL_TYPE[,] tableState)
	{
		PathData pathData = new PathData();
		
		int direction = 0; 
		if (team == TEAM.WHITE_TEAM)
		{
			direction = -1; // Forward for white team
		}
		else
		{
			direction = 1; //Backward for black team
		}
		
		Position pos = new Position();
	
		
//		for (int i = -2; i <= 2; i++)
//		{
//			pos = pawnPos;
//			pos.x += i;
//			pos.y += i;
//			
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//			
//			pos.y = pawnPos.y;
//			pos.y -= i;
//			
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//		}

		//BR
		for (int i = -1; i >= -1; i--)
		{
			pos = pawnPos;
			pos.x += i * direction;
			pos.y += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}

		//FL
		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.x += i * direction;
			pos.y += i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}

		//FR
		for (int i = -1; i >= -2; i--)
		{
			pos = pawnPos;
			pos.x += i * direction;
			pos.y -= i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}

		//BL
		for (int i = 1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i * direction;
			pos.y -= i * direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
				break;
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}
		

		return pathData;
	}
	
	private static PathData KingPath(TEAM team, Position boardSize, Position pawnPos, CELL_TYPE[,] tableState)
	{
		PathData pathData = new PathData();
		
		int direction = 0; 
		if (team == TEAM.WHITE_TEAM)
		{
			direction = -1; // Forward for white team
		}
		else
		{
			direction = 1; //Backward for black team
		}
		
		Position pos = new Position();
		
		//King formula
		for (int i = -1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			
			pos.x = pawnPos.x;
			pos.y += i;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
		}
		
		for (int i = -1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i;
			pos.y += i;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
			
			pos.y = pawnPos.y;
			pos.y -= i;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
			{
				pathData.AddMove(pos);
			}
		}


//		for (int i = -1; i >= -2; i--)
//		{
//			pos = pawnPos;
//			pos.x += i;
//			pos.y += i;
//
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//				break;
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//			else 
//			{
//				break;
//			}
//		}
//		for (int i = 1; i <= 2; i++)
//		{
//			pos = pawnPos;
//			pos.x += i;
//			pos.y += i;
//			
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//				break;
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//			else 
//			{
//				break;
//			}
//		}
//		for (int i = -1; i >= -2; i--)
//		{
//			pos = pawnPos;
//			pos.x += i;
//			pos.y -= i;
//			
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//				break;
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//			else 
//			{
//				break;
//			}
//		}
//		for (int i = 1; i <= 2; i++)
//		{
//			pos = pawnPos;
//			pos.x += i;
//			pos.y -= i;
//			
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//				break;
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
//			else 
//			{
//				break;
//			}
//		}

//			pos.y = pawnPos.y;
//			pos.y -= i;
//
//			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
//			{
//				pathData.AddHit(pos);
//			}
//			else if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY)
//			{
//				pathData.AddMove(pos);
//			}
		

		/*for (int i = -1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i;
			pos.y += direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
		}
		for (int i = -1; i <= 1; i++)
		{
			pos = pawnPos;
			pos.x += i;
			pos.y -= direction;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
		}
		for (int i = -1; i <= 1; i+=2)
		{
			pos = pawnPos;
			pos.x += i;
			
			if (InTheBounds(boardSize, pos) && !Pathfinder.IsEnemyTeam(tableState[pos.x, pos.y], team))
			{
				pathData.AddHit(pos);
			}
		}

		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.y += i * direction;

			if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY && !pathData.Contains(pos))
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}
		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.y -= i * direction;

			if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY && !pathData.Contains(pos))
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}
		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.x += i * direction;

			if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY && !pathData.Contains(pos))
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}
		for (int i = 1; i <= 2; i++)
		{
			pos = pawnPos;
			pos.x -= i * direction;

			if (InTheBounds(boardSize, pos) && tableState[pos.x, pos.y] == CELL_TYPE.EMPTY && !pathData.Contains(pos))
			{
				pathData.AddMove(pos);
			}
			else
			{
				break;
			}
		}*/
		
		return pathData;
	}



	/// <summary>
	/// Check if position in the bounds or not
	/// </summary>
	/// <returns><c>true</c>, if the bounds was ined, <c>false</c> otherwise.</returns>
	/// <param name="boardSize">Board size.</param>
	/// <param name="pos">Position.</param>
	private static bool InTheBounds(Position boardSize, Position pos)
	{
		if (pos.x >= 0 && pos.x < boardSize.x && pos.y >= 0 && pos.y < boardSize.y)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}