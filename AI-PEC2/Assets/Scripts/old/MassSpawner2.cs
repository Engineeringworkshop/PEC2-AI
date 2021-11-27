using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MassSpawner2 : MonoBehaviour
{

    public GameObject prefab;
    public GameObject wanderArea;
    //public GameObject player;
    //public GameObject shootPoint;
    //public GameObject bullet;


    public int Spawns = 10;
    int spawnCount = 0;
    List<GameObject> entities;

    void Start()
    {
        entities = new List<GameObject>();
        //entities.Add(player);
        InvokeRepeating("Spawn", 0f, 1.0f / 1000.0f);
    }

    // Metodo para instanciar prefabs y asignarles las componentes del behavior tree
    void Spawn()
    {
        if (entities.Count < Spawns)
        {
            GameObject instance =
              Instantiate(prefab, GetRandomPosition(), Quaternion.identity)
              as GameObject;
            BehaviorExecutor component =
              instance.GetComponent<BehaviorExecutor>();
            component.SetBehaviorParam("wanderArea", wanderArea);
            //component.SetBehaviorParam("player", player);
            //component.SetBehaviorParam("shootPoint", shootPoint.transform.position);
            //component.SetBehaviorParam("bullet", bullet);
            //component.SetBehaviorParam("bulletVelocity", 20);

            ++spawnCount;

            entities.Add(instance);
        }
        else
        {
            CancelInvoke();
        }

    }

    // Metodo para obtener una posición aleatoria dentro de un BoxCollider
    private Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = Vector3.zero;
        BoxCollider boxCollider = wanderArea.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            randomPosition =
              new Vector3(
                Random.Range(
                    wanderArea.transform.position.x -
                    wanderArea.transform.localScale.x *
                    boxCollider.size.x * 0.5f,
                    wanderArea.transform.position.x +
                    wanderArea.transform.localScale.x *
                    boxCollider.size.x * 0.5f),
                wanderArea.transform.position.y,
                Random.Range(
                    wanderArea.transform.position.z -
                    wanderArea.transform.localScale.z *
                    boxCollider.size.z * 0.5f,
                    wanderArea.transform.position.z +
                    wanderArea.transform.localScale.z *
                    boxCollider.size.z * 0.5f));
        }

        return randomPosition;
    }
}