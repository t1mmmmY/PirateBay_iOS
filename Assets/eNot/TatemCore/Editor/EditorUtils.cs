using System;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorUtils
{
	public static bool ConfigureSpriteTextureImporter(string assetPath)
	{
		// make sure the source texture is npot and readable, and uncompressed
		TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
		if (importer.textureType != TextureImporterType.Advanced ||
		    importer.textureFormat != TextureImporterFormat.AutomaticTruecolor ||
		    importer.npotScale != TextureImporterNPOTScale.None ||
		    importer.isReadable != true ||
		    importer.alphaIsTransparency != true ||
		    importer.maxTextureSize < 4096)
		{
			importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			importer.textureType = TextureImporterType.Advanced;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.isReadable = true;
			importer.mipmapEnabled = false;
			importer.alphaIsTransparency = true;
			importer.maxTextureSize = 4096;
			
			return true;
		}
		
		return false;
	}

	public static void SaveJSON(string assetPath, string json)
	{
		File.WriteAllText(assetPath, json);
		AssetDatabase.Refresh();
	}
	
	public static void SaveTexture(string assetPath, Texture2D texture)
	{	
		Directory.CreateDirectory(Path.GetDirectoryName(assetPath));
		
		File.WriteAllBytes(assetPath, texture.EncodeToPNG());
		AssetDatabase.Refresh();
	}
}