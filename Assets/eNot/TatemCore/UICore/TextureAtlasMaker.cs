using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TatemGames.UI.Atlas;

namespace TatemGames.UI
{
	public static class TextureAtlasMaker
	{
		public class SpriteTexture
		{
			public Texture2D		texture;
			public Color[,] 		colors;
			public SpriteDefinition sprite;
			public int width;
			public int height;
			public float pixelSize;

			public SpriteTexture(Texture2D texture, float pixelSize, ExtendedSpriteDefinition extendedSpriteDefinition)
			{
				this.texture   = texture;
				this.sprite    = new SpriteDefinition();
				this.pixelSize = pixelSize;

				sprite.extendedDefinition = extendedSpriteDefinition;
				sprite.name 			  = texture.name;

				GetColors();
			}

			private void GetColors()
			{
				width  = texture.width;
				height = texture.height;

				colors = new Color[width, height];
				
				for(int i = 0; i < width; i++)
				{
					for(int k = 0; k < height; k++)
					{
						colors[i, k] = texture.GetPixel(i, k);
					}
				}

				sprite.width  = width * pixelSize;
				sprite.height = height * pixelSize;

				sprite.trimmedWidth  = width * pixelSize;
				sprite.trimmedHeight = height * pixelSize;
			}

			public void Trim()
			{
				int left  = width;
				int up 	  = height;
				int right = 0;
				int down  = 0;

				for(int i = 0; i < width; i++)
				{
					for(int k = 0; k < height; k++)
					{
						if(colors[i, k].a != 0)
						{
							if(i < left)
							{
								left = i;
							}
							if(i > right)
							{
								right = i;
							}
							if(k < up)
							{
								up = k;
							}
							if(k > down)
							{
								down = k;
							}
						}
					}
				}
			
				int trimmedWidth  = right - left + 1;
				int trimmedHeight = down  - up + 1;

				var trimmedColors = new Color[trimmedWidth, trimmedHeight];

				for(int i = left; i < right + 1; i++)
				{
					for(int k = up; k < down + 1; k++)
					{
						trimmedColors[i - left, k - up] = colors[i, k];
					}
				}

				width  = trimmedWidth;
				height = trimmedHeight;

				colors = trimmedColors;

				sprite.x 			 = left * pixelSize;
				sprite.y 			 = up * pixelSize;
				sprite.trimmedWidth  = width * pixelSize;
				sprite.trimmedHeight = height * pixelSize;
			}
		}

		private static Color[] CreateTexture(TatemGames.UI.Atlas.Data atlasData, List<SpriteTexture> sprites, int paddingX, int paddingY)
		{
			var colors = new Color[atlasData.width * atlasData.height];
			
			for(int i = 0; i < atlasData.width; i++)
			{
				for(int k = 0; k < atlasData.height; k++)
				{
					colors[i + k * atlasData.width] = new Color(0, 0, 0, 0);
				}
			}
			
			for (int i = 0; i < atlasData.entries.Length; ++i)
			{
				var entry  = atlasData.entries[i];
				var sprite = sprites[entry.index];

				int extraPadding = sprite.sprite.extendedDefinition != null ? (sprite.sprite.extendedDefinition.isTiled ? 1 : 0) : 0;

				int pX = paddingX + extraPadding;
				int pY = paddingY + extraPadding;

				if(!entry.flipped)
				{
					if(sprite.sprite.extendedDefinition != null && sprite.sprite.extendedDefinition.isTiled)
					{
						for(int y = 0; y < sprite.height; ++y)
						{
							int x = -1;

							colors[entry.x + x + pX + (entry.y + y + pY) * atlasData.width] = sprite.colors[0, y];

							x = sprite.width;
							
							colors[entry.x + x + pX + (entry.y + y + pY) * atlasData.width] = sprite.colors[sprite.width - 1, y];
						}

						for(int x = 0; x < sprite.width; ++x)
						{
							int y = -1;
							
							colors[entry.x + x + pX + (entry.y + y + pY) * atlasData.width] = sprite.colors[x, 0];
							
							y = sprite.height;
							
							colors[entry.x + x + pX + (entry.y + y + pY) * atlasData.width] = sprite.colors[x, sprite.height - 1];
						}
					}

					for(int y = 0; y < sprite.height; ++y)
					{
						for(int x = 0; x < sprite.width; ++x)
						{
							colors[entry.x + x + pX + (entry.y + y + pY) * atlasData.width] = sprite.colors[x, y];
						}
					}

					sprite.sprite.uvRect = new UVRect((entry.x + pX) / (float)atlasData.width, (entry.y + pY) / (float)atlasData.height, (entry.w - pX * 2) / (float)atlasData.width, (entry.h - pY * 2) / (float)atlasData.height);
				}
				else
				{
					if(sprite.sprite.extendedDefinition != null && sprite.sprite.extendedDefinition.isTiled)
					{
						for(int y = 0; y < sprite.height; ++y)
						{
							int x = -1;
							
							colors[entry.x + y + paddingY + (entry.y + x + pX) * atlasData.width] = sprite.colors[0, sprite.height - 1 - y];
							
							x = sprite.width;
							
							colors[entry.x + y + paddingY + (entry.y + x + pX) * atlasData.width] = sprite.colors[sprite.width - 1, sprite.height - 1 - y];
						}
						
						for(int x = 0; x < sprite.width; ++x)
						{
							int y = -1;
							
							colors[entry.x + y + paddingY + (entry.y + x + pX) * atlasData.width] = sprite.colors[x, sprite.height - 1];
							
							y = sprite.height;
							
							colors[entry.x + y + paddingY + (entry.y + x + pX) * atlasData.width] = sprite.colors[x, 0];
						}
					}
					
					for(int y = 0; y < sprite.height; ++y)
					{
						for(int x = 0; x < sprite.width; ++x)
						{
							colors[entry.x + y + paddingY + (entry.y + x + pX) * atlasData.width] = sprite.colors[x, sprite.height - 1 - y];
						}
					}
					
					sprite.sprite.uvRect = new UVRect((entry.x + pY) / (float)atlasData.width, (entry.y + pX) / (float)atlasData.height, (entry.w - pY * 2) / (float)atlasData.width, (entry.h - pX * 2) / (float)atlasData.height);
				}

				sprite.sprite.isFliped = entry.flipped;
			}

			return colors;
		}

		public static Texture2D PackTextures(Texture2D[] textures, List<ExtendedSpriteDefinition> extendedSpriteDefinitions, ref List<SpriteTexture> sprites, float pixelSize, int paddingX, int paddingY)
		{
			sprites = new List<SpriteTexture>();

			for(int i = 0; i < textures.Length; i++)
			{
				var sprite = new SpriteTexture(textures[i], pixelSize, extendedSpriteDefinitions[i]);

				if(extendedSpriteDefinitions[i] != null && extendedSpriteDefinitions[i].isTrimed)
				{
					sprite.Trim();
				}

				sprites.Add(sprite);
			}

			var atlasBuilder = new Builder(4096, 4096, 1, true, false);

			foreach (var sprite in sprites)
			{
				int extraPadding = sprite.sprite.extendedDefinition != null ? (sprite.sprite.extendedDefinition.isTiled ? 1 : 0) : 0;

				atlasBuilder.AddRect(sprite.width + (paddingX + extraPadding) * 2, sprite.height + (paddingY + extraPadding) * 2);
			}

			if(atlasBuilder.Build() != 0)
			{
				Debug.LogError("Error While Packing");

				return null;
			}

			var atlasData = atlasBuilder.GetAtlasData();

			var texture = new Texture2D(atlasData[0].width, atlasData[0].height, TextureFormat.ARGB32, false, false);
			
			var colors = CreateTexture(atlasData[0], sprites, paddingX, paddingY);
			
			texture.SetPixels(colors);
			texture.Apply();

			return texture;
		}
	}
}