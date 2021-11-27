using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    /// <summary>
    /// It is an action to move towards the given goal until especified distance using a NavMeshAgent.
    /// </summary>
    [Action("Navigation/MoveToGameObjectAtDistance")]
    [Help("Moves the game object towards a given target until especified distance by using a NavMeshAgent")]
    public class MoveToGameObjectAtDistance : GOAction
    {
        ///<value>Input target game object towards this game object will be moved Parameter.</value>
        [InParam("target")]
        [Help("Target game object towards this game object will be moved")]
        public GameObject target;

        ///<value>Input distance to get close to game object Parameter.</value>
        [InParam("distance")]
        [Help("Target game object towards this game object will be moved")]
        public float distance;

        private UnityEngine.AI.NavMeshAgent navAgent;

        private Vector3 targetPosistion;
        private Vector3 agentPosition;

        /// <summary>Initialization Method of MoveToGameObjectAtDistance.</summary>
        /// <remarks>Check if GameObject object exists and NavMeshAgent, if there is no NavMeshAgent, the default one is added.</remarks>
        public override void OnStart()
        {
            if (target == null)
            {
                Debug.LogError("The movement target of this game object is null", gameObject);
                return;
            }
            // Get target GameObject position
            targetPosistion = target.transform.position;

            navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
                navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }
            // Get this agent GameObject position
            agentPosition = navAgent.transform.position;

            // Set stoppingDistance (tolerance)
            navAgent.stoppingDistance = 1; // Remember to return stoppingDistance to 0!!!!

            // calculate destination position (world coordinates).
            navAgent.SetDestination(CalculateTargetPositionToReach());

#if UNITY_5_6_OR_NEWER
            navAgent.isStopped = false;
#else
                navAgent.Resume();
#endif
        }

        /// <summary>Method of Update of MoveToGameObjectAtDistance.</summary>
        /// <remarks>Verify the status of the task, if there is no objective fails, if it has traveled the road or is near the goal it is completed
        /// y, the task is running, if it is still moving to the target.</remarks>
        public override TaskStatus OnUpdate()
        {
            if (target == null)
            {
                return TaskStatus.FAILED;
            }     
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                navAgent.stoppingDistance = 0; // Remember to return stoppingDistance to 0!!!!
                return TaskStatus.COMPLETED;
            }
            else if (navAgent.destination != targetPosistion)
            {
                return TaskStatus.RUNNING;
            }
            return TaskStatus.RUNNING;
        }
        /// <summary>Abort method of MoveToGameObject </summary>
        /// <remarks>When the task is aborted, it stops the navAgentMesh.</remarks>
        public override void OnAbort()
        {

#if UNITY_5_6_OR_NEWER
            if (navAgent != null)
                navAgent.isStopped = true;
#else
            if (navAgent!=null)
                navAgent.Stop();
#endif

        }

        // Method to calculate target position to moe according current agent location, destination and distance from target onject to stop
        public Vector3 CalculateTargetPositionToReach()
        {
            return agentPosition + (targetPosistion - agentPosition).normalized * (Vector3.Distance(targetPosistion, agentPosition) - distance); //finalPos = agentPosition + directionNormalized * travelDistance;
        }
    }
}

