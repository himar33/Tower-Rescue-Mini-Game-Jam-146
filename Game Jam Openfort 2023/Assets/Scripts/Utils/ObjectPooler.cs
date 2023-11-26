using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public delegate GameObject CreateNewInstanceDelegate();
    
    [SerializeField]
    string PoolerID;

    [SerializeField]
    Transform mParentTransform;

    List<GameObject> mInstances;

    CreateNewInstanceDelegate CreateNewInstanceFunction;

    private void Start()
    {
        mInstances = new List<GameObject>();
    }
    
    public GameObject GetPooledObject()
    {
        foreach(var ObjectInstance in mInstances)
        {
            if(!ObjectInstance.activeSelf)
                return ObjectInstance;
        }

        GameObject NewGameObject = CreateNewInstanceFunction();
        NewGameObject.transform.SetParent(mParentTransform);
        mInstances.Add(NewGameObject);

        return NewGameObject;
    }

    public void AddConstructor(CreateNewInstanceDelegate aNewInstanceFunction)
    {
        CreateNewInstanceFunction += aNewInstanceFunction;
    }
}
