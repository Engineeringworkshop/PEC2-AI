# AI-PEC2

Repositorio de la PEC2 de la asignatura de inteligencia artificial.PEC 2

# 1 Video y repositorio de las 3 Actividades
Enlace: https://youtu.be/gP-4p3V0U7Y
Repositorio GitLab: https://gitlab.com/pmolinapa/ai-pec2
Repositorio GitHub: https://github.com/Engineeringworkshop/PEC2-AI 

# 2  Actividad 1
Partiendo del tutorial del enunciado: (http://bb.padaonegames.com/doku.php). He añadido algunos elementos decorativos: Una cruz, cuatro pilares y 9 árboles.
Los comportamientos usados son:
Wander: Vagabundear. Mismo comportamiento que el tutorial.
LookAtObject: Mirar a un objeto. Se seleccionará el objeto más cercano que tenga el tag “Cross”, correspondiente a la cruz del centro de la plaza. El agente se desplazará a una distancia mínima, mirará hacia la cruz y la contemplará (esperará) 5 segundos.
OrbitTagObject: Orbitar un objeto a una distancia determinada. El agente seleccionará el objeto con el tag “Tree” correspondiente a cualquier árbol de la plaza. Se acercará a él y dará una vuelta completa al árbol.

# 2.1  Behaviour tree

•	Behaviours

Actividad1:
Para controlar a los agentes (NPC) he utilizado el siguiente behaviour tree. Un repetidor, un random selector para dotar de aleatoriedad a los agentes y las 3 acciones que quiero que hagan.
Cada una de las acciones creada por separado.

Wander:

Usando las acciones incluidas en el asset behaviour bricks.

LookAtObject2: 

Usando las acciones incluidas en el asset behaviour bricks.

OrbitTagObject:

Usando las acciones incluidas en el asset behaviour bricks

•	Actions:

MoveToGameObjectAtDistance:

Su funcionamiento es análogo a MoveToGameObject disponible en el tutorial. La única diferencia es que, en el cálculo del de la posición objetivo se tienen en cuenta la distancia a la que queremos quedarnos del objetivo.

2  using Pada1.BBCore.Tasks;
3  using Pada1.BBCore;
4  using UnityEngine;
5  
6  namespace BBUnity.Actions
7  {
8      /// <summary>
9      /// It is an action to move towards the given goal until especified distance using a NavMeshAgent.
10      /// </summary>
11      [Action("Navigation/MoveToGameObjectAtDistance")]
12      [Help("Moves the game object towards a given target until especified distance by using a NavMeshAgent")]
13      public class MoveToGameObjectAtDistance : GOAction
14      {
15          ///<value>Input target game object towards this game object will be moved Parameter.</value>
16          [InParam("target")]
17          [Help("Target game object towards this game object will be moved")]
18          public GameObject target;
19  
20          ///<value>Input distance to get close to game object Parameter.</value>
21          [InParam("distance")]
22          [Help("Target game object towards this game object will be moved")]
23          public float distance;
24  
25          private UnityEngine.AI.NavMeshAgent navAgent;
26  
27          private Vector3 targetPosistion;
28          private Vector3 agentPosition;
29  
30          /// <summary>Initialization Method of MoveToGameObjectAtDistance.</summary>
31          /// <remarks>Check if GameObject object exists and NavMeshAgent, if there is no NavMeshAgent, the default one is added.</remarks>
32          public override void OnStart()
33          {
34              if (target == null)
35              {
36                  Debug.LogError("The movement target of this game object is null", gameObject);
37                  return;
38              }
39              // Get target GameObject position
40              targetPosistion = target.transform.position;
41  
42              navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
43              if (navAgent == null)
44              {
45                  Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
46                  navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
47              }
48              // Get this agent GameObject position
49              agentPosition = navAgent.transform.position;
50  
51              // Set stoppingDistance (tolerance)
52              navAgent.stoppingDistance = 1; // Remember to return stoppingDistance to 0!!!!
53  
54              // calculate destination position (world coordinates).
55              navAgent.SetDestination(CalculateTargetPositionToReach());
56  
57  #if UNITY_5_6_OR_NEWER
58              navAgent.isStopped = false;
59  #else
60                  navAgent.Resume();
61  #endif
62          }
63  
64          /// <summary>Method of Update of MoveToGameObjectAtDistance.</summary>
65          /// <remarks>Verify the status of the task, if there is no objective fails, if it has traveled the road or is near the goal it is completed
66          /// y, the task is running, if it is still moving to the target.</remarks>
67          public override TaskStatus OnUpdate()
68          {
69              if (target == null)
70              {
71                  return TaskStatus.FAILED;
72              }     
73              if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
74              {
75                  navAgent.stoppingDistance = 0; // Remember to return stoppingDistance to 0!!!!
76                  return TaskStatus.COMPLETED;
77              }
78              else if (navAgent.destination != targetPosistion)
79              {
80                  return TaskStatus.RUNNING;
81              }
82              return TaskStatus.RUNNING;
83          }
84          /// <summary>Abort method of MoveToGameObject </summary>
85          /// <remarks>When the task is aborted, it stops the navAgentMesh.</remarks>
86          public override void OnAbort()
87          {
88  
89  #if UNITY_5_6_OR_NEWER
90              if (navAgent != null)
91                  navAgent.isStopped = true;
92  #else
93              if (navAgent!=null)
94                  navAgent.Stop();
95  #endif
96  
97          }
98  
99          // Method to calculate target position to moe according current agent location, destination and distance from target onject to stop
100          public Vector3 CalculateTargetPositionToReach()
101          {
102              return agentPosition + (targetPosistion - agentPosition).normalized * (Vector3.Distance(targetPosistion, agentPosition) - distance); //finalPos = agentPosition + directionNormalized * travelDistance;
103          }
104      }
105  }

OneOrbitGameObject:

Esta acción ha sido creada explícitamente. Necesita dos parámetros, el objetivo (GameObject) y la distancia (float) para orbitarle. Para ello genero los puntos de ruta aplicando formulas trigonométricas. Usando el parámetro waypointAmount = 50;, que por otro lado podría ser un parámetro externo, podemos definir el número de vértices que tendrá el polígono de aspecto circular pudiendo ajustar la suavidad del movimiento.

1  using Pada1.BBCore.Tasks;
2  using Pada1.BBCore;
3  using UnityEngine;
4  
5  namespace BBUnity.Actions
6  {
7      /// <summary>
8      /// It is an action to orbit the given goal using a NavMeshAgent at a distance around an axis.
9      /// </summary>
10      [Action("Navigation/OneOrbitGameObject")]
11      [Help("Moves the game object orbiting target using a NavMeshAgent at current distance around Y axis")]
12      public class OneOrbitGameObject : GOAction
13      {
14          ///<value>Input target game object around this game object will be orbit Parameter.</value>
15          [InParam("targetToOrbit")]
16          [Help("Target game object this game object will be orbited")]
17          public GameObject targetToOrbit;
18  
19          ///<value>Input target orbit distance this game object will be orbit Parameter.</value>
20          [InParam("distanceToOrbit")]
21          [Help("Target game object this game object will be orbited")]
22          public float distanceToOrbit = 5;
23  
24          private UnityEngine.AI.NavMeshAgent navAgent;
25  
26          // Variables to generate path
27          public Vector3[] points; // Vector of Vector3 position of the Path
28          private int destPoint = 0; // Vector index.
29          private int waypointAmount = 50; // Number of división points of the orbit
30  
31          /// <summary>Initialization Method of MoveToGameObject.</summary>
32          /// <remarks>Check if GameObject object exists and NavMeshAgent, if there is no NavMeshAgent, the default one is added.</remarks>
33          public override void OnStart()
34          {
35              // Cheks
36              if (targetToOrbit == null)
37              {
38                  Debug.LogError("The movement target of this game object is null", gameObject);
39                  return;
40              }
41  
42              navAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
43              if (navAgent == null)
44              {
45                  Debug.LogWarning("The " + gameObject.name + " game object does not have a Nav Mesh Agent component to navigate. One with default values has been added", gameObject);
46                  navAgent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
47              }
48  
49              // Array initzialization
50              points = new Vector3[waypointAmount];
51  
52              // Set stoppingDistance (tolerance)
53              navAgent.stoppingDistance = 0.5f; // Remember to return stoppingDistance to 0!!!!
54  
55              // trayectory points generqator.
56              GenerateCirclePath();
57  
58              // Go to next (first) position.
59              GotoNextPoint();
60  
61  #if UNITY_5_6_OR_NEWER
62              navAgent.isStopped = false;
63  #else
64              navAgent.Resume();
65  #endif
66          }
67  
68          /// <summary>Method of Update of OneOrbitGameObject.</summary>
69          /// <remarks>Verify the status of the task, if there is no objective fails, if it has traveled the road or is near the goal it is completed
70          /// y, the task is running, if it is still moving to the target.</remarks>
71          public override TaskStatus OnUpdate()
72          {
73              // Si no tiene un target fallará
74              if (targetToOrbit == null)
75                  return TaskStatus.FAILED;
76  
77              // Si ha llegado al destino actual, pasa al siguiente
78              if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
79              {
80                  // SI era el ultimo punto ha terminado
81                  if (destPoint >= points.Length)
82                  {
83                      // Ha alcanzado el ultimo punto de la ruta => Completed
84                      navAgent.stoppingDistance = 0; // Remember to return stoppingDistance to 0!!!!
85                      return TaskStatus.COMPLETED;
86                  }
87                  else
88                  {
89                      GotoNextPoint();
90                      return TaskStatus.RUNNING;
91                  }
92              } // SI no ha llegado, sigue
93              else
94              {
95                  return TaskStatus.RUNNING;
96              }
97          }
98  
99          /// <summary>Abort method of MoveToGameObject </summary>
100          /// <remarks>When the task is aborted, it stops the navAgentMesh.</remarks>
101          public override void OnAbort()
102          {
103  
104  #if UNITY_5_6_OR_NEWER
105              if (navAgent != null)
106                  navAgent.isStopped = true;
107  #else
108              if (navAgent!=null)
109                  navAgent.Stop();
110  #endif
111  
112          }
113  
114          // Method to calculate path points
115          public void GenerateCirclePath()
116          {
117              float angularIncrement = 2f*Mathf.PI / waypointAmount; // Incremento angular en radianes.
118  
119              Vector3 targetPosition = targetToOrbit.transform.position;
120  
121              for (int i = 0; i < waypointAmount; i++)
122              {
123                  points[i].Set(targetPosition.x + distanceToOrbit * Mathf.Cos(angularIncrement * i), targetPosition.y, targetPosition.z + distanceToOrbit * Mathf.Sin(angularIncrement * i));
124                  //Debug.Log("Point " + i + " : " + points[i]);
125              }
126          }
127  
128          // Method to load path points
129          public void GotoNextPoint()
130          {
131              // if point list is empty finish
132              if (points.Length == 0)
133                  return;
134  
135              // Set next point as destination
136              navAgent.SetDestination(points[destPoint]);
137  
138              // Increase counter
139              destPoint++;
140          }
141      }
142  }

# 2.1  Spawner:

He utilizado el GameObject llamado spawner para instanciar las copias de los agentes. El spawn asignará los objetos (y strings) a los campos correspondientes del behaviour tree de cada agente. El código del spawner, también copiado y adaptado del tutorial es:

1  using UnityEngine;
2  using System.Collections.Generic;
3  
4  public class NPCSpawner : MonoBehaviour
5  {
6  
7      public GameObject prefab;
8      public GameObject wanderArea;
9      public string targetToLookAt;
10      public string tagToOrbit;
11      public float distanceToOrbit;
12  
13      public int Spawns = 10;
14      int spawnCount = 0;
15      List<GameObject> entities;
16  
17      void Start()
18      {
19          entities = new List<GameObject>();
20          //entities.Add(player);
21          InvokeRepeating("Spawn", 0f, 1.0f / 1000.0f);
22      }
23  
24      // Metodo para instanciar prefabs y asignarles las componentes del behavior tree
25      void Spawn()
26      {
27          if (entities.Count < Spawns)
28          {
29              GameObject instance =
30                Instantiate(prefab, GetRandomPosition(), Quaternion.identity)
31                as GameObject;
32              BehaviorExecutor component =
33                instance.GetComponent<BehaviorExecutor>();
34              component.SetBehaviorParam("wanderArea", wanderArea);
35              component.SetBehaviorParam("targetToLookAt", targetToLookAt);
36              component.SetBehaviorParam("tagToOrbit", tagToOrbit);
37              component.SetBehaviorParam("distanceToOrbit", distanceToOrbit);
38  
39              ++spawnCount;
40  
41              entities.Add(instance);
42          }
43          else
44          {
45              CancelInvoke();
46          }
47  
48      }
49  
50      // Metodo para obtener una posición aleatoria dentro de un BoxCollider
51      private Vector3 GetRandomPosition()
52      {
53          Vector3 randomPosition = Vector3.zero;
54          BoxCollider boxCollider = wanderArea.GetComponent<BoxCollider>();
55          if (boxCollider != null)
56          {
57              randomPosition =
58                new Vector3(
59                  Random.Range(
60                      wanderArea.transform.position.x -
61                      wanderArea.transform.localScale.x *
62                      boxCollider.size.x * 0.5f,
63                      wanderArea.transform.position.x +
64                      wanderArea.transform.localScale.x *
65                      boxCollider.size.x * 0.5f),
66                  wanderArea.transform.position.y,
67                  Random.Range(
68                      wanderArea.transform.position.z -
69                      wanderArea.transform.localScale.z *
70                      boxCollider.size.z * 0.5f,
71                      wanderArea.transform.position.z +
72                      wanderArea.transform.localScale.z *
73                      boxCollider.size.z * 0.5f));
74          }
75  
76          return randomPosition;
77      }
78  }

# 3  Actividad 2

# 3.1  FlockManager

El FlockManager se encargará de administrar los flock (FlockAgent). Instanciará los FlockAgents alrededor del controlador, en este caso, una colmena de abejas.
Para los cálculos matemáticos he usado el cuadrado de las variables. Esto evita tener que calcular la raíz de la magnitud de los vectores, lo cual tiene un alto coste computacional.

Para los FlockBehaviours he creado una clase abstracta de la que heredarán los behaviours que definamos. De esta forma se puede añadir cualquier cantidad de comportamientos y ajustar su funcionamiento mediante los weights (pesos).
También he creado la clase ContextFilter para poder crear behaviours que filtren los objetos, para poder evitar obstáculos.

1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  public class FlockManager : MonoBehaviour
6  {
7      // Components
8      public FlockAgent agentPrefab;
9      List<FlockAgent> agents = new List<FlockAgent>();
10      public FlockBehaviour behaviour;
11  
12      // Spawn variables
13      [Header("Spawn control")]
14      [Range(10, 500)]
15      public int startingCount = 50;
16      [Range(0.5f, 10f)]
17      public float spawnMaxRadius = 2f;
18      [HideInInspector] int spawnCount = 0; // Auxiliar variable to help to set Agent names ("Agent" + spawnCount)
19  
20      // how the flock behaves
21      [Header("Flock control")]
22      [Range(1f, 100f)]
23      public float driveFactor = 10f;// To avoid slow movements from flock
24      [Range(1f, 100f)]
25      public float maxSpeed = 5f;// to avoid exceed max speed when using driveFactor
26      [Range(0.01f, 1f)]
27      public float neighborRadius = 0.02f;
28      [Range(0f, 0.5f)]
29      public float avoidanceRadiusMultiplier = 0.1f;
30  
31      // Utility variables to save math calculations, unity uses square values for magnitude, for example
32      float squareMaxSpeed;
33      float squareNeighborRadius;
34      float squareAvoidanceRadius;
35      public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
36  
37      // Start is called before the first frame update
38      void Start()
39      {
40          squareMaxSpeed = maxSpeed * maxSpeed;
41          squareNeighborRadius = neighborRadius * neighborRadius;
42          squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier; // is correct?? squareNeighborRadius is necessary???
43  
44          for (int i = 0; i < startingCount; i++)
45          {
46              SpawnNewAgent();
47          }
48      }
49  
50      // Update is called once per frame
51      void Update()
52      {
53          // foreach agent, calculate movement based on his nearby objects
54          foreach (FlockAgent agent in agents)
55          {
56              // get list of nearby objects
57              List<Transform> context = GetNearbyObjects(agent);
58              
59              // Now we'll calculate movement
60              Vector3 move = behaviour.CalculateMove(agent, context, this);
61  
62              // Apply velocity
63              move *= driveFactor;
64              if (move.sqrMagnitude > squareMaxSpeed)
65              {
66                  move = move.normalized * maxSpeed;
67              }
68  
69              // Ordero to move
70              agent.Move(move);
71          }
72  
73          // Check if need to spawn new agents (if something is deleting it)
74          if (agents.Count < startingCount)
75          {
76              SpawnNewAgent();
77          }
78      }
79  
80      // Method to spawn FlockAgents
81      private void SpawnNewAgent()
82      {
83          FlockAgent newAgent = Instantiate(
84              agentPrefab,
85              transform.position + Random.insideUnitSphere * spawnMaxRadius, //startingCount * agentDensity * agentPrefab.transform.localScale.sqrMagnitude,
86              Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))),
87              transform
88      );
89          newAgent.name = "Agent" + spawnCount;
90          newAgent.InitializeFlockManager(this);
91          agents.Add(newAgent);
92  
93          spawnCount++;
94      }
95  
96  
97      // Method to get nearby objects from a FlockAgent
98      List<Transform> GetNearbyObjects(FlockAgent agent)
99      {
100          // We want to get the list of nearby angets, not all agents
101          List<Transform> context = new List<Transform>();
102          Collider[] contextColliders = Physics.OverlapSphere(agent.transform.position, neighborRadius);
103  
104          // if any collider that is not this, and add it to the list
105          foreach(Collider c in contextColliders)
106          {
107              if (c != agent.AgentCollider)
108              {
109                  context.Add(c.transform);
110              }
111          }
112          return context;
113      }
114  
115      //Method to destroy agent
116      public void DestroyAgentAndUnlist(FlockAgent obj)
117      {
118          agents.Remove(obj);
119          Destroy(obj.gameObject);
120      }
121  }


# 3.2  FlockAgent

Es la clase que incorporarán cada uno de los agentes que formen el flock. Permitirá al FlockManager administrar cada uno de sus FlockAgents.

122  using System.Collections;
123  using System.Collections.Generic;
124  using UnityEngine;
125  
126  // Need a Collider component attached to the flock agent prefab to can manage algoritm
127  [RequireComponent(typeof(Collider))]
128  public class FlockAgent : MonoBehaviour
129  {
130      FlockManager flockManager;
131  
132      public FlockManager FlockManager { get { return flockManager; } }
133  
134      // We add agentCollider component and the acces to the collider
135      Collider agentCollider;
136      public Collider AgentCollider { get { return agentCollider; } }
137  
138      // Start is called before the first frame update
139      void Start()
140      {
141          agentCollider = GetComponent<Collider>();
142      }
143  
144      // Method to initialized flockManager variable
145      public void InitializeFlockManager(FlockManager fM)
146      {
147          flockManager = fM;
148      }
149  
150      //Method to move the agent
151      public void Move(Vector3 velocity)
152      {
153          //Turn agent to face direction
154          transform.forward = velocity;
155  
156          //Move agent
157          transform.position += velocity * Time.deltaTime;
158      }
159  
160      //Method to destroy agent
161      public void DestroyAgent()
162      {
163          flockManager.DestroyAgentAndUnlist(this);
164      }
165  }

# 3.3  FlockBehaviour

Esta es la clase abstracta de la que heredarán los demás behaviours. Para cada behaviour crearé un objeto (asset) de unity (Con CreateAssetMenu) para poder arrastrarlo al CompositeBehaviour que será el behaviour que controlará al resto y poder configurarlos desde el inspector.


1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  public abstract class FlockBehaviour : ScriptableObject
6  {
7      // context variable is a listoftranforms to keep generic how to add conditions to movement: neighbours, obstacles, boundaries...
8      public abstract Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager);
9  }

•	CompositeBehaviour

Behaviour principal, se encargará de calcular el movimiento final de cada agente en función de los resultados obtenidos de cada comportamiento por separado. El cálculo final se realiza ponderando cada movimiento según unos weights (pesos) que definiremos en el propio objeto desde el inspector de Unity.
Cada uno de los behaviours incorpora en su código la llamada al filtro, si el filtro no está asignado no hará nada.

 
1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
6  public class CompositeBehaviour : FlockBehaviour
7  {
8      // Array of flock behabiours
9      public FlockBehaviour[] behaviours;
10  
11      // Array of the weights associated to each behaviour
12      public float[] weights; // will correlate agents to calculate weights to compensate calculations
13  
14      public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
15      {
16          // handle data missmatch
17          if(weights.Length != behaviours.Length)
18          {
19              Debug.LogError("Data mismatch in " + name, this);
20              return Vector3.zero; // No movement in case of error
21          }
22  
23          // Set up move
24          Vector3 move = Vector3.zero;
25  
26          // Iterate through behaviors
27          for (int i = 0; i < behaviours.Length; i++)
28          {
29              Vector3 partialMove = behaviours[i].CalculateMove(agent, context, flockManager) * weights[i];
30  
31              if(partialMove != Vector3.zero)
32              {
33                  // Check if mgnitude is bigger than weight, if is => limitate to the max weight.
34                  if (partialMove.sqrMagnitude > weights[i] * weights[i])
35                  {
36                      partialMove.Normalize();
37                      partialMove *= weights[i];
38                  }
39  
40                  move += partialMove;
41              }
42          }
43  
44          return move;
45      }
46  }


•	SteeredCohesionBehaviour

Este behaviour se encargará de calcular la cohesión. Para suavizar el comportamiento y evitar que cambien radicalmente de dirección los agentes, he añadido un suavizado con la función de Unity SmoothDamp. 


1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Behaviour/Steered Cohesion")]
6  public class SteeredCohesionBehaviour : FilteredFlockBehaviour
7  {
8      Vector3 currentVelocity;
9      public float agentSmoothTime = 0.5f; // how quickly we want to correct the cohesion Bigger=>smoother 
10  
11      public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
12      {
13          // If no neighbors, return no adjustment
14          if (context.Count == 0)
15              return Vector3.zero;
16  
17          // Add all points together and average
18          Vector3 cohesionMove = Vector3.zero;
19  
20          // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
21          List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
22  
23          foreach (Transform item in filteredContext)
24          {
25              cohesionMove += item.position;
26          }
27          // need average => middle point
28          cohesionMove /= context.Count;
29  
30          // Create offset from agent position
31          cohesionMove -= agent.transform.position;
32          cohesionMove = Vector3.SmoothDamp(agent.transform.forward, cohesionMove, ref currentVelocity, agentSmoothTime);
33  
34          return cohesionMove;
35      }
36  }

•	AlignmentBehaviour

AlignmentBehaviour se encarga de controlar las direcciones de cada FlockAgent.


1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Behaviour/Aligment")]
6  public class AlignmentBehaviour : FilteredFlockBehaviour
7  {
8      // 
9      public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
10      {
11          // If no neighbors, maintain current aligment
12          if (context.Count == 0)
13              return agent.transform.forward;
14  
15          // Add all forward directions together and average
16          Vector3 aligmentMove = Vector3.zero;
17  
18          // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
19          List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
20  
21          foreach (Transform item in filteredContext)
22          {
23              aligmentMove += item.transform.forward;
24          }
25          // need average => middle point
26          aligmentMove /= context.Count;
27  
28          return aligmentMove;
29      }
30  }

•	AvoidanceBehaviour

Controla la separación entre agentes.

1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
6  public class AvoidanceBehaviour : FilteredFlockBehaviour
7  {
8      // This method find middle point between neighbours and tryes to move there.
9      public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
10      {
11          // If no neighbors, return no adjustment
12          if (context.Count == 0)
13              return Vector3.zero;
14  
15          // Add all points together and average
16          Vector3 avoidanceMove = Vector3.zero;
17          int nAvoid = 0; // to count number of objects in the avoidnce radius
18  
19          // If no filter is asigned, filteredContext will iqual to context, if filter is asigned, then filter context.
20          List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);
21  
22          foreach (Transform item in filteredContext)
23          {
24              // Get the closes point between agent and context item.
25              Vector3 closestPoint = item.gameObject.GetComponent<Collider>().ClosestPoint(agent.transform.position);
26  
27              // If is closer to avoid
28              if (Vector3.SqrMagnitude(closestPoint - agent.transform.position) < flockManager.SquareAvoidanceRadius)
29              {
30                  nAvoid++;
31                  avoidanceMove += agent.transform.position - closestPoint; // adds the offset
32              }
33          }
34  
35          // Calculate average offset
36          if (nAvoid > 0)
37              avoidanceMove /= nAvoid;
38  
39          return avoidanceMove;
40      }
41  }

•	StayInRadiusFromNest

Behaviour adicional. Con este comportamiento se consigue que los agentes no se alejen de su FlockManager, en este caso, las abejas no se alejarán de la colmena más de la distancia programada. En este caso utilizo la raíz de la magnitud para los cálculos, matemáticamente resultaba demasiado complejo.

1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Behaviour/Stay In Radius From Nest")]
6  public class StayInRadiusFromNest : FlockBehaviour
7  {
8      public float radius = 15f;
9  
10      public override Vector3 CalculateMove(FlockAgent agent, List<Transform> context, FlockManager flockManager)
11      {
12          // Define center position
13          Vector3 center = flockManager.transform.position; // Center of movement equals to flockManager positions (nest origin)
14  
15          // Check agent distance from center
16          Vector3 centerOffset = center - agent.transform.position;
17          float t = centerOffset.magnitude / (radius); // radius*radius to use sqrMagnitude instead Magnitude
18  
19          // If we are on the 90% from center need to do nothing (0.9 * 0.9 to use sqrMagnitude instead Magnitude)
20          if (t < 0.9f * 0.9f)
21          {
22              return Vector3.zero;
23          }
24  
25          return centerOffset * t * t;
26      }
27  }
•	ContextFilter

Los filtros de objetos a evitar heredarán de esta clase abstracta.

1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  public abstract class ContextFilter : ScriptableObject
6  {
7      public abstract List<Transform> Filter(FlockAgent agent, List<Transform> original);
8  }


•	PhysicsLayerFilter

PhysicsLayerFilter se encargará de filtrar los objetos según las capas que le digamos en Unity inspector.

1  using System.Collections;
2  using System.Collections.Generic;
3  using UnityEngine;
4  
5  [CreateAssetMenu(menuName = "Flock/Filter/Physics Layer")]
6  public class PhysicsLayerFilter : ContextFilter
7  {
8      public LayerMask mask;
9  
10      // Method to filter list<Transform>
11      public override List<Transform> Filter(FlockAgent agent, List<Transform> original)
12      {
13          List<Transform> filtered = new List<Transform>();
14  
15          // For each item in original list, we want to filter the agents of the same nest and ignore the rest of them
16          foreach (Transform item in original)
17          {
18              //if(mask == (mask | (1 << item.gameObject.layer)))
19              if(item.gameObject.layer == mask)
20              {
21                  filtered.Add(item);
22              }
23          }
24  
25          return filtered;
26      }
27  }

# 4  Actividad 3

# 4.1  Behaviour tree

El comportamiento del perro viene definido por el árbol siguiente. Un repetidor y un random slector irán alternando aleatoriamente entre vagabundear y cazar. La acción de caza está limitada por tiempo, como las abejas pueden ser inalcanzables para el perro, puede darse la situación de que no llegue a cogerla en un tiempo razonable. En ese caso, parece pensar que el perro desistirá y vovlerá a intentarlo más tarde.

•	Wander
El comportamiento de wander es el mismo descrito en la Actividad 1.
•	HuntObject

Una secuencia se encarga de que primero se seleccione el GameObject más cercano con la etiqueta especificada. Una vez lo tiene, pasa a un prioriti selector que se acercará al objetivo hasta que esté lo bastante cerca de él (2 unidades de distancia). En ese momento, mediante el sendMessage se enviará al FlockAgent la señal para destrirse. Este a si vez invocará a su FlockManager que será el encargado de sacarlo de la lista del FlockManager y posterior y finalmente destruirle.

 
# 5  Recursos:

•	Modelos 3D: “rts_props.blend, models by Lamoot, Blender Texture CD for the wood, pdtextures blog for the stone, https://opengameart.org/content/medieval-props-textured, CC-BY 3.0.
