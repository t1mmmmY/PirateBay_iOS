using UnityEngine;

namespace TatemGames.UI
{
	public interface IColoredUI
	{
		Color MainColor
		{
			get;
			set;
		}

		float Alpha
		{
			get;
			set;
		}
		Vector3 Scale
		{
			set;
			get;
		}
		float Size
		{
			get;
			set;
		}

		TextAnchor Anchor
		{
			get;
			set;
		}

		bool usingAxisScaling
		{
			set;
			get;
		}

		void CreateMesh();
	}
}