using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCameraRotation : MonoBehaviour
{
	public Vector3 offset;

	public Vector3 point     = new Vector3(0.0f, 0.0f, 0.0f); // point of rotation
	public float sensitivity = 10f;                           // speed of rotation

	public float zoomMax     = 12;                            // max zoom
	public float zoomMin     = 4;                             // min zoom

	private float X, Y;
	private int direction;

	private Camera cam;

	void Start()
	{
		direction = 1;
		Y = -25.0f;

		offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax) / 2);
		transform.position = point + offset;

		cam = gameObject.GetComponent<Camera>();
	}

	void Update()
	{
		X = transform.localEulerAngles.y + sensitivity * Time.deltaTime;
		if (Y <= -35.0f || Y >= -15.0f)
        {
			direction = -direction;
        }
		Y += sensitivity * direction * Time.deltaTime / 10.0f;
		transform.localEulerAngles = new Vector3(-Y, X, 0);

		offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));
		transform.position = transform.localRotation * offset + point;
	}
}
