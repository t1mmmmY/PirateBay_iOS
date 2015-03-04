using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TatemGames.UI
{
	public class TextureAtlas : MonoBehaviour
	{
		public TextAsset atlasJSON;// {protected set; get;}
		public Texture2D texture;//  {protected set; get;}

		public  Material  material
		{
			get
			{
				if(texture != null && mainMaterial.mainTexture != texture)
				{
					mainMaterial.mainTexture = texture;
				}

				return mainMaterial;
			}
		}

		public Material mainMaterial;

		protected Dictionary<string, SpriteDefinition> sprites
		{
			get
			{
				if(_sprites == null)
				{
					_sprites = JsonFx.Json.JsonReader.Deserialize<Dictionary<string, SpriteDefinition>>(atlasJSON.text);
				}

				return _sprites;
			}
		}
		protected Dictionary<string, SpriteDefinition> _sprites;

		public Dictionary<string, SpriteDefinition> Sprites
		{
			get
			{
				return sprites;
			}
		}

		public SpriteDefinition this[string spriteName] 
		{
			get 
			{
				if(sprites.ContainsKey(spriteName))
				{
					return sprites[spriteName];
				}
				else
				{
					return null;
				}
			}
		}

		public void Refresh()
		{
			_sprites = null;
		}
	}
}