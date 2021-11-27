using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius From Nest")]
public class StayInRadiusFromNest : FlockBehaviour
{
    public float radius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // Define center position
        Vector3 center = flockManager.transform.position; // Center of movement equals to flockManager positions (nest origin)

        // Check agent distance from center
        Vector3 centerOffset = center - agent.transform.position;
        float t = centerOffset.magnitude / (radius); // radius*radius to use sqrMagnitude instead Magnitude

        // If we are on the 90% from center need to do nothing (0.9 * 0.9 to use sqrMagnitude instead Magnitude)
        if (t < 0.9f * 0.9f)
        {
            return Vector3.zero;
        }

        return centerOffset * t * t;
    }
}
