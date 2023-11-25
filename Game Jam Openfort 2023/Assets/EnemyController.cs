using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public float velocidad = 5f;
    public Transform mFloorTransform;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MoverEnemigo();

    }
    void MoverEnemigo()
    {
        // Obt�n la entrada del teclado o cualquier otra fuente de entrada.
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        // Calcula la direcci�n del movimiento en el plano XZ.
        Vector3 direccion = new Vector3(movimientoHorizontal, 0, movimientoVertical).normalized;

        transform.Translate((mFloorTransform.right* direccion.x + mFloorTransform.forward* direccion.z)*velocidad*Time.deltaTime);

    }
}
