using UnityEngine;
using System.Collections;

public class Board : MonoBehaviour 
{

	/// <summary>
	/// You should not change this in editor
	/// </summary>
	[Vector2Range(3, 20, 3, 20)]
	public Vector2 boardSize = Vector2.zero; 

	public Material cellMaterial;
	private float startA;

	private static Cell[,] allCells;
	public static CELL_TYPE[,] GetBoardState()
	{
//		int i = 0;
//		foreach (Cell cell in allCells)
//		{
//			Debug.Log(cell.name + " " + i);
//			i++;
//		}
		return Pathfinder.Encode(allCells);
	}

	void Start()
	{
		Cell[] startCells = GetComponentsInChildren<Cell>(true);
		allCells = new Cell[(int)boardSize.x, (int)boardSize.y];

		for (int i = 0; i < allCells.GetLength(0); i++)
		{
			for (int j = 0; j < allCells.GetLength(1); j++)
			{
				allCells[i, j] = startCells[i * allCells.GetLength(0) + j];
			}
		}
		SaveTableState(allCells);

		if (cellMaterial != null)
		{
			startA = cellMaterial.color.a;
			StartCoroutine("EnableBoard");
		}
	}

	void OnDestroy()
	{
		if (cellMaterial != null)
		{
			Color color = cellMaterial.color;
			color.a = startA;
			cellMaterial.color = color;
		}
	}

	IEnumerator EnableBoard()
	{
		Color color = cellMaterial.color;
		color.a = 0.0f;
		cellMaterial.color = color;

		yield return new WaitForSeconds(1.0f);

		do
		{
			color = cellMaterial.color;
			color.a += Time.deltaTime * startA;
			cellMaterial.color = color;
			yield return null;

		} while (color.a <= startA);

		color = cellMaterial.color;
		color.a = startA;
		cellMaterial.color = color;
	}


	static Cell currentCell;
	static PawnController selectedFigure;

	public static void SaveTableState(Cell[,] cells)
	{
		//Encode using pathfinder and send to them
		Pathfinder.SaveTableState(Pathfinder.Encode(cells), false);
	}

	public static void MakeAction(Position posFrom, Position posTo)
	{
		OnPushCell(posFrom.x, posFrom.y);
		OnPushCell(posTo.x, posTo.y);
	}

	public static void RevertAction(OneTurn revertTurn)
	{
		currentCell = allCells[revertTurn.to.x, revertTurn.to.y];
		selectedFigure = currentCell.placedShip;
		
		//Why is only move???
		switch (revertTurn.actionType)
		{
		case ACTIONS.MOVE:
			RevertMove(allCells[revertTurn.from.x, revertTurn.from.y], revertTurn);
			break;
		case ACTIONS.BEAT:
			RevertBeat(allCells[revertTurn.from.x, revertTurn.from.y], revertTurn);
			break;
		default:
			Debug.LogError("Revert not a move and not a beat action???");
			break;
		}

	}

	public static void OnPushCell(int x, int y)
	{
		OnPushCell(allCells[x, y]);
	}

	public static void OnPushCell(Cell newCell)
	{
		//NOT INCLUDED TEAM LOGIC YET

		if (selectedFigure == null) //Figure is not selected
		{
			if (newCell.placedShip == null) //Select empty cell
			{
				newCell.ActivateCell(ACTIONS.EMPTY_CELL);
				return;
			}
			else //Select cell with figure
			{
				if (GameController.currentTeam == newCell.placedShip.team) //Check team
				{
					SelectFigure(newCell);
				}
				else
				{
					//Select opponent figure
				}
			}
		}
		else //Some figure selected already
		{
			if (newCell == currentCell)
			{
				UnselectFigure();
			}
			else if (newCell.placedShip == null) //Move to the empty cell
			{
				if (newCell.possiblePath)
				{
					MoveFigure(newCell);
				}
			}
			else //This cell contains other ship
			{
				if (selectedFigure.team == newCell.placedShip.team) //Change selected figure. Or maybe make some oter action
				{
					UnselectFigure();
					SelectFigure(newCell);
				}
				else if (newCell.possiblePath) //beat opponent figure
				{
					BeatFigure(newCell);
				}

			}
		}

	}

	public static void UnselectFigure()
	{
		if (currentCell != null)
		{
			currentCell.DeactivateCell();
			currentCell = null;
			selectedFigure.Unselect();
			selectedFigure = null;

			GameController.DisablePath();
		}
	}

	public static Cell GetSelectedCell()
	{
		return currentCell;
	}

	private static void SelectFigure(Cell newCell)
	{
		currentCell = newCell;
		currentCell.ActivateCell(ACTIONS.SELECT);
		selectedFigure = currentCell.placedShip;
		selectedFigure.Select();

		ShowPossiblePath();
	}

	private static void MoveFigure(Cell newCell)
	{
//		if (revert == OneTurn.Empty)
		{
			OneTurn turn = new OneTurn(0, 0, currentCell.position, newCell.position, 0, GameController.currentTeam, ACTIONS.MOVE);
//			turn.actionType = ACTIONS.MOVE;
			GameHistory.AddTurn(turn);
		}

//		if (revert == OneTurn.Empty)
		{
			newCell.ActivateCell(ACTIONS.MOVE);
		}
		newCell.placedShip = selectedFigure;
		newCell.isEmpty = false;
		currentCell.placedShip.MoveHere(newCell.shipPosition.position);
		currentCell.placedShip = null;
		currentCell.isEmpty = true;

//		if (revert == OneTurn.Empty)
		{
			UnselectFigure();
			GameController.EndTurn();
		}
	}

	private static void RevertMove(Cell newCell, OneTurn revert)
	{
		newCell.placedShip = selectedFigure;
		newCell.isEmpty = false;
		currentCell.placedShip.RevertMoveHere(newCell.shipPosition.position, revert);
		currentCell.placedShip = null;
		currentCell.isEmpty = true;

	}

	private static void BeatFigure(Cell newCell)
	{
//		if (!revert)
		{
			OneTurn turn = new OneTurn(0, 0, currentCell.position, newCell.position, 0, GameController.currentTeam, ACTIONS.BEAT);
//			turn.actionType = ACTIONS.BEAT;
			GameHistory.AddTurn(turn);
		}
		currentCell.placedShip.BeatOppenent(newCell.placedShip, newCell.shipPosition.position);

//		if (!revert)
		{
			newCell.ActivateCell(ACTIONS.BEAT);
		}
		newCell.placedShip = selectedFigure;
		newCell.isEmpty = false;
		currentCell.placedShip = null;
		currentCell.isEmpty = true;

//		if (!revert)
		{
			UnselectFigure();
			GameController.EndTurn();
		}
	}

	private static void RevertBeat(Cell newCell, OneTurn revert)
	{
//		if (!revert)
//		{
//			OneTurn turn = new OneTurn(0, 0, currentCell.position, newCell.position, 0, GameController.currentTeam);
//			turn.actionType = ACTIONS.BEAT;
//			GameHistory.AddTurn(turn);
//		}
		currentCell.placedShip.RevertBeatOppenent(/*newCell.placedShip, */newCell.shipPosition.position, revert);
		
//		if (!revert)
//		{
//			newCell.ActivateCell(ACTIONS.BEAT);
//		}
		newCell.placedShip = selectedFigure;
		newCell.isEmpty = false;
		currentCell.placedShip = newCell.placedShip.opponent;
		currentCell.isEmpty = false;

		newCell.placedShip.RemoveLastOpponent();
		
//		if (!revert)
//		{
//			UnselectFigure();
//			GameController.EndTurn();
//		}
	}


	private static void ShowPossiblePath()
	{
		CELL_TYPE[,] code = Pathfinder.FindPossiblePath(selectedFigure.pawnType, selectedFigure.team, Pathfinder.Encode(allCells));
		for (int i = 0; i < code.GetLength(0); i++)
		{
			for (int j = 0; j < code.GetLength(1); j++)
			{
				switch (code[i,j])
				{
				case CELL_TYPE.POSSIBLE_PATH:
					allCells[i,j].PossibleMovement(CELL_TYPE.POSSIBLE_PATH, currentCell);
					break;
				case CELL_TYPE.POSSIBLE_HIT:
					allCells[i,j].PossibleMovement(CELL_TYPE.POSSIBLE_HIT, currentCell);
					break;
				}
			}
		}
	}

}
