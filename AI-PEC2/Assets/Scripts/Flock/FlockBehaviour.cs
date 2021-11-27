using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockBehaviour : ScriptableObject
{
    // context variable is a listoftranforms to keep generic how to add conditions to movement: neighbours, obstacles, boundaries...
    public abstract Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager);
}
