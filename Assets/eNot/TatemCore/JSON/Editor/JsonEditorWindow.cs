using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
	
public enum DataType{Object, Array, String, Int, Float, Long, Double};

public class JsonEditorWindow : EditorWindow
{
	[MenuItem("Strategic/JsonEditor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(JsonEditorWindow));
	}
	
	JSONObject currentData;
	string assetPath;
	string dataSource;
	DateTime time;
	Vector2 scrollPosition;
	DataType dataType;
		
	void OnGUI()
	{
		if(GUILayout.Button("Reload"))
		{
			dataSource  = null;
			currentData = null;
		}
		
		if(currentData == null)
		{
			assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		}
		
		if(assetPath == "")
		{
			GUILayout.Label("Select .conf file");
			
			return;
		}
		
		if(time != File.GetLastWriteTime(assetPath))
		{
			dataSource  = null;
			currentData = null;
		}
		
		time = File.GetLastWriteTime(assetPath);
		string extention = Path.GetExtension(assetPath);
		string fileName  = Path.GetFileName(assetPath);
		
		if(extention == ".conf")
		{
			EditorGUILayout.LabelField(fileName);
			
			if(currentData == null)
			{
				dataSource = File.ReadAllText(assetPath);
				
				currentData = dataSource.ToJsonObject();
			}
		}
		
		if(currentData != null)
		{
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);

			DrawJson(currentData);
			
			GUILayout.EndScrollView();
			
			dataType = (DataType)EditorGUILayout.EnumPopup("New data type", dataType);
			
			File.WriteAllText(assetPath, currentData.ToJson());
		}
	}
	
	void DrawJson(JSONObject data)
	{
		if(data == null)
		{
			return;	
		}
		
		if(data.GetType() == typeof(Dictionary<string, JSONObject>))
		{		
			List<string> keys       = new List<string>();
			List<JSONObject> values = new List<JSONObject>();
			
			OrderDictionary(data.ToDictionary(), keys, values);
			
			EditorGUILayout.BeginVertical();
			for(int i = 0; i < keys.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
					
				GUILayout.Space(16);
				
				if(GUILayout.Button("X", GUILayout.Width(20)))
				{
					keys.RemoveAt(i);
					values.RemoveAt(i);
					
					data.SetObject(UnorderDictionary(keys, values));
					return;
				}
				
				string newKey = EditorGUILayout.TextField(keys[i].ToString());
				
				if(values[i].GetType() == typeof(List<JSONObject>) || values[i].GetType() == typeof(Dictionary<string, JSONObject>))
				{
					if(GUILayout.Button("+", GUILayout.Width(20)))
					{
						if(values[i].GetType() == typeof(Dictionary<string, JSONObject>))
						{
							if(dataType == DataType.Object)
							{
								values[i].AddObject("NewObject");
							}
							else if(dataType == DataType.Array)
							{
								values[i].AddList("NewList");
							}
							else if(dataType == DataType.String)
							{
								values[i].AddItem("NewItem", "");
							}
							else if(dataType == DataType.Int)
							{
								values[i].AddItem("NewItem", 0);
							}
							else if(dataType == DataType.Float)
							{
								values[i].AddItem("NewItem", 0.0);
							}
							else if(dataType == DataType.Double)
							{
								values[i].AddItem("NewItem", 0.0);
							}
							else if(dataType == DataType.Long)
							{
								values[i].AddItem("NewItem", 0);
							}
						}
						else if(values[i].GetType() == typeof(List<JSONObject>))
						{
							if(dataType == DataType.Object)
							{
								//values[i].AddObject();
							}
							else if(dataType == DataType.Array)
							{
								//values[i].AddList("NewList");
							}
							else if(dataType == DataType.String)
							{
								values[i].AddItem("");
							}
							else if(dataType == DataType.Int)
							{
								values[i].AddItem(0);
							}
							else if(dataType == DataType.Float)
							{
								values[i].AddItem(0.0);
							}
							else if(dataType == DataType.Double)
							{
								values[i].AddItem(0.0);
							}
							else if(dataType == DataType.Long)
							{
								values[i].AddItem(0);
							}
						}
					}
					
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					
					GUILayout.Space(16);
				}
				
				if(newKey != keys[i])
				{
					keys[i] = newKey;
					
					data.SetObject(UnorderDictionary(keys, values));
					return;
				}
				
				DrawJson(values[i]);
				
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
			
			data.SetObject(UnorderDictionary(keys, values));
		}
		else if(data.GetType() == typeof(List<JSONObject>))
		{
			EditorGUILayout.BeginVertical();

			for(int i = 0; i < data.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(16);
				if(GUILayout.Button("X", GUILayout.Width(20)))
				{
					data.ToList().RemoveAt(i);
					
					return;
					
				}
				GUILayout.Label("[" + i + "]", GUILayout.Width(36));
				DrawJson(data[i]);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}
		else
		{
			if(data.GetType() == typeof(int))
			{
				data.SetObject(double.Parse(EditorGUILayout.TextField(data.ToString())));
			}
			else if(data.GetType() == typeof(float))
			{
				data.SetObject(double.Parse(EditorGUILayout.TextField(data.ToString())));
			}
			else if(data.GetType() == typeof(long))
			{
				data.SetObject(double.Parse(EditorGUILayout.TextField(data.ToString())));
			}
			else if(data.GetType() == typeof(double))
			{
				data.SetObject(double.Parse(EditorGUILayout.TextField(data.ToString())));
			}
			else
			{
				data.SetObject(EditorGUILayout.TextField(data.ToString()));
			}
		}
	}
	
	void OrderDictionary(Dictionary<string, JSONObject> dic, List<string> keys, List<JSONObject> values)
	{
		foreach(var node in dic)
		{
			keys.Add(node.Key);
			values.Add(node.Value);
		}
	}
	
	Dictionary<string, JSONObject> UnorderDictionary(List<string> keys, List<JSONObject> values)
	{
		Dictionary<string, JSONObject> newDic = new Dictionary<string, JSONObject>();
		
		for(int i = 0; i < keys.Count; i++)
		{
			newDic.Add(keys[i], values[i]);
		}
		
		return newDic;
	}
}