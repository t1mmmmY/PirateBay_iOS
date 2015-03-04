using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TreeOfTurns
{
	Branch rootNode;
	List<Branch> mainBranches;
	Branch[] mainBranchesArray;
//	public int countMainBranches
//	{
//		get 
//		{ 
//			if (mainBranches == null)
//			{
//				return 0;
//			}
//			else
//			{
//				return mainBranches.Count;
//			}
//		}
//	}

//	public OneTurn GetMainTurn(int number)
//	{
//		if (mainBranches == null)
//		{
//			return OneTurn.Empty;
//		}
//
//		if (mainBranches.Count < number)
//		{
//			return OneTurn.Empty;
//		}
//		else
//		{
//			return mainBranches[number].turn;
//		}
//
//	}
	
	public TreeOfTurns()
	{
		mainBranches = new List<Branch>();
	}
	
	private void AddMainBranch(Branch branch)
	{
//		if (mainBranches == null)
//		{
//			mainBranches = new List<Branch>();
//		}
		mainBranches.Add(branch);
		mainBranchesArray = mainBranches.ToArray();
	}

	private void AddMainBranches(List<Branch> branches)
	{
//		if (mainBranches == null)
//		{
//			mainBranches = new List<Branch>();
//		}
		mainBranches = branches;
		mainBranchesArray = mainBranches.ToArray();
	}

	private void AddMainBranches(List<OneTurn> firstTurns)
	{
//		if (mainBranches == null)
//		{
//			mainBranches = new List<Branch>();
//		}


		foreach (OneTurn turn in firstTurns)
		{
			mainBranches.Add(new Branch(Branch.Empty, turn));
		}
		mainBranchesArray = mainBranches.ToArray();
	}

	/// <summary>
	/// Creates the tree.
	/// </summary>
	/// <param name="deep">Deep can be 2 / 4 / 6 / 8 etc.</param>
	public void CreateTree(int deep, TEAM team, int direction)
	{
		rootNode = new Branch();

		rootNode.AddChildArray(AIController.instance.FillGraph(team, 
		                                                       direction, 
		                                                       AIController.instance.RestoreTurns(Branch.Empty, true), 
		                                                       rootNode));
		mainBranches = rootNode.childBrabches;

		if (mainBranches != null)
		{
//			foreach (Branch ch in mainBranches)
//			{
//				ch.turn.mass = (int)(ch.turn.mass * deep);
//			}

			mainBranchesArray = mainBranches.ToArray();
			foreach (Branch branch in mainBranches)
			{
				CreateTree(branch, deep, team, direction);

//				if (branch.childBrabches == null)
//				{
//					branch.turn.mass *= 1000 * deep;
//				}
			}
		}
		else
		{
			GameController.EndGame(true);
			Debug.LogWarning("User wins!");
		}
	}

	private void CreateTree(Branch branch, int deep, TEAM team, int direction)
	{
		direction = -direction; //change direction
		team = team == TEAM.BLACK_TEAM ? TEAM.WHITE_TEAM : TEAM.BLACK_TEAM; //change team

		branch.AddChildArray(AIController.instance.FillGraph(team, 
		                                                     direction, 
		                                                     AIController.instance.RestoreTurns(branch), 
		                                                     branch));

//		if (branch.childBrabches == null)
//		{
//			branch.turn.mass *= 1000 * deep;
//		}

//		if (branch.childBrabches != null && deep != 0)
//		{
//			foreach (Branch childBranch in branch.childBrabches)
//			{
//				childBranch.turn.mass *= deep;
//			}
//		}

		if (branch.childBrabches != null)
		{
//			foreach (Branch ch in branch.childBrabches)
//			{
//				ch.turn.mass = (int)(ch.turn.mass * deep);
//			}

			if (deep - 2 != 0)
			{

//				direction = -direction; //change direction
//				team = team == TEAM.BLACK_TEAM ? TEAM.WHITE_TEAM : TEAM.BLACK_TEAM; //change team

				foreach (Branch childBranch in branch.childBrabches)
				{
					CreateTree(childBranch, deep - 2, team, direction);

				}

//				if (branch.turn.from == new Position(1, 1) && branch.turn.to == new Position(2, 2))
//				{
//					int k = 0;
//				}

				direction = -direction; //change direction
				team = team == TEAM.BLACK_TEAM ? TEAM.WHITE_TEAM : TEAM.BLACK_TEAM; //change team

//				Branch bestEnemyTurn = FindBestPath(team,
//				                                    direction,
//				                                    branch.childBrabches,
//				                                    false);
				Branch bestEnemyTurn = FindBestPath(team == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM,
										            direction == 1 ? -1 : 1,
										            branch.childBrabches,
										            false);

				branch.childBrabches = new List<Branch>();
				branch.childBrabches.Add(bestEnemyTurn);
				if (branch.childBrabches != null)
				{
					branch.childBranchesArray = branch.childBrabches.ToArray();
				}


				foreach (Branch childBranch in branch.childBrabches)
				{
					
					foreach(Branch last in childBranch.GetLastNodes())
					{
//						last.AddChildArray(AIController.instance.FillGraph(team == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM,
//						                                                   direction == 1 ? -1 : 1,
//						                                                   AIController.instance.RestoreTurns(last), 
//						                                                   last));
						last.AddChildArray(AIController.instance.FillGraph(team,
						                                                   direction,
						                                                   AIController.instance.RestoreTurns(last), 
						                                                   last));
						
						if (last.childBrabches == null)
						{
							last.turn.mass *= 1000 * deep;
						}
						else
						{
//							foreach (Branch ch in last.childBrabches)
//							{
//								ch.turn.mass = (int)(ch.turn.mass * deep / 2.0f);
//							}
						}
					}
				}



			}
			else
			{
				//Is this right direction?
				branch.RemoveWorstBranches(direction);
//				branch.RemoveWorstBranches(direction == 1 ? -1 : 1);

				foreach (Branch childBranch in branch.childBrabches)
				{
					childBranch.AddChildArray(AIController.instance.FillGraph(team == TEAM.WHITE_TEAM ? TEAM.BLACK_TEAM : TEAM.WHITE_TEAM,
						                                                   	  direction == 1 ? -1 : 1,
					                                                          AIController.instance.RestoreTurns(childBranch), 
					                                                          childBranch));
						
					if (childBranch.childBrabches == null)
					{
						childBranch.turn.mass *= 1000 * deep;
					}
					else
					{
//						foreach (Branch ch in childBranch.childBrabches)
//						{
//							ch.turn.mass = (int)(ch.turn.mass * deep);
//						}
					}

					childBranch.RemoveWorstBranches(direction);
//					childBranch.RemoveWorstBranches(direction == 1 ? -1 : 1);

				}


			}
		}
		else
		{
			branch.turn.mass *= 1000 * deep;
		}



	}


	private Branch FindBestPath(TEAM team, int direction, List<Branch> firstBranbches, bool print = false)
	{
//		Branch[] fb = firstBranbches.ToArray();


		Branch bestTurn = Branch.Empty;
		int maxMass = int.MinValue;
		
		List<Branch> lastNodes = new List<Branch>();
		Branch lastTurn = Branch.Empty;
		Branch branchWithHistory = Branch.Empty;
		List<Branch> history = new List<Branch>();
		
		if (firstBranbches == null)
		{
			return Branch.Empty;
		}
		
		foreach (Branch branch in firstBranbches)
		{
			lastNodes = branch.GetLastNodes();
			
			foreach (Branch lastNode in lastNodes)
			{
				//				if (lastNode.turn.from == new Position(3, 2) && lastNode.turn.to == new Position(3,3)) //For debug
				//				{
				//					int k = 0;
				//				}
				Branch parent = lastNode;
				history = new List<Branch>();
				branchWithHistory = Branch.Empty;
				int mass = 0;
				bool done = false;
				float deep = 1;
				do
				{
					if (firstBranbches.Contains(parent))
					{
//						if (parent.turn.from == new Position(2, 3) && parent.turn.to == new Position(2, 2) && parent.turn.mass == 200)
//						{
//							int k = 0;
//						}
						done = true;
					}

					history.Add(parent);
					mass += (int)(parent.turn.mass * deep);
					parent = parent.parentBranch;
					deep += 0.5f;
					
				} while (!done);//(parent != rootNode);

				if (mass * direction > maxMass)
				{
					Branch[] h = history.ToArray();
					
					branchWithHistory = new Branch();
					CreateHistoryBranch(ref branchWithHistory, history, 0);
					
					maxMass = mass * direction;
					bestTurn = branchWithHistory;
					lastTurn = lastNode;
				}
				else if (mass * direction == maxMass)
				{
					random = AIController.instance.Randomize();

					
					if (random == 1)
					{
						Branch[] h = history.ToArray();

						branchWithHistory = new Branch();
						CreateHistoryBranch(ref branchWithHistory, history, 0);

						maxMass = mass * direction;
						bestTurn = branchWithHistory;
						lastTurn = lastNode;
					}
				}
			}
		}
		
		//		int t = 0;

		//if (print)
		{
			bool done = false;
			do
			{
				if (firstBranbches.Contains(lastTurn))
				{
					done = true;
				}

				if (print)
				{
					Debug.Log(lastTurn.turn.Print());
				}

				if (done)
				{
					return lastTurn;
				}
				
				lastTurn = lastTurn.parentBranch;
				
			} while (!done); //(lastTurn != rootNode);
			Debug.Log("maxMass " + maxMass + " " + bestTurn.turn.Print());
		}
		//else
		{
			//Debug.Log("maxMass " + maxMass + " " + bestTurn.turn.Print());
		}
		
		return bestTurn;
	}

	private void CreateHistoryBranch(ref Branch branch, List<Branch> history, int number)
	{
		if (number == 0)
		{
			branch = history[number];
			if (branch != null)
			{
				CreateHistoryBranch(ref branch, history, ++number);
			}
		}
		else if (number < history.Count) //Recursive add child
		{
			branch.parentBranch = history[number];
			if (branch.parentBranch != null)
			{
				CreateHistoryBranch(ref branch.parentBranch, history, ++number);
			}
		}
		else //Return from method
		{
			return;
			//return branch;
		}
	}

	public OneTurn FindBestTurn(TEAM team, int direction, bool print = false)
	{
		if (mainBranches != null)
		{
			Branch bestBranch = FindBestPath(team, direction, mainBranches, print);
//			if (bestBranch.childBrabches == null)
//			{
//				GameController.EndGame(false);
//			}
			return bestBranch.turn;
		}
		else
		{
			return OneTurn.Empty;
		}
	}


	int random = 1;
	
	private void Randomize()
	{
		random = Random.Range(0, 2);
//		Debug.Log(random);
	}



}

public class Branch
{
	public List<Branch> childBrabches;
	public Branch[] childBranchesArray;
	public Branch parentBranch;
	public OneTurn turn;
	
	public Branch()
	{
//		childBrabches = new List<Branch>();
		parentBranch = Branch.Empty;
	}
	
	public Branch(Branch parent, OneTurn turn)
	{
//		childBrabches = new List<Branch>();
		parentBranch = parent;
		this.turn = turn;
	}
	
	public void AddChild(Branch branch)
	{
//		if (childBrabches == null)
//		{
//			childBrabches = new List<Branch>();
//		}
		childBrabches.Add(branch);
		if (childBrabches != null)
		{
			childBranchesArray = childBrabches.ToArray();
		}
	}

	public void AddChildArray(List<Branch> branches)
	{
//		Debug.Log("AddChildArray");
//		foreach (Branch br in branches)
//		{
//			Debug.Log(br.turn.Print());
//		}

		childBrabches = branches;
		if (childBrabches != null)
		{
			childBranchesArray = childBrabches.ToArray();
		}

//		if (childBrabches == null)
//		{
//			childBrabches = new List<Branch>();
//		}
//		childBrabches.Add(branch);
	}

	public void RemoveWorstBranches(int direction)
	{
		if (childBrabches == null)
		{
			//childBrabches = new List<Branch>();
			return;
		}

		int maxMass = -10000;
		List<Branch> bestBranch = new List<Branch>();

		foreach (Branch branch in childBrabches)
		{
			if (branch.turn.mass * direction >= maxMass)
			{
//				random = 1;

				if (branch.turn.mass * direction == maxMass)
				{
					bestBranch.Add(branch);
//					Loom.QueueOnMainThread(Randomize);
				}
				else
				{
					maxMass = branch.turn.mass * direction;
					bestBranch = new List<Branch>();
					bestBranch.Add(branch);
				}
			}
			else
			{
				//childBrabches.Remove(branch);
			}
		}
		childBrabches = new List<Branch>();
		childBrabches.AddRange(bestBranch);
		if (childBrabches != null)
		{
			childBranchesArray = childBrabches.ToArray();
		}
	}


	public List<Branch> GetLastNodes()
	{
		List<Branch> lastNodes = new List<Branch>();

		GetLastNodes(ref lastNodes);

		return lastNodes;

	}

//	public Branch GetFirstNode()
//	{
//	}

	private void GetLastNodes(ref List<Branch> lastNodes)
	{
		if (childBrabches != null)
		{
			foreach (Branch branch in childBrabches)
			{
				if (branch.childBrabches == null)
				{
					lastNodes.Add(branch);
				}
				else
				{
					branch.GetLastNodes(ref lastNodes);
				}
			}
		}
		else
		{
			lastNodes.Add(this);
		}
	}

	public static Branch Empty
	{
		get
		{
			return null;
		}
	}

	int random = 1;
	
	private void Randomize()
	{
		random = Random.Range(0, 2);
		
	}

}


public struct OneTurn
{
	public int id;
	public int previousId;
	//	public int nextId;
	public Position from;
	public Position to;
	public int mass;
	public TEAM team;
	public ACTIONS actionType;

	public OneTurn(int ID, int PreviousID, Position From, Position To, int Mass, TEAM Team)
	{
		from = From;
		to = To;
		id = ID;
		previousId = PreviousID;
		mass = Mass;
		team = Team;
		actionType = ACTIONS.NOTHING;
	}

	public OneTurn(int ID, int PreviousID, Position From, Position To, int Mass, TEAM Team, ACTIONS ActionType)
	{
		from = From;
		to = To;
		id = ID;
		previousId = PreviousID;
		mass = Mass;
		team = Team;
		actionType = ActionType;
	}
	
	public string Print()
	{
		try
		{
			return string.Format("[{0}:{6}] {1}; {7} from {2},{3}; to {4},{5}", id, mass, from.x, from.y, to.x, to.y, previousId, team);
		}
		catch
		{
		}
		finally
		{	
		}
		return "empty";
	}
	
	public static OneTurn Empty
	{
		get 
		{
			return new OneTurn(-1, -1, new Position(-1, -1), new Position(-1, -1), -1, TEAM.WHITE_TEAM);
		}
	}
	
	public static bool operator ==(OneTurn turn1, OneTurn turn2)
	{
		return (turn1.id == turn2.id ? true : false);
	}
	
	public static bool operator !=(OneTurn turn1, OneTurn turn2)
	{
		return !(turn1 == turn2);
	}
}
