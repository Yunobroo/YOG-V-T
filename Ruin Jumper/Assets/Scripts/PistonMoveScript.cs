using UnityEngine;

public class PistonMoveScript : MonoBehaviour
{
         [Header("Move Settings")]
    public float moveSpeed = 10f;
    public float acceleration = 5f; //WIP
    public bool waitOnPlayer = false;
    public bool loop = true; //WIP
    public float waitTime = 1f;
    public Transform[] waypoints;

    int m_CurrentWaypointIndex;
    bool loopFin = true;
    
    void FixedUpdate ()
    {
        if (loop || loopFin)
        {
            Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
            Vector3 currentToTarget = currentWaypoint.position - transform.position;

            if (currentToTarget.magnitude < 0.1f)
            {
                m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            }

            if (currentWaypoint == waypoints[waypoints.Length - 1]) loopFin = false;

            transform.position = transform.position + currentToTarget.normalized * moveSpeed * Time.deltaTime;
        }
    }
}

