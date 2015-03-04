using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour 
{
	private static AIController _instance;
	public static AIController instance
	{
		get
		{
//			if (_instance == null)
//			{
//				_instance = FindObjectOfType<AIController>();
//			}
			return _instance;
		}
	}

	void Awake()
	{
		_instance = this;
		difficult = (DIFFICULT)PlayerPrefs.GetInt("DIFFICULT", 4);
	}

	public DIFFICULT difficult = DIFFICULT.NORMAL;
	List<Pawn> botPawns;
	int pathNumber = 0;
	int random = 1;

	bool isPlaying = true;

	public static System.Action<DIFFICULT> OnChangeDifficult;

	void Start()
	{
	}

	void OnDestroy()
	{
		isPlaying = false;
	}

	void Update()
	{
		random = Random.Range(0, 2);
	}

	public void SetDifficult(DIFFICULT newDiff)
	{
		difficult = newDiff;
		PlayerPrefs.SetInt("DIFFICULT", (int)difficult);

		if (OnChangeDifficult != null)
		{
			OnChangeDifficult(difficult);
		}
	}

	public void RunAsync(System.Action action)
	{
//		action();
		Loom.RunAsync(action);
	}


	void WhoIsIdiot()
	{
		do
		{
			Debug.Log("I am an idiot!:)");
		} while (isPlaying);
	}


	public List<OneTurn> FillGraph(TEAM team, int direction, CELL_TYPE[,] state, int parentId)
	{
		List<OneTurn> lastState = new List<OneTurn>();
		OneTurn oneTurn = new OneTurn();
		
		botPawns = GetMyPawns(team, state);
		if (botPawns.Count == 0)
		{
//			Debug.LogWarning("GAME OVER");
			return null;
		}

		List<Pawn> enemyPawns = GetMyPawns(team == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM, state);
		
		foreach(Pawn pawn in botPawns)
		{
			foreach(Position movePos in pawn.possibleMove)
			{
				int turnMass = (int)(DistanceToEnemy(enemyPawns, movePos) + DistanceToEnemy(enemyPawns, pawn.position, true));
				oneTurn = new OneTurn(pathNumber, parentId, pawn.position, movePos, turnMass * direction, team);
//				Debug.Log(oneTurn.Print());
				lastState.Add(oneTurn);
//				possiblePaths.Add(oneTurn);
				pathNumber++;
			}
			foreach(Position hitPos in pawn.possibleHit)
			{
				oneTurn = new OneTurn(pathNumber, parentId, pawn.position, hitPos, 
				                      GetPawnMass(state[hitPos.x, hitPos.y]) * direction, team);
				lastState.Add(oneTurn);
//				possiblePaths.Add(oneTurn);
				pathNumber++;
			}
		}
		
		
		//		foreach(OneTurn turn in lastState)
		//		{
		//			Debug.Log(turn.Print());
		//		}
		
		return lastState;
	}


	public List<Branch> FillGraph(TEAM team, int direction, CELL_TYPE[,] state, Branch parent)
	{
//		Debug.Log("FillGraph");

		List<Branch> lastState = new List<Branch>();
		OneTurn oneTurn = new OneTurn();
		
		botPawns = GetMyPawns(team, state);
		if (botPawns.Count == 0)
		{
//			Debug.LogWarning("GAME OVER");
			return null;
		}
		
		List<Pawn> enemyPawns = GetMyPawns(team == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM, state);

		foreach(Pawn pawn in botPawns)
		{
			foreach(Position movePos in pawn.possibleMove)
			{
				int turnMass = (int)(DistanceToEnemy(enemyPawns, movePos) + DistanceToEnemy(enemyPawns, pawn.position, true));
				oneTurn = new OneTurn(pathNumber, parent == Branch.Empty ? -1 : parent.turn.id, 
				                      pawn.position, movePos, turnMass * direction, team);
//				Debug.Log(oneTurn.Print());
				lastState.Add(new Branch(parent, oneTurn));
				//				possiblePaths.Add(oneTurn);
				pathNumber++;
			}
			foreach(Position hitPos in pawn.possibleHit)
			{
				oneTurn = new OneTurn(pathNumber, parent == Branch.Empty ? -1 : parent.turn.id, pawn.position, hitPos, 
				                      GetPawnMass(state[hitPos.x, hitPos.y]) * direction, team);
				lastState.Add(new Branch(parent, oneTurn));
				//				possiblePaths.Add(oneTurn);
				pathNumber++;
			}
		}
		
		
		//		foreach(OneTurn turn in lastState)
		//		{
		//			Debug.Log(turn.Print());
		//		}
		
		return lastState;
	}


	public int GetPawnsCount(TEAM team, CELL_TYPE[,] state)
	{
		int count = 0;
		foreach (CELL_TYPE type in state)
		{
			if (Pathfinder.IsMyTeam(type, team))
			{
				count++;
			}
		}

		return count;
	}
	
	private List<Pawn> GetMyPawns(TEAM team, CELL_TYPE[,] state)
	{
		//botPawns = new List<Pawn>();
		List<Pawn> pawns = new List<Pawn>();
		
		//		Debug.Log(team);
		
		int i = 0;
		foreach (CELL_TYPE type in state)
		{
			if (Pathfinder.IsMyTeam(type, team))
			{
				Pawn pawn = new Pawn();
				pawn.pawnType = GetPawnType(type);
				pawn.position = IndexToPosition(i, new Position(state.GetLength(0), state.GetLength(1)));
				PathData pathData = Pathfinder.FindPossiblePathData(pawn.pawnType, team, state, pawn.position);
				pawn.possibleMove = pathData.move;
				pawn.possibleHit = pathData.hit;
				
				//				Debug.LogWarning(team + " " + pawn.position.x + "," + pawn.position.y + " " + type);
				
				pawns.Add(pawn);
			}
			
			i++;
		}

		return pawns;
		//return botPawns.Count > 0 ? true : false;
	}


	public CELL_TYPE[,] RestoreTurns(Branch branch, bool firstTurn = false)
	{
		CELL_TYPE[,] boardState = Board.GetBoardState();

		if (branch == Branch.Empty)
		{
//			return boardState;
			Debug.LogWarning("Empty");
		}
		if (firstTurn)
		{
			return boardState;
		}

		List<OneTurn> turnChain = new List<OneTurn>();
				
		do
		{
			if (branch.turn.from == new Position(0, 0) && branch.turn.to == new Position(0, 0))
			{
			}
			else
			{
				turnChain.Add(branch.turn);
			}
			branch = branch.parentBranch;
			
		} while (branch != Branch.Empty);
		
		turnChain.Reverse();
		
		foreach(OneTurn historyTurn in turnChain)
		{
			boardState[historyTurn.to.x, historyTurn.to.y] = boardState[historyTurn.from.x, historyTurn.from.y];
			boardState[historyTurn.from.x, historyTurn.from.y] = CELL_TYPE.EMPTY;
		}

		return boardState;
	}

	
	public CELL_TYPE[,] RestoreTurns(CELL_TYPE[,] state, OneTurn turn)
	{
//		if (turn.id == 5)
//		{
//			int k = 0;
//		}
		CELL_TYPE[,] newState = new CELL_TYPE[state.GetLength(0),state.GetLength(1)];
		for (int i = 0; i < newState.GetLength(0); i++)
		{
			for (int j = 0; j < newState.GetLength(1); j++)
			{
				newState[i,j] = state[i,j];
			}
		}
		
//		int prevId = -1;
//		List<OneTurn> turnChain = new List<OneTurn>();
//		
//		do
//		{
//			turnChain.Add(turn);
//			prevId = turn.previousId;
//			
//			if (prevId != -1)
//			{
//				turn = FindTurnById(prevId);
//			}
//			
//		} while (prevId != -1);
//		
//		turnChain.Reverse();
//		
//		foreach(OneTurn historyTurn in turnChain)
//		{
//			newState[historyTurn.to.x, historyTurn.to.y] = newState[historyTurn.from.x, historyTurn.from.y];
//			newState[historyTurn.from.x, historyTurn.from.y] = CELL_TYPE.EMPTY;
//		}

		
		return newState;
		
	}

	public int Randomize()
	{
		return random;
	}

	private Position IndexToPosition(int index, Position arraySize)
	{
		Position pos = new Position();
		for (int i = 0; i < arraySize.y; i++)
		{
			pos.x = index - i * arraySize.x;
			if (pos.x >= 0 && pos.x < arraySize.x)
			{
				pos.x = i;
				pos.y = index - pos.x * arraySize.x;
				
				return pos;
			}
		}
		
		return new Position(-1, -1);
	}

	private float DistanceToEnemy(List<Pawn> enemyPawns, Position position, bool revert = false)
	{
		float distanceMass = 0;
		float mass = 0;

		foreach (Pawn pawn in enemyPawns)
		{
			float distance = Mathf.Sqrt(Mathf.Pow(position.x - pawn.position.x, 2) + Mathf.Pow(position.y - pawn.position.y, 2));

			if (distance == 0 && !revert)
			{
				distance = 1;
//				return 1;
			}
//			mass = (1.0f / distance + GetPawnMass(pawn.pawnType) / 10.0f);
			if (!revert)
			{
				mass = (30.0f / distance);
			}
			else
			{
				mass = distance;
			}

//			Debug.Log("distance " + distance + "; mass = " + mass);
//			Debug.Log("mass " + mass);
			if (mass > distanceMass)
			{
				distanceMass = mass;
			}
		}

		return distanceMass;
	}

	private int GetPawnMass(PAWN_TYPE type)
	{
		return (int)type;
	}
	
	private int GetPawnMass(CELL_TYPE cellType)
	{
		return (int)GetPawnType(cellType);
	}

	private PAWN_TYPE GetPawnType(CELL_TYPE cellType)
	{
		int typeIndex = (int)cellType;

		if (typeIndex == 70)
		{
			int k = 0;
		}

		if ((int)cellType >= (int)CELL_TYPE.BLACK_PAWN && (int)cellType <= (int)CELL_TYPE.BLACK_KING)
		{
			typeIndex -= 30;
		}
		
		switch (typeIndex)
		{
		case 11:
			return PAWN_TYPE.PAWN;
		case 41:
			return PAWN_TYPE.PAWN;
		case 20:
			return PAWN_TYPE.DIAGONAL;
		case 50:
			return PAWN_TYPE.DIAGONAL;
		case 40:
			return PAWN_TYPE.KING;
		case 71:
			return PAWN_TYPE.KING;
		}
		//typeIndex = (int)cellType - 10;
		
		return (PAWN_TYPE)typeIndex;
	}

}
