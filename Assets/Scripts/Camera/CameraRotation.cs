using UnityEngine;
using System.Collections;
using System.Threading;

public class CameraRotation : MonoBehaviour
{
    private bool FreeCamera = false;

    public enum RotationAxes
    {
        mouseXAndY = 0,
        mouseX = 1,
        mouseY = 2
    }

    public RotationAxes axes = RotationAxes.mouseXAndY;

    public float sensHor = 9.0f;
    public float sensVert = 9.0f;

    public float minVert = -45.0f;
    public float maxVert = 45.0f;

    private float _rotationX = 0;

    public void SetFreeCamera()
    {
        FreeCamera = true;
    }

    void Start()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FreeCamera = !FreeCamera;
        }

        if (FreeCamera)
        {
            if (axes == RotationAxes.mouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensVert, 0);
            }
            else if (axes == RotationAxes.mouseY)
            {
                _rotationX -= Input.GetAxis("Mouse Y") * sensVert;
                _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert);

                float rotationY = transform.localEulerAngles.y;

                transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
            }
            else
            {
                _rotationX -= Input.GetAxis("Mouse Y") * sensVert;
                _rotationX = Mathf.Clamp(_rotationX, minVert, maxVert);

                float delta = Input.GetAxis("Mouse X") * sensHor;
                float rotationY = transform.localEulerAngles.y + delta;

                transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
            }
        }
    }
}
