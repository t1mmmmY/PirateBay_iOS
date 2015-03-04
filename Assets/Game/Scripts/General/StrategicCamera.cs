using UnityEngine;
using System.Collections;

public class StrategicCamera : BaseSingleton<StrategicCamera>
{
	[SerializeField] Camera currentCamera;

	public float scaleSpeed   = 0.2f;
	public float dragSpeed = 0.2f;
	public float verticalAngleSpeed = 1.0f;
	public float horizontalAngleSpeed = 0.2f;
	private float magicDPInumber = 317;

	public Rect  limits		   = new Rect(-30, -30, 30, 30);

	public float minOrto = 1.5f;
	public float maxOrto = 7.5f;

	public float minZ = 1.5f;
	public float maxZ = 7.5f;

	public bool isRotating = true;
	public float minAngle = 20.0f;
	public float maxAngle = 80.0f;

	Vector3 startMousePosition;
	Vector3 startPosition;

	bool isSwiping = false;


	void OnEnable()
	{
		if (Screen.dpi != 0)
		{
			scaleSpeed *= magicDPInumber / Screen.dpi;
			dragSpeed *= magicDPInumber / Screen.dpi;
			verticalAngleSpeed *= magicDPInumber / Screen.dpi;
			horizontalAngleSpeed *= magicDPInumber / Screen.dpi;
		}

		TouchController.OnScale += OnScale;
		TouchController.OnDrag  += OnDrag;
		if (isRotating)
		{
			TouchController.OnRotate3D += OnRotate3D;
			//TouchController.OnRotate += OnRotate;
		}
	}


	void OnRotate(float angle)
	{
		angle *= verticalAngleSpeed;
		this.transform.Rotate(Vector3.up, angle, Space.World);
	}

	void OnRotate3D (Vector2 angle)
	{
		angle.x *= verticalAngleSpeed;
		angle.y *= horizontalAngleSpeed;

		this.transform.Rotate(Vector3.up, angle.x, Space.World);

		float newAngle = this.transform.eulerAngles.x - angle.y;

		//Max angle bounds
		if (angle.y < 0 && newAngle > maxAngle)
		{
			angle.y = this.transform.eulerAngles.x - maxAngle;
		}
		else if (angle.y > 0 && newAngle < minAngle)
		{
			angle.y = this.transform.eulerAngles.x - minAngle;
		}

		this.transform.Rotate(Vector3.right, -angle.y, Space.Self);


	}


	void OnDisable()
	{
		TouchController.OnScale -= OnScale;
		TouchController.OnDrag -= OnDrag;
		if (isRotating)
		{
			TouchController.OnRotate3D -= OnRotate3D;
			//TouchController.OnRotate -= OnRotate;
		}

		isSwiping = false;
	}

	void OnScale(float scale)
	{
		if(currentCamera.orthographic)
		{
			currentCamera.orthographicSize -= scale * 5;

			currentCamera.orthographicSize = Mathf.Clamp(currentCamera.orthographicSize, minOrto, maxOrto);
		}
		else
		{
			float z = -currentCamera.transform.localPosition.z;

			z -= scale * 100 * Time.deltaTime * scaleSpeed;

			z = Mathf.Clamp(z, minZ, maxZ);

			currentCamera.transform.localPosition = new Vector3(0, 0, -z);
		}
	}

	public Vector3 MouseTerrainOffset(Vector3 screenPoint)
	{
		var matrix = new Matrix4x4();
		
		matrix.SetTRS(Vector3.zero, Camera.main.transform.rotation, Vector3.one);
		
		var ray = new Ray(matrix.MultiplyPoint((screenPoint) / Screen.height * 2 * Camera.main.orthographicSize), Camera.main.ScreenPointToRay(Input.mousePosition).direction);
		var Plane = new Plane(Vector3.up, Vector3.zero);
		
		float hit = 0;
		
		Plane.Raycast(ray, out hit);

		return ray.GetPoint(hit);;
	}

	public Vector3 MouseTerrainPosition(Vector3 screenPoint)
	{
		var ray = Camera.main.ScreenPointToRay(screenPoint);
		var Plane = new Plane(Vector3.up, Vector3.zero);
		
		float hit = 0;
		
		Plane.Raycast(ray, out hit);

		return ray.GetPoint(hit);;
	}

	void OnDrag(Vector3 newPos, Vector3 prevPos)
	{
		var mousePos = MouseTerrainOffset(newPos - prevPos);

		transform.position -= mousePos * dragSpeed;

		transform.position = new Vector3(Mathf.Clamp(transform.position.x, limits.x, limits.width), 0, Mathf.Clamp(transform.position.z, limits.y, limits.height));
	}
}
