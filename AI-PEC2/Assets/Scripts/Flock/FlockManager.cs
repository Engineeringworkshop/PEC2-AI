using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    // Components
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behaviour;

    // Spawn variables
    [Header("Spawn control")]
    [Range(10, 500)]
    public int startingCount = 50;
    [Range(0.5f, 10f)]
    public float spawnMaxRadius = 2f;
    [HideInInspector] int spawnCount = 0; // Auxiliar variable to help to set Agent names ("Agent" + spawnCount)

    // how the flock behaves
    [Header("Flock control")]
    [Range(1f, 100f)]
    public float driveFactor = 10f;// To avoid slow movements from flock
    [Range(1f, 100f)]
    public float maxSpeed = 5f;// to avoid exceed max speed when using driveFactor
    [Range(0.01f, 1f)]
    public float neighborRadius = 0.02f;
    [Range(0f, 0.5f)]
    public float avoidanceRadiusMultiplier = 0.1f;

    // Utility variables to save math calculations, unity uses square values for magnitude, for example
    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier; // is correct?? squareNeighborRadius is necessary???

        for (int i = 0; i < startingCount; i++)
        {
            SpawnNewAgent();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // foreach agent, calculate movement based on his nearby objects
        foreach (FlockAgent agent in agents)
        {
            // get list of nearby objects
            List<Transform> context = GetNearbyObjects(agent);
            
            // Now we'll calculate movement
            Vector3 move = behaviour.CalculateMove(agent, context, this);

            // Apply velocity
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            // Ordero to move
            agent.Move(move);
        }

        // Check if need to spawn new agents (if something is deleting it)
        if (agents.Count < startingCount)
        {
            SpawnNewAgent();
        }
    }

    // Method to spawn FlockAgents
    private void SpawnNewAgent()
    {
        FlockAgent newAgent = Instantiate(
            agentPrefab,
            transform.position + Random.insideUnitSphere * spawnMaxRadius, //startingCount * agentDensity * agentPrefab.transform.localScale.sqrMagnitude,
            Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))),
            transform
    );
        newAgent.name = "Agent" + spawnCount;
        newAgent.InitializeFlockManager(this);
        agents.Add(newAgent);

        spawnCount++;
    }


    // Method to get nearby objects from a FlockAgent
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        // We want to get the list of nearby angets, not all agents
        List<Transform> context = new List<Transform>();
        Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);

        // if any collider that is not this, and add it to the list
        foreach(Collider c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }
        return context;
    }

    //Method to destroy agent
    public void DestroyAgentAndUnlist(FlockAgent obj)
    {
        agents.Remove(obj);
        Destroy(obj.gameObject);
    }
}
