using UnityEngine;
using System.Collections;

public class Vector2Range : PropertyAttribute 
{
	public int xMin;
	public int xMax;

	public int yMin;
	public int yMax;

	public Vector2Range(int xMin, int xMax, int yMin, int yMax)
	{
		this.xMin = xMin;
		this.xMax = xMax;
		this.yMin = yMin;
		this.yMax = yMax;
	}

//	public Vector2Range()
//	{
//	}
}
