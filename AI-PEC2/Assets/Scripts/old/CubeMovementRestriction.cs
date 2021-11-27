using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cube Movement Restriction")]
public class CubeMovementRestriction : FlockBehaviour
{
    public float radius = 15f;

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // references
        Vector3 agentPosition = agent.transform.position;

        // Control Variable
        float tolerance = 0.9f;

        // Define center position
        Vector3 center = new Vector3(0f, 0f, 0f); // Center of movement 

        Vector3 limits = new Vector3(25f, 25f, 25f);

        // Offsets
        float offsetX = center.x - agent.transform.position.x;
        float offsetY = center.y - agent.transform.position.y;
        float offsetZ = center.z - agent.transform.position.z;

        // X
        if (offsetX  < tolerance && offsetX  > -tolerance)
        {
            return Vector3.zero;
        }



        // Check agent distance from center
        Vector3 centerOffset = center - agent.transform.position;
        float t = centerOffset.sqrMagnitude / (radius * radius); // radius*radius to use sqrMagnitude instead Magnitude

        // If we are on the 90% from center need to do nothing (0.9 * 0.9 to use sqrMagnitude instead Magnitude)
        if (t < 0.9f * 0.9f)
        {
            return Vector3.zero;
        }

        return Vector3.zero; //centerOffset * t * t;
    }
}
