using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Filter/Physics Layer")]
public class PhysicsLayerFilter : ContextFilter
{
    public LayerMask mask;

    // Method to filter list<Transform>
    public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
    {
        List<Transform> filtered = new List<Transform>();

        // For each item in original list, we want to filter the agents of the same nest and ignore the rest of them
        foreach (Transform item in original)
        {
            //if(mask == (mask | (1 << item.gameObject.layer)))
            if(item.gameObject.layer == mask)
            {
                filtered.Add(item);
            }
        }

        return filtered;
    }
}
