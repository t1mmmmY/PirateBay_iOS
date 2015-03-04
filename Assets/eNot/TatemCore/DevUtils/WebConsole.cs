using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConsoleMessage
{
	public string log;
	
	public string stack;
	
	public LogType type;
	
	public string time;
}

public class WebConsole : MonoBehaviour
{
	static WebConsole instance;
	
	static List<ConsoleMessage> log = new List<ConsoleMessage>();
	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
		
		Application.RegisterLogCallback (Print);
	}
	
	public static void Init()
	{
		if(instance == null)
		{
			GameObject go = new GameObject("WebConsole");

			instance = go.AddComponent<WebConsole>();
		}
	}
	
	public static void Print(string logString, string stackTrace, LogType logType)
	{
		log.Add(new ConsoleMessage(){log = logString, stack = stackTrace, type = logType, time = System.DateTime.Now.ToString("hh:mm:ss")});
	}
		
	public static void Print(object message)
	{
		Print(message.ToString(), "", LogType.Log);
	}
	
	bool isHided = true;
	float scrollPosition = 0;
	
	ConsoleMessage expandedMessage;
	
	float logWidth = 400;
	
	float showButtonWidth = 70;
	
	void OnGUI()
	{		
		int count = isHided ? Mathf.Min(1, log.Count) : Mathf.Min(10, log.Count);
		
		Rect consoleRect = new Rect(0, Screen.height - count * 20, logWidth + 40 + showButtonWidth, count * 20);

		GUI.Box(consoleRect, "");
		for(int i = 0; i < count; i++)
		{
			ConsoleMessage message = log[log.Count - 1 - i - Mathf.FloorToInt(scrollPosition)];
			
			if(message.type == LogType.Error || message.type == LogType.Exception)
			{
				GUI.color = Color.red;
			}
			else if(message.type == LogType.Warning)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.white;
			}
			
			GUI.TextField(new Rect(0, Screen.height - count * 20 + i * 20, 70, 20), message.time);
			GUI.TextField(new Rect(70, Screen.height - count * 20 + i * 20, logWidth - 70, 20), message.log);
			
			if(GUI.Button(new Rect(logWidth, Screen.height - count * 20 + i * 20, 20, 20), "X"))
			{
				if(message == expandedMessage)
				{
					expandedMessage = null;
				}
				log.RemoveAt(log.Count - 1 - i);
				
				break;
			}
			if(GUI.Button(new Rect(logWidth + 20, Screen.height - count * 20 + i * 20, 20, 20), ">"))
			{
				expandedMessage = message;
			}
		}
		
		GUI.color = Color.white;
		
		if(GUI.Button(new Rect(logWidth + 40, Screen.height - count * 20, showButtonWidth, 20), isHided ? ("Show" + (((log.Count - 1) == 0) ? "" : "(" + (log.Count - 1) + ")")) : "Hide"))
		{
			isHided = !isHided;
		}
		
		if(GUI.Button(new Rect(logWidth + 40, Screen.height - count * 20 + 20, showButtonWidth, 20), "Clear"))
		{
			log.Clear();
			
			expandedMessage = null;
			
			isHided = true;
			
			scrollPosition = 0;
			
			return;
		}
		
		if(!isHided)
		{
			if(log.Count > count)
			{
				scrollPosition = GUI.VerticalScrollbar(new Rect(logWidth + 40, Screen.height - count * 20 + 40, 20, count * 20 - 40), scrollPosition, log.Count - 1 - (log.Count - count), 0, log.Count - 1);
			}
			else
			{
				scrollPosition = 0;
			}
		}
		else
		{
			scrollPosition = 0;	
		}
		
		if(expandedMessage != null)
		{
			if(expandedMessage.type == LogType.Error || expandedMessage.type == LogType.Exception)
			{
				GUI.color = Color.red;
			}
			else if(expandedMessage.type == LogType.Warning)
			{
				GUI.color = Color.yellow;
			}
			else
			{
				GUI.color = Color.white;
			}
			
			Rect stackRect = new Rect(logWidth + 40 + showButtonWidth, Screen.height - 200, 450, 200);

			GUI.Box(stackRect, "");

			GUI.TextArea(new Rect(logWidth + 40 + showButtonWidth, Screen.height - 200, 450 - 20, 200), expandedMessage.log + "(" + expandedMessage.time + ")" + "\n" + expandedMessage.stack);
			
			if(GUI.Button(new Rect(logWidth + 40 + showButtonWidth + 450 - 20, Screen.height - 200, 20, 20), "<"))
			{
				expandedMessage = null;
			}
		}
		
		GUI.color = Color.white;
	}
}
