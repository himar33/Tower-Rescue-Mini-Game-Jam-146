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

    private SphereCollider mCollider;
    private ObjectPooler mPooler;
    private List<DiscipleController> mDiscipleControllers;

    private void Start()
    {
        mPooler = GetComponent<ObjectPooler>();
        mPooler.AddConstructor(CreateDisciple);

        mDiscipleControllers = new List<DiscipleController>();

        if (Physics.SphereCast(transform.position, mRadius, transform.forward, out RaycastHit hit, 100, mLayerMask, QueryTriggerInteraction.UseGlobal))
        {
            transform.position = hit.point;
            mCollider = GetComponent<SphereCollider>();
            mCollider.radius = mRadius;
            for (int i = 0; i < mAmountDisciples; i++)
            {
                Vector3 pos = hit.point;
                AddDisciple(pos, Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().AmountDisciples = mDiscipleControllers.Count;
            foreach (var disciple in mDiscipleControllers)
            {
                disciple.IsRescued = true;
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

    public void AddDisciple(Vector3 pos, Quaternion rot)
    {
        GameObject mDisciple = mPooler.GetPooledObject();
        mDisciple.transform.position = pos;
        mDisciple.transform.rotation = rot;
        mDisciple.gameObject.SetActive(true);

        DiscipleController mController = mDisciple.GetComponent<DiscipleController>();
        mController.PlayerTransform = GameObject.FindGameObjectWithTag("PlayerFoot").transform;
        mController.IsRescued = false;

        mDiscipleControllers.Add(mController);
    }

    public void RemoveDisciple(int index)
    {
        if (mDiscipleControllers[index] != null)
        {
            mDiscipleControllers[index].gameObject.SetActive(false);
            mDiscipleControllers.RemoveAt(index);
        }
    }

    private GameObject CreateDisciple()
    {
        return Instantiate(mDisciplePrefab);
    }
}
