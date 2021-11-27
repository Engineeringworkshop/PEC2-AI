using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class AvoidanceBehaviour : FilteredFlockBehaviour
{
    // This method find middle point between neighbours and tryes to move there.
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // If no neighbors, return no adjustment
        if (context.Count == 0)
            return Vector3.zero;

        // Add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0; // to count number of objects in the avoidnce radius

        // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            // Get the closes point between agent and context item.
            Vector3 closestPoint = item.gameObject.GetComponent<Collider>().ClosestPoint(agent.transform.position);

            // If is closer to avoid
            if (Vector3.SqrMagnitude(closestPoint - agent.transform.position) < flockManager.SquareAvoidanceRadius)
            {
                nAvoid++;
                avoidanceMove += agent.transform.position - closestPoint; // adds the offset
            }
        }

        // Calculate average offset
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;

        return avoidanceMove;
    }
}
