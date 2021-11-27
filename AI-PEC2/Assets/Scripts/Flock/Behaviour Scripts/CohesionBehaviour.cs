using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Cohesion")]
public class CohesionBehaviour : FilteredFlockBehaviour
{
    // This method find middle point between neighbours and tryes to move there.
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // If no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector3.zero;

        // Add all points together and average
        Vector3 cohesionMove = Vector3.zero;

        // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            cohesionMove += item.position;
        }
        // need average => middle point
        cohesionMove /= context.Count;

        // Create offset from agent position
        cohesionMove -= agent.transform.position;

        return cohesionMove;
    }
}
