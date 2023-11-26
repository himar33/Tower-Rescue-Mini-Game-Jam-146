using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class DiscipleSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private int mAmountDisciples = 5;
    [SerializeField] private GameObject mDisciplePrefab;
    [SerializeField] private Transform mFloor;

    [Header("GizmoSettings")]
    [SerializeField] private float mRadius = 5f;
    [SerializeField] private Color mColor = Color.red;
    [SerializeField] private float mMaxDistance = 20f;
    [SerializeField] private LayerMask mLayerMask;

    private float mCurrentHitDistance;

    private void Start()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, mRadius, transform.forward, out hit, 100, mLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            for (int i = 0; i < mAmountDisciples; i++)
            {
                Vector3 InstantiatePos = hit.point;
                Instantiate(mDisciplePrefab, InstantiatePos, Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (mFloor != null)
        {
            RaycastHit hit;
            float currentHitDistance;
            if(Physics.SphereCast(transform.position, mRadius, transform.forward, out hit, 100, mLayerMask, QueryTriggerInteraction.UseGlobal))
            {
                currentHitDistance = hit.distance;
            }
            else
            {
                currentHitDistance = mMaxDistance;
            }

            Gizmos.color = mColor;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * currentHitDistance);
            Gizmos.DrawWireSphere(transform.position + transform.forward * currentHitDistance, mRadius);
        }
    }
}
