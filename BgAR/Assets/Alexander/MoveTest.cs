using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public List<Transform> waypoints;
    public float moveSpeed = 5f;
    public float bounceHeight = 1f;
    public float bounceDuration = 1f;

    private int currentWaypointIndex;
    private float originalYPosition;

    void Start()
    {
        if (waypoints.Count < 2)
        {
            Debug.LogError("Not enough waypoints found!");
            return;
        }

        // Set the initial position to the first waypoint
        transform.position = waypoints[0].position;
        originalYPosition = transform.position.y;

        // Start moving towards the first waypoint
        StartCoroutine(BounceBetweenWaypoints());
    }

    IEnumerator BounceBetweenWaypoints()
    {
        while (true)
        {
            Vector3 startPosition = transform.position;
            Vector3 controlPoint = startPosition + Vector3.up * (bounceHeight * 2f); // Adjust multiplier for the curve
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;

            float startTime = Time.time;

            while (Time.time < startTime + bounceDuration)
            {
                float t = (Time.time - startTime) / bounceDuration;
                transform.position = BezierCurve(startPosition, controlPoint, targetPosition, t);
                yield return null;
            }

            // Ensure the object reaches the exact target position
            transform.position = targetPosition;

            // Wait for a moment at the top of the bounce
            yield return new WaitForSeconds(0.5f);

            // Return to the original Y-axis position
            float returnDuration = 0.5f;
            startTime = Time.time;

            while (Time.time < startTime + returnDuration)
            {
                float t = (Time.time - startTime) / returnDuration;
                transform.position = Vector3.Lerp(targetPosition, new Vector3(targetPosition.x, originalYPosition, targetPosition.z), t);
                yield return null;
            }

            // Ensure the object reaches the original Y-axis position
            transform.position = new Vector3(transform.position.x, originalYPosition, transform.position.z);

            // Move to the next waypoint
            MoveToNextWaypoint();
        }
    }

    void MoveToNextWaypoint()
    {
        // Move to the next waypoint in the list
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }

    Vector3 BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }
}
