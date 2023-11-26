using System;
using System.Collections.Generic;
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
        SONAR_STATE_PAN_OUT,
        SONAR_STATE_PAN_REVEAL,
        SONAR_STATE_PAN_ACTIVE,
        SONAR_STATE_PAN_IN,
        SONAR_STATE_FINISH
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
    float mCameraPanTransitionTime = 1;
    
    [SerializeField]
    float mCameraPanActiveTime = 5;
    
    [SerializeField]
    float mCameraPanOffset = 30;

    [SerializeField]
    float mAppearAllTime = 3;

    [SerializeField]
    List<sSonarPingType> mPingTypes;

    List<GameObject> mSequenceObjects;

    TSonarState mState;

    float mMaxDistance;
    float mInitCameraZ;
    float mCurrentTime;

    public Action OnSonarFinished;

    public void Start()
    {
        mHUDPingPooler.AddConstructor(CreatePingObject);
        mInitCameraZ = Camera.main.transform.position.z;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && mState == TSonarState.SONAR_STATE_WAITING)
        {
            mState = TSonarState.SONAR_STATE_PAN_OUT;
            mCurrentTime = 0;

            mSequenceObjects = new List<GameObject>();

            mMaxDistance = GetItemsMaxDistance();
        }
        
        mCurrentTime += Time.deltaTime;

        if(mState == TSonarState.SONAR_STATE_PAN_OUT)
        {
            float Pan01 = Mathf.Clamp01(mCurrentTime / mCameraPanTransitionTime);

            Vector3 InitPos = Vector3.forward * mInitCameraZ;
            Vector3 FinalPos = Vector3.forward * (mInitCameraZ - mCameraPanOffset);

            Camera.main.transform.position = Vector3.Lerp(InitPos, FinalPos, Pan01);

            if(Pan01 == 1)
                mState = TSonarState.SONAR_STATE_PAN_REVEAL;
        }              
        else if(mState == TSonarState.SONAR_STATE_PAN_REVEAL)
        {
            mState = TSonarState.SONAR_STATE_PAN_IN;

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
                    PooledPing.GetComponent<CSonarPing>().InitShow(mStartDelay, mCameraPanActiveTime);
                    PooledPing.transform.position = ViewportPos;
                    PooledPing.SetActive(true);

                    mSequenceObjects.Add(PooledPing);
                }
            }
        }
        else if(mState == TSonarState.SONAR_STATE_PAN_IN && mCurrentTime > mCameraPanTransitionTime + mCameraPanActiveTime)
        {
            float TransitionTime = mCameraPanTransitionTime + mCameraPanActiveTime;
            float FinalTime = 2.0f * mCameraPanTransitionTime + mCameraPanActiveTime;
            float Pan01 = Mathf.Clamp01((mCurrentTime - TransitionTime) / (FinalTime - TransitionTime));

            Vector3 InitPos = Vector3.forward * mInitCameraZ;
            Vector3 PanPos = Vector3.forward * (mInitCameraZ - mCameraPanOffset);
            
            Camera.main.transform.position = Vector3.Lerp(PanPos, InitPos, Pan01);

            if(mCurrentTime > FinalTime)
            {
                mState = TSonarState.SONAR_STATE_FINISH;
                OnSonarFinished();
                mCurrentTime = 0;
            }
        }
    }

    GameObject CreatePingObject()
    {
        GameObject NewObject = new GameObject();

        NewObject.transform.localScale = Vector3.one * 0.5f;
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
