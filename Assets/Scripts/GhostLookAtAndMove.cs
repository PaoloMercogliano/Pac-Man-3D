using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostLookAtAndMove : MonoBehaviour
{

    public GameObject ghostRef;
    public float speedMin;
    public float speedMax;

    private float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.LookAt(ghostRef.transform);
        if (Vector3.Magnitude(transform.position - ghostRef.transform.position) > 0.4) speed = speedMax;
        if (Vector3.Magnitude(transform.position - ghostRef.transform.position) < 0.1) speed = speedMin;
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
