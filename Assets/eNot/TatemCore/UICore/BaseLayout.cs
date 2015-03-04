using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TatemGames.UI
{
	[ExecuteInEditMode]
	public class BaseLayout : MonoBehaviour
	{
		virtual public Bounds bounds
		{
			get;
			set;
		}
	}
}