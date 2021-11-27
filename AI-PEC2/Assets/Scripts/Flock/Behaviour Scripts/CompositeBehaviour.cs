using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class CompositeBehaviour : FlockBehaviour
{
    // Array of flock behabiours
    public FlockBehaviour[] behaviours;

    // Array of the weights associated to each behaviour
    public float[] weights; // will correlate agents to calculate weights to compensate calculations

    public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
    {
        // handle data missmatch
        if(weights.Length != behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero; // No movement in case of error
        }

        // Set up move
        Vector3 move = Vector3.zero;

        // Iterate through behaviors
        for (int i = 0; i < behaviours.Length; i++)
        {
            Vector3 partialMove = behaviours[i].CalculateMove(agent, context, flockManager) * weights[i];

            if(partialMove != Vector3.zero)
            {
                // Check if mgnitude is bigger than weight, if is => limitate to the max weight.
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
