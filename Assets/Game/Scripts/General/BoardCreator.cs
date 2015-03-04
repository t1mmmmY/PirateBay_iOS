using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BoardCreator : MonoBehaviour 
{
	public Cell cellBase;
	
	[Vector2Range(3, 20, 3, 20)]
	public Vector2 boardSize = new Vector2(5, 5);
	
	
	private GameObject boardParent;
	private int boardNumber = 0;
	private Cell[,] allCells;
	
	public void CreateBoard()
	{
		allCells = new Cell[(int)boardSize.y, (int)boardSize.x];
		
		boardParent = new GameObject("Board_" + boardNumber.ToString());
		boardParent.transform.position = this.transform.position;
		boardParent.transform.rotation = this.transform.rotation;
		boardParent.AddComponent<Board>();
		boardParent.GetComponent<Board>().boardSize = boardSize;
		boardNumber++;
		
		for (int i = 0; i < allCells.GetLength(0); i++)
		{
			for (int j = 0; j < allCells.GetLength(1); j++)
			{
				
				allCells[i, j] = (Cell)GameObject.Instantiate(cellBase);
				allCells[i, j].transform.parent = boardParent.transform;
				allCells[i, j].gameObject.name = "cell_" + i.ToString() + "_" + j.ToString();
				
				allCells[i, j].transform.localPosition = new Vector3((i - allCells.GetLength(0) / 2) * cellBase.transform.localScale.x,
				                                                     0,
				                                                     (allCells.GetLength(1) / 2 - j) * cellBase.transform.localScale.z);
			}
		}
		
	}
}
