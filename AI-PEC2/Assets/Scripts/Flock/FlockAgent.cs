using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Need a Collider component attached to the flock agent prefab to can manage algoritm
[RequireComponent(typeof(Collider))]
public class FlockAgent : MonoBehaviour
{
    FlockManager flockManager;

    public FlockManager FlockManager { get { return flockManager; } }

    // We add agentCollider component and the acces to the collider
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }

    // Start is called before the first frame update
    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    // Method to initialized flockManager variable
    public void InitializeFlockManager(FlockManager fM)
    {
        flockManager = fM;
    }

    //Method to move the agent
    public void Move(Vector3 velocity)
    {
        //Turn agent to face direction
        transform.forward = velocity;

        //Move agent
        transform.position += velocity * Time.deltaTime;
    }

    //Method to destroy agent
    public void DestroyAgent()
    {
        flockManager.DestroyAgentAndUnlist(this);
    }
}
