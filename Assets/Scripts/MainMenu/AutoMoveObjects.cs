using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;

public class AutoMoveObjects : MonoBehaviour
{
    public float maxX;
    public float minX;
    public float direction;

    private static int count = 0;
    
    private float speed;
    private float y;
    private float z;

    private int limit;
    private bool canMove;

    private IEnumerator DelayResetCount()
    {
        yield return new WaitForSeconds(0.1f);

        count = 0;
    }

    void Start()
    {
        count = 0;

        speed = 3f;

        y = transform.position.y;
        z = transform.position.z;

        limit = 16;
        canMove = true;
    }

    void Update()
    {
        if (count >= limit)
        {
            StartCoroutine(DelayResetCount());
            canMove = true;
            transform.position = new Vector3(transform.position.x + speed * direction * Time.deltaTime, y, z);
            return;
        }

        if (!canMove)
        {
            return;
        }

        float x = transform.position.x;

        if (x >= maxX)
        {
            x = maxX;
            direction = -direction;
            count++;
            canMove = !canMove;
            transform.position = new Vector3(x, y, z);
            return;
        }
        else if (x <= minX)
        {
            x = minX;
            direction = -direction;
            count++;
            canMove = !canMove;
            transform.position = new Vector3(x, y, z);
            return;
        }

        transform.position = new Vector3(x + speed * direction * Time.deltaTime, y, z);
    }
}
