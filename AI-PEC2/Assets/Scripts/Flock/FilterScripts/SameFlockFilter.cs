using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
    // Method to filter list<Transform>
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        // For each item in original list, we want to filter the agents of the same nest and ignore the rest of them
        foreach (Transform item in original)
        {
            FlockAgent itemAgent = item.GetComponent<FlockAgent>();
            if (itemAgent != null && itemAgent.FlockManager == agent.FlockManager) // If they have the same FlockManager (same nest) then add to filtered list,
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}
