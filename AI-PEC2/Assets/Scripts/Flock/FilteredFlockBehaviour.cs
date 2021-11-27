using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The behaviours that we need to have filter, need to inheritance from this class
public abstract class FilteredFlockBehaviour : FlockBehaviour
{
    public ContextFilter filter;
}
