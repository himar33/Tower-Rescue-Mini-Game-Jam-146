using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float vel = 5f;
    public Transform mFloorTransform;
    public Transform mPlayerTransform;

    [Range(1f,10f)]
    public float radioMelee;  // Asigna el Slider desde el Inspector.
    private SphereCollider collDetect;


    // Start is called before the first frame update
    void Start()
    {
        collDetect = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();

    }
    void MoveEnemy()
    {
        // Obtén la entrada del teclado o cualquier otra fuente de entrada.
        float moveHorizontal = Input.GetAxis("Horizontal");
        float mvVertical = Input.GetAxis("Vertical");

        // Calcula la dirección del movimiento en el plano XZ.
        Vector3 dir = new Vector3(moveHorizontal, 0, mvVertical).normalized;

        transform.Translate((mFloorTransform.right* dir.x + mFloorTransform.forward* dir.z)*vel*Time.deltaTime);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Alert();
            Pursue();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //todo dejar de seguir
            //Pursue();
        }
    }

    public void Alert() 
    {
        // Obtiene las posiciones de los colliders.
        Vector3 pos1 = transform.position;
        Vector3 pos2 = mPlayerTransform.transform.position;

        Debug.Log("¡Jugador detectado!");
        if (Vector3.Distance(pos1, pos2) < radioMelee)
        {
            Melee();
        }
    }

    public void Melee()
    {
        // Calcula la dirección hacia el jugador sin cambiar la rotación del enemigo.
        Vector3 dir = (mPlayerTransform.transform.position - transform.position).normalized;

        //TODO hacer que primero cargue la embestida

        // Mueve el enemigo en la dirección calculada.
        transform.Translate((mFloorTransform.right * dir.x + (mFloorTransform.forward * dir.z)) * (vel * 2) * Time.deltaTime, Space.World);

    }
    void Pursue()
    {
        // Calcula la dirección hacia el jugador sin cambiar la rotación del enemigo.
        Vector3 dir = (mPlayerTransform.transform.position - transform.position).normalized;

        // Mueve el enemigo en la dirección calculada.
        transform.Translate((mFloorTransform.right * dir.x + mFloorTransform.forward * dir.z) * vel * Time.deltaTime, Space.World);
     
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, radioMelee);
    }
}
