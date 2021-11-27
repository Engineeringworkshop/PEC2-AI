using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Una vez que la bala es instanciada, se destruirá a los 2 segundos.
        Destroy(gameObject, 2);
    }
}
