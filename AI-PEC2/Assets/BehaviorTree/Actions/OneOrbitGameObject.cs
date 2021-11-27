using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{
    /// <summary>
    /// It is an action to orbit the given goal using a NavMeshAgent at a distance around an axis.
    /// </summary>
    [Action("Navigation/OneOrbitGameObject")]
    [Help("Moves the game object orbiting target using a NavMeshAgent at current distance around Y axis")]
    public class OneOrbitGameObject : GOAction
    {
        ///<value>Input target game object around this game object will be orbit Parameter.</value>
        [InParam("targetToOrbit")]
        [Help("Target game object this game object will be orbited")]
        public GameObject targetToOrbit;

        ///<value>Input target orbit distance this game object will be orbit Parameter.</value>
        [InParam("distanceToOrbit")]
        [Help("Target game object this game object will be orbited")]
        public float distanceToOrbit = 5;

        private UnityEngine.AI.NavMeshAgent navAgent;

        // Variables to generate path
        public Vector3[] points; // Vector of Vector3 position of the Path
        private int destPoint = 0; // Vector index.
        private int waypointAmount = 50; // Number of división points of the orbit

        /// <summary>Initialization Method of MoveToGameObject.</summary>
        /// <remarks>Check if GameObject object exists and NavMeshAgent, if there is no NavMeshAgent, the default one is added.</remarks>
        public override void OnStart()
        {
            // Cheks
            if (targetToOrbit == null)
            {
                Debug.LogError("The movement target of this game object is null", gameObject);
                return;
            }

            navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (navAgent == null)
            {
                Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
                navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
            }

            // Array initzialization
            points = new Vector3[waypointAmount];

            // Set stoppingDistance (tolerance)
            navAgent.stoppingDistance = 0.5f; // Remember to return stoppingDistance to 0!!!!

            // trayectory points generqator.
            GenerateCirclePath();

            // Go to next (first) position.
            GotoNextPoint();

#if UNITY_5_6_OR_NEWER
            navAgent.isStopped = false;
#else
            navAgent.Resume();
#endif
        }

        /// <summary>Method of Update of OneOrbitGameObject.</summary>
        /// <remarks>Verify the status of the task, if there is no objective fails, if it has traveled the road or is near the goal it is completed
        /// y, the task is running, if it is still moving to the target.</remarks>
        public override TaskStatus OnUpdate()
        {
            // Si no tiene un target fallará
            if (targetToOrbit == null)
                return TaskStatus.FAILED;

            // Si ha llegado al destino actual, pasa al siguiente
            if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            {
                // SI era el ultimo punto ha terminado
                if (destPoint >= points.Length)
                {
                    // Ha alcanzado el ultimo punto de la ruta => Completed
                    navAgent.stoppingDistance = 0; // Remember to return stoppingDistance to 0!!!!
                    return TaskStatus.COMPLETED;
                }
                else
                {
                    GotoNextPoint();
                    return TaskStatus.RUNNING;
                }
            } // SI no ha llegado, sigue
            else
            {
                return TaskStatus.RUNNING;
            }
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

        // Method to calculate path points
        public void GenerateCirclePath()
        {
            float angularIncrement = 2f*Mathf.PI / waypointAmount; // Incremento angular en radianes.

            Vector3 targetPosition = targetToOrbit.transform.position;

            for (int i = 0; i < waypointAmount; i++)
            {
                points[i].Set(targetPosition.x + distanceToOrbit * Mathf.Cos(angularIncrement * i), targetPosition.y, targetPosition.z + distanceToOrbit * Mathf.Sin(angularIncrement * i));
                //Debug.Log("Point " + i + " : " + points[i]);
            }
        }

        // Method to load path points
        public void GotoNextPoint()
        {
            // if point list is empty finish
            if (points.Length == 0)
                return;

            // Set next point as destination
            navAgent.SetDestination(points[destPoint]);

            // Increase counter
            destPoint++;
        }
    }
}
