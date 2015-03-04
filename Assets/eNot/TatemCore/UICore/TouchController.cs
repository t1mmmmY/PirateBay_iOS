using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchController : BaseSingleton<TouchController>
{
	public enum TouchType { MOUSE_BUTTON, FINGER_TOUCH }
	
	public float fastClickTime = 0.4f;
	public float doubledClickTime = 0.3f;
	public float thresholdRange = 10.0f;
	
	protected List<TouchData> _touchList = new List<TouchData>();
	
	private List<TouchData> _doubleClickHash = new List<TouchData>();
	
	// event
	//public delegate void OnTouchDelegate(TouchData[] touchData);
	
	public delegate void OnChangeCountClickDelegate(int count);
	public static event OnChangeCountClickDelegate OnChangeCountClick;
	
	public delegate void OnDoubleClickDelegate(Vector3 position);
	public static event OnDoubleClickDelegate OnDoubleClick;
	
	public delegate void OnSingleClickDelegate(Vector3 position);
	public static event OnSingleClickDelegate OnSingleClick;
	
	public delegate void OnSingleDragDelegate(Vector3 position, Vector3 privPosition);
	public static event OnSingleDragDelegate OnDrag;
	
	public delegate void OnDoubleRotateDelegate(float angle);
	public static event OnDoubleRotateDelegate OnRotate;
	
	public delegate void OnDoubleRotateDelegate3D(Vector2 angle);
	public static event OnDoubleRotateDelegate3D OnRotate3D;
	
	public delegate void OnDoubleScaleDelegate(float scale);
	public static event OnDoubleScaleDelegate OnScale;
	
	public delegate void OnHoldClickDelegate(Vector3 position, float time);
	public static event OnHoldClickDelegate OnHold;
	
	public delegate void OnReleaseClickDelegate( Vector3 position );
	public static event OnReleaseClickDelegate OnRelease;
	
	// Update is called once per frame
	void Update () 
	{
		#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		if (Input.touchCount > 0)
		{
			// get change
			for(int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				
				if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
				{
					TouchData touchData = SearchTouch(touch.fingerId, TouchType.FINGER_TOUCH);
					if(touchData == null)
					{
						touchData = new TouchData();
						touchData.SetTypeTouch(TouchType.FINGER_TOUCH, touch.fingerId);
						touchData.SetStartPosition(touch.position);
						
						AddTouch(touchData);
					}
					touchData.AddTime(Time.deltaTime);
				}
				else
				{
					// update
					TouchData touchData = SearchTouch(touch.fingerId, TouchType.FINGER_TOUCH);
					touchData.AddTime(Time.deltaTime);
					
					if (touch.phase == TouchPhase.Moved)
					{
						touchData.SetNewPosition(touch.position);
					}
					if (touch.phase == TouchPhase.Ended)
					{
						RemoveTouch(touchData);
					}
					if (touch.phase == TouchPhase.Canceled)
					{
						// WARNING
						RemoveTouch(touchData);
					}
				}
			}
		}
		#else
		
		#region MOVE
		//for(int i = 0; i < 3; i++)
		//{
		int i = 0;
		
		if(Input.GetMouseButtonDown(i))
		{
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			if(touchData == null)
			{
				touchData = new TouchData();
				touchData.SetTypeTouch(TouchType.MOUSE_BUTTON, i);
				touchData.SetStartPosition(Input.mousePosition);
				
				AddTouch(touchData);
			}
		}
		else if(Input.GetMouseButton(i))
		{
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			touchData.AddTime(Time.deltaTime);
			touchData.SetNewPosition(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(i))
		{
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			touchData.AddTime(Time.deltaTime);
			
			RemoveTouch(touchData);
		}
		//}
		#endregion
		
		#region ROTATE
		i = 1;
		if(Input.GetMouseButtonDown(i))
		{
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			if(touchData == null)
			{
				touchData = new TouchData();
				touchData.SetTypeTouch(TouchType.MOUSE_BUTTON, i);
				touchData.SetStartPosition(Input.mousePosition);
				
				AddTouch(touchData);

			}
		}
		else if(Input.GetMouseButton(i))
		{
			Vector3 shift = Vector3.zero;
			
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			touchData.AddTime(Time.deltaTime);
			touchData.SetNewPosition(Input.mousePosition);
		}
		else if(Input.GetMouseButtonUp(i))
		{
			TouchData touchData = SearchTouch(i, TouchType.MOUSE_BUTTON);
			touchData.AddTime(Time.deltaTime);
			
			RemoveTouch(touchData);
		}
		#endregion
		
		
		#endif
		UpdateDoubleTap(Time.deltaTime);
		
		UseBaseEvents();
	}
	
	private TouchData SearchTouch ( int id, TouchType touchType)
	{
		int count = _touchList.Count;
		for(int i=0; i < count; i++)
		{
			TouchData td = _touchList[i];
			if(td.touchType == touchType && td.touchID == id)
				return td;
		}
		return null;
	}
	
	protected virtual void AddTouch(TouchData touchData)
	{
		_touchList.Add(touchData);
		
		if(OnChangeCountClick != null)
			OnChangeCountClick(_touchList.Count);
	}
	
	protected virtual void RemoveTouch(TouchData touchData)
	{
		if(_touchList.Count == 1)
			IsSingleClick(touchData);
		
		if(OnRelease != null)
			OnRelease( touchData.position );
		
		_touchList.Remove(touchData);
		
		if(OnChangeCountClick != null)
			OnChangeCountClick(_touchList.Count);
	}
	
	protected virtual void IsSingleClick(TouchData touchData)
	{
		// lock first tap for double tap
		if( touchData.time < doubledClickTime && thresholdRange > Vector2.Distance( new Vector2(touchData.startPosition.x, touchData.startPosition.y), new Vector2(touchData.position.x, touchData.position.y) ))
		{
			_doubleClickHash.Add( touchData );
			CheckDoubleTap();
		}
		else if(touchData.time < fastClickTime && thresholdRange > Vector2.Distance( new Vector2(touchData.startPosition.x, touchData.startPosition.y), new Vector2(touchData.position.x, touchData.position.y) ))
		{
			if( OnSingleClick != null ) OnSingleClick(touchData.position);
			//Debug.Log( "SINGLE CLICK 1 : " + touchData.position.x.ToString() + " " + touchData.position.y.ToString() + " " + touchData.position.z.ToString() ); 
		}
	}
	
	protected virtual void CheckDoubleTap()
	{
		TouchData[] doubleClickHashArray = _doubleClickHash.ToArray();
		foreach( TouchData touchData in doubleClickHashArray )
		{
			TouchData[] hashArray = _doubleClickHash.ToArray();
			foreach( TouchData touchData2 in hashArray )
			{
				if( touchData != touchData2 && thresholdRange > Vector2.Distance( new Vector2(touchData2.position.x, touchData2.position.y), new Vector2(touchData.position.x, touchData.position.y) ))
				{
					if( OnDoubleClick != null ) OnDoubleClick( Vector3.Lerp( touchData2.position, touchData.position, 0.5f) );
					//Vector3 pos = Vector3.Lerp( touchData2.position, touchData.position, 0.5f);
					//Debug.Log( "DOUBLE CLICK : " + pos.x.ToString() + " " + pos.y.ToString() + " " + pos.z.ToString() ); 
					_doubleClickHash.Remove( touchData );
					_doubleClickHash.Remove( touchData2 );
					return;
				}
			}
		}
	}
	
	protected virtual void UpdateDoubleTap( float delta )
	{
		TouchData[] doubleClickHashArray = _doubleClickHash.ToArray();
		foreach( TouchData touchData in doubleClickHashArray )
		{
			touchData.AddTime( delta );
			
			if( touchData.time > fastClickTime )
			{
				if( OnSingleClick != null ) OnSingleClick(touchData.position);
				//Debug.Log( "SINGLE CLICK 2 : " + touchData.position.x.ToString() + " " + touchData.position.y.ToString() + " " + touchData.position.z.ToString() ); 
				_doubleClickHash.Remove( touchData );
			}
		}
	}
	
	protected virtual void UseBaseEvents()
	{
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			if(OnScale != null)
			{
				OnScale(Input.GetAxis("Mouse ScrollWheel"));
			}
		}
		
		if(_touchList.Count == 1)
		{
			//event OnSingleDragDelegate OnDrag;
			if(OnDrag != null)
			{
				TouchData touchData = _touchList[0];
				if (touchData.touchID == 0) //Left mouse button click
				{
					Vector3 dif = touchData.position - touchData.privPosition;
					if(  Vector3.Distance( new Vector3(0,0,0), dif) > 0.0f )
						OnDrag( touchData.position, touchData.privPosition);
				}
			}
			
			//event OnHoldClickDelegate OnHold
			if(OnHold != null)
			{
				TouchData touchData = _touchList[0];
				OnHold( touchData.position, touchData.time );
			}
			
			#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
			if(OnRotate != null)
			{
				TouchData touchData = _touchList[0];
				if (touchData.touchID == 1) //Right mouse button click
				{
					float angle = touchData.position.x - touchData.privPosition.x;
					if(angle != 0)
						OnRotate(angle);
				}
				
			}
			if(OnRotate3D != null)
			{
				TouchData touchData = _touchList[0];
				if (touchData.touchID == 1) //Right mouse button click
				{
					Vector2 angle = touchData.position - touchData.privPosition;
					if(angle != Vector2.zero)
						OnRotate3D(angle);
				}
			}
			#endif
		}
		
		if(_touchList.Count == 2)
		{
			TouchData touchData1 = _touchList[0];
			TouchData touchData2 = _touchList[1];

			//2 finger drag
//			if(OnDrag != null)
//			{
//				TouchData touchData = _touchList[0];
//				Vector3 dif = touchData.position - touchData.privPosition;
//				if(  Vector3.Distance( new Vector3(0,0,0), dif) > 0.0f )
//					OnDrag( touchData.position, touchData.privPosition);
//			}
			
			//event OnDoubleScaleDelegate OnScale;
			if(OnScale != null)
			{
				float privScale = Vector3.Distance(touchData1.privPosition, touchData2.privPosition);
				float currentScale = Vector3.Distance(touchData1.position, touchData2.position);			
				
				if(currentScale - privScale != 0.0f)
					OnScale((currentScale - privScale) / Camera.main.pixelHeight);
			}

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
			//event OnDoubleRotateDelegate OnRotate;
			if(OnRotate != null)
			{
				Vector3 privRot = touchData1.privPosition - touchData2.privPosition;
				Vector3 currentRot = touchData1.position - touchData2.position;
				
				float angle = Vector3.Angle(privRot, currentRot);
				float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(currentRot, privRot))); 
				
				if(angle != 0)
					OnRotate(-angle*sign);
			}


			if(OnRotate3D != null)
			{
				Vector3 privRot = touchData1.privPosition - touchData2.privPosition;
				Vector3 currentRot = touchData1.position - touchData2.position;
				
				float angle = Vector3.Angle(privRot, currentRot);
				float sign = Mathf.Sign(Vector3.Dot(Vector3.forward, Vector3.Cross(currentRot, privRot))); 


				float touch1DeltaY = touchData1.position.y - touchData1.privPosition.y;
				float touch2DeltaY = touchData2.position.y - touchData2.privPosition.y;
				float heightAngle = 0.0f;

				if (touch1DeltaY * touch2DeltaY > 0)
				{
					heightAngle = (touch1DeltaY + touch2DeltaY) / 2.0f;
				}
				
				if(angle != 0)
					OnRotate3D(new Vector2(-angle*sign, heightAngle));
			}
#endif
		}
	}
}

public class TouchData {
	public Vector3 position;
	public Vector3 privPosition;
	public Vector3 startPosition;
	
	public float trackTouch;
	public float time;
	
	public TouchController.TouchType touchType;
	public int touchID;
	
	public void SetNewPosition(Vector3 newPosition)
	{
		this.privPosition = this.position;
		this.position = newPosition;
		trackTouch += Vector3.Distance(this.privPosition, newPosition);
	}
	
	public void SetStartPosition(Vector3 startPosition)
	{
		this.privPosition = startPosition;
		this.startPosition = startPosition;
		this.position = startPosition;
		
		time = 0.0f;
		trackTouch = 0.0f;
	}
	
	public void SetTypeTouch(TouchController.TouchType newTouchType, int id)
	{
		this.touchType = newTouchType;
		this.touchID = id;
	}
	
	public void AddTime(float addTime)
	{
		time += addTime;
		this.privPosition = this.position;
	}
}
