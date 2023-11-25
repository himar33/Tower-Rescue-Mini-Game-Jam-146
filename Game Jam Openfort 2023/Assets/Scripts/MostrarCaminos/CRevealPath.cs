using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class CRevealPath : MonoBehaviour
{
    [SerializeField]
    Material mMaskMaterial;
    
    [SerializeField]
    CSpellScriptableObject mSpellInfo;

    [SerializeField]
    LayerMask mRevealObjectMask;
    
    [SerializeField]
    int mDrawMaskBufferLayer;
    
    // Drag positions from camera to world
    Vector3 mInitWorldPos;
    Vector3 mCurrentWorldPos;

    readonly KeyCode mClickKeycode;

    public CRevealPath()
    {
        mInitWorldPos = Vector2.zero;
        mClickKeycode = KeyCode.Mouse0;
    }
    
    private void Update() 
    {
        ClickAndDrag();
    }

    void ClickAndDrag()
    {
        if(Input.GetKeyDown(mClickKeycode))
        {
            Ray ClickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(ClickRay, out RaycastHit HitInfo, Mathf.Infinity, mRevealObjectMask))
            {
                mInitWorldPos = HitInfo.point;
            }
            else
            {
                Debug.LogError("There should be an object to mask a reveal path");
            }
        }

        if(Input.GetKey(mClickKeycode))
        {
            Ray DragRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if(Physics.Raycast(DragRay, out RaycastHit HitInfo, Mathf.Infinity, mRevealObjectMask))
            {
                mCurrentWorldPos = HitInfo.point;
            }
            else
            {
                Debug.LogError("There should be an object to mask a reveal path");
                mCurrentWorldPos = mInitWorldPos;
            }
        }

        if(Input.GetKeyUp(mClickKeycode) && mCurrentWorldPos != mInitWorldPos)
        {
            Mesh MaskMesh = CreateBoundMesh(mInitWorldPos, mCurrentWorldPos);
            
            var NewMaskObject = CreateMaskGameobject(MaskMesh);
            NewMaskObject.GetComponent<CSpellRevealMask>().Config(mSpellInfo);

            mInitWorldPos = Vector2.zero;
        } 
    }

    void ClampPositionToTopLeftCorner()
    {
        mCurrentWorldPos.x = Math.Max(mCurrentWorldPos.x, mInitWorldPos.x);
        mCurrentWorldPos.y = Math.Max(mCurrentWorldPos.y, mInitWorldPos.y);
    }

    Mesh CreateBoundMesh(Vector3 aPointA, Vector3 aPointB)
    {
        Mesh NewMesh = new Mesh();

        Vector3 TopLeft = aPointA;
        Vector3 BotRight = aPointB;
        Vector3 TopRight = new Vector3(BotRight.x, TopLeft.y, TopLeft.z);
        Vector3 BotLeft = new Vector3(TopLeft.x, BotRight.y, BotRight.z);
        
        if(aPointA.y > aPointB.y)
        {
            if(aPointA.x > aPointB.x)
            {
                (BotLeft, BotRight) = (BotRight, BotLeft);
                (TopLeft, TopRight) = (TopRight, TopLeft);
            }
        }
        else
        {
            (TopRight, BotRight) = (BotRight, TopRight);
            (TopLeft, BotLeft) = (BotLeft, TopLeft);

            if(aPointA.x > aPointB.x)
            {
                (TopLeft, TopRight) = (TopRight, TopLeft);
                (BotLeft, BotRight) = (BotRight, BotLeft);
            }
        }


        List<Vector3> Vertices = new List<Vector3>()
        {
            TopLeft,
            TopRight,
            BotLeft,
            BotRight
        };

        List<int> TriangleIndices = new List<int>()
        {
            0,1,2,
            2,1,3
        };

        List<Vector2> UVs = new List<Vector2>()
        {
            Vector2.up,
            Vector2.one,
            Vector2.zero,
            Vector2.right
        };

        NewMesh.SetVertices(Vertices);
        NewMesh.SetUVs(0, UVs);
        NewMesh.SetTriangles(TriangleIndices, 0);

        return NewMesh;
    }

    GameObject CreateMaskGameobject(Mesh aMaskMesh)
    {
        var NewMaskObject = new GameObject("RevealMask");

        NewMaskObject.layer = mDrawMaskBufferLayer;

        var MeshFilterComponent = NewMaskObject.AddComponent<MeshFilter>();
        MeshFilterComponent.mesh = aMaskMesh;

        var RendererComponent = NewMaskObject.AddComponent<MeshRenderer>();
        RendererComponent.material = mMaskMaterial;

        var SpellMaskComponent = NewMaskObject.AddComponent<CSpellRevealMask>();

        return NewMaskObject;
    }
}
