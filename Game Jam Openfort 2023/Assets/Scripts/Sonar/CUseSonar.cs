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

    [SerializeField]
    ObjectPooler mHUDPingPooler;

    [SerializeField]
    List<sSonarPingType> mPingTypes;

    public void Start()
    {
        mHUDPingPooler.AddConstructor(CreatePingObject);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            foreach(sSonarPingType PingType in mPingTypes)
            {
                for(int itChild = 0; itChild < PingType.mItemParent.childCount; ++itChild)
                {
                    Transform ItemWorldPos = PingType.mItemParent.GetChild(itChild).transform;
                    Vector3 ViewportPos = Camera.main.WorldToScreenPoint(ItemWorldPos.position);

                    GameObject PooledPing = mHUDPingPooler.GetPooledObject();
                    PooledPing.GetComponent<Image>().sprite = PingType.mHUDSprite;
                    PooledPing.transform.position = ViewportPos;
                    Debug.Log(ViewportPos);
                    PooledPing.SetActive(true);
                }
            }
        }
    }

    GameObject CreatePingObject()
    {
        GameObject NewObject = new GameObject();

        NewObject.AddComponent<Image>();

        return NewObject;
    }
}
