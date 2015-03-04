using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TatemGames.UI
{
	public class SpriteDefinition
	{
		public UVRect uvRect;
		public float  x;
		public float  y;
		public float  width;
		public float  height;
		public float  trimmedWidth;
		public float  trimmedHeight;
		public string name;
		public bool	  isFliped;
		public ExtendedSpriteDefinition extendedDefinition;
	}
}