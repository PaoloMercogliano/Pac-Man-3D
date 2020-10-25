using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    // Ghost speed
    public float speed;
    // Min distance to allow walking
    public float distanceToTurn;
    // Scene controller
    public SceneController sceneController;

    // These variables will hold what we find in each direction (p = player, w = wall)
    private char nObj;
    private char eObj;
    private char sObj;
    private char wObj;

    // Distances to the objects
    private float nDist;
    private float eDist;
    private float sDist;
    private float wDist;

    // Current moving direction
    private Vector3 movingDir = new Vector3(0, 0, -1);

    // Movement vectors
    private Vector3 north = new Vector3(0, 0, 1);
    private Vector3 est = new Vector3(1, 0, 0);
    private Vector3 south = new Vector3(0, 0, -1);
    private Vector3 west = new Vector3(-1, 0, 0);

    // Update is called once per frame
    void FixedUpdate()
    {
        // f.false n.north e.est s.south w.west
        char canSeePlayer = 'f';
        // direction in which the ghost sees the player
        Vector3 playerDir = new Vector3(0, 0, 0);

        // Bit shift the index of the layer (2) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 2.
        // But instead we want to collide against everything except layer 2. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects north
        if (Physics.Raycast(transform.position, north, out hit, Mathf.Infinity, layerMask))
        {
            // You can use these functions to debug the rays
            // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            // Debug.Log("Hit north");
            nDist = hit.distance;
            switch (hit.transform.tag)
            {
                case "Player":
                    nObj = 'p';
                    canSeePlayer = 'n';
                    playerDir = north;
                    break;
                case "Wall":
                    nObj = 'w';
                    break;
            }
        }

        // Does the ray intersect any objects est
        if (Physics.Raycast(transform.position, est, out hit, Mathf.Infinity, layerMask))
        {
            eDist = hit.distance;
            switch (hit.transform.tag)
            {
                case "Player":
                    eObj = 'p';
                    canSeePlayer = 'e';
                    playerDir = est;
                    break;
                case "Wall":
                    eObj = 'w';
                    break;
            }
        }

        // Does the ray intersect any objects south
        if (Physics.Raycast(transform.position, south, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            sDist = hit.distance;
            switch (hit.transform.tag)
            {
                case "Player":
                    sObj = 'p';
                    canSeePlayer = 's';
                    playerDir = south;
                    break;
                case "Wall":
                    sObj = 'w';
                    break;
            }
        }

        // Does the ray intersect any objects west
        if (Physics.Raycast(transform.position, west, out hit, Mathf.Infinity, layerMask))
        {
            wDist = hit.distance;
            switch (hit.transform.tag)
            {
                case "Player":
                    wObj = 'p';
                    canSeePlayer = 'w';
                    playerDir = west;
                    break;
                case "Wall":
                    wObj = 'w';
                    break;
            }
        }

        // if player is visible and the ghost is chasing
        if (sceneController.IsPlayerChased() && canSeePlayer != 'f')
        {
            movingDir = playerDir;
        }   // if player is visible and the player is chasing
        else if (!sceneController.IsPlayerChased() && canSeePlayer != 'f')
        {
            movingDir = -playerDir;
        }   // the player is not visible
        else
        {
            switch (getMovingDirInChar())
            {
                case 'n':
                    if (nDist > distanceToTurn)
                    {
                        movingDir = north;
                        break;
                    }
                    else
                    { 
                        if (sDist > distanceToTurn) movingDir = south;
                        if (Random.Range(0, 10) > 4) {
                            if (eDist > distanceToTurn) movingDir = est;
                            if (wDist > distanceToTurn) movingDir = west;
                        } else {
                            if (wDist > distanceToTurn) movingDir = west;
                            if (eDist > distanceToTurn) movingDir = est;
                        } 
                        break;
                    }
                case 'e':
                    if (eDist > distanceToTurn)
                    {
                        movingDir = est;
                        break;
                    }
                    else
                    {
                        if (wDist > distanceToTurn) movingDir = west;
                        if (Random.Range(0, 10) > 4)
                        {
                            if (sDist > distanceToTurn) movingDir = south;
                            if (nDist > distanceToTurn) movingDir = north;
                        }
                        else
                        {
                            if (nDist > distanceToTurn) movingDir = north;
                            if (sDist > distanceToTurn) movingDir = south;
                        }                        
                        break;
                    }
                case 's':
                    if (sDist > distanceToTurn)
                    {
                        movingDir = south;
                        break;
                    }
                    else
                    {
                        if (nDist > distanceToTurn) movingDir = north;
                        if (Random.Range(0, 10) > 4)
                        {
                            if (eDist > distanceToTurn) movingDir = est;
                            if (wDist > distanceToTurn) movingDir = west;
                        }
                        else
                        {
                            if (wDist > distanceToTurn) movingDir = west;
                            if (eDist > distanceToTurn) movingDir = est;
                        }
                        break;
                    }
                case 'w':
                    if (wDist > distanceToTurn)
                    {
                        movingDir = west;
                        break;
                    }
                    else
                    {
                        if (eDist > distanceToTurn) movingDir = est;
                        if (Random.Range(0, 10) > 4)
                        {
                            if (sDist > distanceToTurn) movingDir = south;
                            if (nDist > distanceToTurn) movingDir = north;
                        }
                        else
                        {
                            if (nDist > distanceToTurn) movingDir = north;
                            if (sDist > distanceToTurn) movingDir = south;
                        }
                        break;
                    }
            }
        }

        // Do the actual movement
        transform.position += movingDir * speed * Time.deltaTime;
    }

    private char getMovingDirInChar()
    {
        if (movingDir == north) return 'n';
        if (movingDir == est) return 'e';
        if (movingDir == south) return 's';
        if (movingDir == west) return 'w';
        Debug.Log("Error in getMovingDirInChar() - probably due to vector precision");
        return 'f';
    }

}