using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	public float speed = 10;

	void FixedUpdate () {
		if (Input.GetKey(KeyCode.LeftShift))
		{
			speed = 3;
		}
		if (Input.GetKey(KeyCode.W))
		{
			gameObject.transform.position += gameObject.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.S))
		{
			gameObject.transform.position -= gameObject.transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.A))
		{
			gameObject.transform.position -= gameObject.transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.D))
		{
			gameObject.transform.position += gameObject.transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Space))
        {
			gameObject.transform.position = new Vector3(gameObject.transform.position.x,
				gameObject.transform.position.y + speed * Time.deltaTime,
				gameObject.transform.position.z);
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
			gameObject.transform.position = new Vector3(gameObject.transform.position.x,
				gameObject.transform.position.y - 2 * speed * Time.deltaTime,
				gameObject.transform.position.z);
		}
	}
}