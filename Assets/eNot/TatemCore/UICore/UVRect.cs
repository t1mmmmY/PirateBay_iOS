using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TatemGames.UI
{
	public class UVRect
	{
		public float  x;
		public float  y;
		public float  width;
		public float  height;

		public UVRect()
		{
		}

		public UVRect(float x, float y, float width, float height)
		{
			this.x 		= x;
			this.y 		= y;
			this.width 	= width;
			this.height = height;
		}

		public UVRect(Rect rect)
		{
			x 		= rect.x;
			y 		= rect.y;
			width 	= rect.width;
			height 	= rect.height;
		}
	}
}