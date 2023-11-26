using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct sSonarPingType
{
    public Transform mItemParent;
    public Sprite mHUDSprite;
};

public class CUseSonar : MonoBehaviour
{
    enum TSonarState
    {
        SONAR_STATE_WAITING,
        SONAR_STATE_REVEALING,
        SONAR_STATE_REVEALED,
    }

    public struct sObjectAppearSequenceInfo
    {
        public GameObject ObjectToAppear;
        public float AppearTime;
    }

    [SerializeField]
    ObjectPooler mHUDPingPooler;

    [SerializeField]
    Transform mSonarCenter;
    
    [SerializeField]
    float mAppearAllTime = 3;

    [SerializeField]
    List<sSonarPingType> mPingTypes;

    List<GameObject> mSequenceObjects;

    TSonarState mState;

    float mMaxDistance;

    public void Start()
    {
        mHUDPingPooler.AddConstructor(CreatePingObject);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && mState == TSonarState.SONAR_STATE_WAITING)
        {
            mState = TSonarState.SONAR_STATE_REVEALING;

            mSequenceObjects = new List<GameObject>();

            mMaxDistance = GetItemsMaxDistance();

            foreach(sSonarPingType PingType in mPingTypes)
            {
                for(int itChild = 0; itChild < PingType.mItemParent.childCount; ++itChild)
                {
                    Vector3 ItemWorldPos = PingType.mItemParent.GetChild(itChild).transform.position;
                    Vector3 ViewportPos = Camera.main.WorldToScreenPoint(ItemWorldPos);

                    float SqrDistanceToCenter = (ItemWorldPos - mSonarCenter.position).sqrMagnitude;
                    float mStartDelay = SqrDistanceToCenter / mMaxDistance * mAppearAllTime;

                    GameObject PooledPing = mHUDPingPooler.GetPooledObject();
                    PooledPing.GetComponent<Image>().sprite = PingType.mHUDSprite;
                    PooledPing.GetComponent<CSonarPing>().InitShow(mStartDelay);
                    PooledPing.transform.position = ViewportPos;
                    PooledPing.SetActive(true);

                    mSequenceObjects.Add(PooledPing);
                }
            }
        }
    }

    GameObject CreatePingObject()
    {
        GameObject NewObject = new GameObject();

        NewObject.AddComponent<Image>();
        NewObject.AddComponent<CSonarPing>();

        return NewObject;
    }

    private float GetItemsMaxDistance()
    {
        float MaxSqrDistance = 0;
        
        if(Input.GetKeyDown(KeyCode.Return))
        {
            foreach(sSonarPingType PingType in mPingTypes)
            {
                for(int itChild = 0; itChild < PingType.mItemParent.childCount; ++itChild)
                {   
                    Vector2 WorldPos2D = (Vector2)PingType.mItemParent.GetChild(itChild).transform.position;
                    Vector2 LocalSpaceVector = WorldPos2D - (Vector2)mSonarCenter.transform.position;
                    
                    if(LocalSpaceVector.sqrMagnitude > MaxSqrDistance)
                    {
                        MaxSqrDistance = LocalSpaceVector.sqrMagnitude;
                    }
                }
            }
        }

        return MaxSqrDistance;
    }
}
