using UnityEngine;
using System.Collections;

namespace TatemGames.UI
{
	public class BaseLayoutSingleton<T> : BaseLayout  where T : BaseLayoutSingleton<T>
	{
		public static T Instance{ get; private set; }
		
		virtual protected void Awake()
		{
			Instance = this as T;
		}
		
		virtual protected void OnDestroy()
		{
			Instance = null;
		}
	}
}