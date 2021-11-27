using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius")]
public class StayInRadiusBehavior : FlockBehaviour
{
    public Vector3 center;
    public float radius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // Check agent distance from center
        Vector3 centerOffset = center - agent.transform.position;
        float t = centerOffset.sqrMagnitude / (radius*radius); // radius*radius to use sqrMagnitude instead Magnitude

        // If we are on the 90% from center need to do nothing (0.9 * 0.9 to use sqrMagnitude instead Magnitude)
        if (t < 0.9f * 0.9f)
        {
            return Vector3.zero;
        }

        return centerOffset * t;
    }
}
