using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistSensor : MonoBehaviour
{
    public float dist;
    public int rayLen;
    void Start() {
        rayLen = 100;
    }

    void Update() {
            float cX = this.transform.position.x;
            float cY = this.transform.position.y - this.transform.localScale.y / 2;
            float cZ = this.transform.position.z;

            Vector3 centre = new Vector3(cX, cY, cZ); // Center of bottom edge of cube;
            Vector3 end = this.transform.TransformDirection(-Vector3.up) * rayLen; // End point

            Debug.DrawRay(centre, end, Color.green);

            Ray r = new Ray(centre, end);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(r, out hit, rayLen))
            {
                dist = hit.distance;
            }
        }
}
