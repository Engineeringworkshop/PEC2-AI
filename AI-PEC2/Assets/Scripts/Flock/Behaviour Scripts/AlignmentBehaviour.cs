using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Aligment")]
public class AlignmentBehaviour : FilteredFlockBehaviour
{
    // 
    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // If no neighbors, maintain current aligment
        if (context.Count == 0)
            return agent.transform.forward;

        // Add all forward directions together and average
        Vector3 aligmentMove = Vector3.zero;

        // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            aligmentMove += item.transform.forward;
        }
        // need average => middle point
        aligmentMove /= context.Count;

        return aligmentMove;
    }
}
