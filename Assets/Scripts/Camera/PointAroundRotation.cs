using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAroundRotation : MonoBehaviour
{
	public Vector3 offset;

	public Vector3 point     = new Vector3(-61.5f, 4.57f, 5f); // point of rotation
	public float sensitivity = 3;                              // mouse sensitivity
	public float limit       = 80;                             // limit for Y rotation
	public float zoom        = 0.4f;                           // sensitivity for zoom
	public float zoomMax     = 12;                             // max zoom
	public float zoomMin     = 4;                              // min zoom

	private float X, Y;

	void Start()
	{
		offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax) / 2);
		transform.position = point + offset;
	}

	void Update()
	{
		// rotation around point
		if (Input.GetKey(KeyCode.Mouse2)) // pressed middle click
		{
			X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
			Y += Input.GetAxis("Mouse Y") * sensitivity;
			Y = Mathf.Clamp(Y, -limit, 0);

			transform.localEulerAngles = new Vector3(-Y, X, 0);
		}

		// zoom
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			offset.z += zoom;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			offset.z -= zoom;
		}
		offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
		transform.position = transform.localRotation * offset + point;
	}
}
