using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace TatemGames.UI
{
	[Serializable]
	public class ExtendedSpriteDefinition
	{
		public bool isTiled;
		public bool isTrimed = true;
		public float slicedLeft = 0.25f;
		public float slicedRight = 0.75f;
		public float slicedDown = 0.25f;
		public float slicedUp = 0.75f;
	}
}