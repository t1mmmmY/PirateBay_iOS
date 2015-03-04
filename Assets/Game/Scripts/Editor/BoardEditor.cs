using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BoardCreator))]
public class BoardCratorEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector();

		BoardCreator boardCreator = (BoardCreator)target;
		if (GUILayout.Button("Create Board"))
		{
			boardCreator.CreateBoard();
		}
	}	
}
