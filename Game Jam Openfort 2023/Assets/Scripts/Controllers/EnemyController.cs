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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.localPosition, radioMelee);
    }
}
